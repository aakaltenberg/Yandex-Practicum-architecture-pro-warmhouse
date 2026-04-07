package handlers

import (
	"context"
	"fmt"
	"log"
	"net/http"
	"strconv"

	"smarthome/db"
	"smarthome/models"
	"smarthome/services"
	"smarthome/gateclient"

	"github.com/gin-gonic/gin"
)

// SensorHandler handles sensor-related requests
type SensorHandler struct {
    DB                 *db.DB
    TemperatureService *services.TemperatureService
    GateClient         *gateclient.GateClient
}

// NewSensorHandler creates a new SensorHandler
func NewSensorHandler(db *db.DB, temperatureService *services.TemperatureService, gateClient *gateclient.GateClient) *SensorHandler {
    return &SensorHandler{
        DB:                 db,
        TemperatureService: temperatureService,
        GateClient:         gateClient,
    }
}

// RegisterRoutes registers the sensor routes
func (h *SensorHandler) RegisterRoutes(router *gin.RouterGroup) {
	sensors := router.Group("/sensors")
	{
		sensors.GET("", h.GetSensors)
		sensors.GET("/:id", h.GetSensorByID)
		sensors.POST("", h.CreateSensor)
		sensors.PUT("/:id", h.UpdateSensor)
		sensors.DELETE("/:id", h.DeleteSensor)
		sensors.PATCH("/:id/value", h.UpdateSensorValue)
		sensors.GET("/temperature/:location", h.GetTemperatureByLocation)
	}
}

// GetSensors handles GET /api/v1/sensors
func (h *SensorHandler) GetSensors(c *gin.Context) {
    // Получаем все сенсоры из локальной БД
	sensors, err := h.DB.GetSensors(context.Background())
    if err != nil {
        c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
        return
    }

	var sensorsWithoutGates []models.Sensor
	for _, s := range sensors {
		if s.Type != models.Gate {
			sensorsWithoutGates = append(sensorsWithoutGates, s)
	    }
	}

	// Обновление температуры
	for i := range sensorsWithoutGates {
		if sensorsWithoutGates[i].Type == models.Temperature {
			tempData, err := h.TemperatureService.GetTemperatureByID(fmt.Sprintf("%d", sensorsWithoutGates[i].ID))
			if err == nil {
				sensorsWithoutGates[i].Value = tempData.Value
				sensorsWithoutGates[i].Status = tempData.Status
				sensorsWithoutGates[i].LastUpdated = tempData.Timestamp
			} else {
				log.Printf("Failed to fetch temperature data for sensor %d: %v", sensorsWithoutGates[i].ID, err)
			}
		}
	}

    // Получаем все ворота из Gate Service
    gates, err := h.GateClient.GetAllGates(context.Background())
    if err != nil {
        log.Printf("Failed to fetch gates from Gate Service: %v", err)
        gates = []gateclient.Gate{}
    }

    // Преобразуем gates в формат Sensor
    var allSensors []models.Sensor
    allSensors = append(allSensors, sensorsWithoutGates...)
    for _, g := range gates {
        allSensors = append(allSensors, models.Sensor{
            ID:          g.ID,
            Name:        g.Name,
            Type:        models.Gate,
            Location:    g.Location,
            Value:       0,
            Unit:        "",
            Status:      g.Status,
            LastUpdated: g.LastUpdated,
            CreatedAt:   g.CreatedAt,
        })
    }

    c.JSON(http.StatusOK, allSensors)
}

// GetSensorByID handles GET /api/v1/sensors/:id
func (h *SensorHandler) GetSensorByID(c *gin.Context) {
    id, err := strconv.Atoi(c.Param("id"))
    if err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid sensor ID"})
        return
    }

    // Определяем тип
    sensor, err := h.DB.GetSensorByID(context.Background(), id)
	if err == nil && sensor.Type == models.Temperature {
        tempData, err := h.TemperatureService.GetTemperatureByID(fmt.Sprintf("%d", sensor.ID))
        if err == nil {
            sensor.Value = tempData.Value
            sensor.Status = tempData.Status
            sensor.LastUpdated = tempData.Timestamp
        } else {
            log.Printf("Failed to fetch temperature data for sensor %d: %v", sensor.ID, err)
        }
        c.JSON(http.StatusOK, sensor)
        return
	}

	if err == nil && sensor.Type == models.Gate {
		gate, err := h.GateClient.GetGate(context.Background(), id)		
		if err != nil {
			c.JSON(http.StatusNotFound, gin.H{"error": "Sensor not found"})
			return
		}
		
		// Преобразуем gate в Sensor
        sensor = models.Sensor{
        ID:          gate.ID,
        Name:        gate.Name,
        Type:        models.Gate,
        Location:    gate.Location,
        Value:       0,
        Unit:        "",
        Status:      gate.Status,
        LastUpdated: gate.LastUpdated,
        CreatedAt:   gate.CreatedAt,
	    }
    }

    c.JSON(http.StatusOK, sensor)
}

// GetTemperatureByLocation handles GET /api/v1/sensors/temperature/:location
func (h *SensorHandler) GetTemperatureByLocation(c *gin.Context) {
	location := c.Param("location")
	if location == "" {
		c.JSON(http.StatusBadRequest, gin.H{"error": "Location is required"})
		return
	}

	// Fetch temperature data from the external API
	tempData, err := h.TemperatureService.GetTemperature(location)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{
			"error": fmt.Sprintf("Failed to fetch temperature data: %v", err),
		})
		return
	}

	// Return the temperature data
	c.JSON(http.StatusOK, gin.H{
		"location":    tempData.Location,
		"value":       tempData.Value,
		"unit":        tempData.Unit,
		"status":      tempData.Status,
		"timestamp":   tempData.Timestamp,
		"description": tempData.Description,
	})
}

// CreateSensor handles POST /api/v1/sensors
func (h *SensorHandler) CreateSensor(c *gin.Context) {
    var sensorCreate models.SensorCreate
    if err := c.ShouldBindJSON(&sensorCreate); err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
        return
    }

	sensor, err := h.DB.CreateSensor(context.Background(), sensorCreate)
    if err != nil {
        c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
        return
    }

    if sensorCreate.Type == models.Gate {
        // Создаём ворота через Gate Service
        gate, err := h.GateClient.CreateGate(context.Background(), gateclient.GateCreate{
			Id:       sensor.ID,
            Name:     sensorCreate.Name,
            Location: sensorCreate.Location,
            HouseId:  "",
        })
        if err != nil {
            c.JSON(http.StatusInternalServerError, gin.H{"error": "Failed to create gate in Gate Service"})
            return
        }
        // Возвращаем ответ в формате Sensor
        c.JSON(http.StatusCreated, models.Sensor{
            ID:          gate.ID,
            Name:        gate.Name,
            Type:        models.Gate,
            Location:    gate.Location,
            Value:       0,
            Unit:        "",
            Status:      gate.Status,
            LastUpdated: gate.LastUpdated,
            CreatedAt:   gate.CreatedAt,
        })
		return
    }

    c.JSON(http.StatusCreated, sensor)
}

// UpdateSensor handles PUT /api/v1/sensors/:id
func (h *SensorHandler) UpdateSensor(c *gin.Context) {
    id, err := strconv.Atoi(c.Param("id"))
    if err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid sensor ID"})
        return
    }

    var sensorUpdate models.SensorUpdate
    if err := c.ShouldBindJSON(&sensorUpdate); err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
        return
    }

    // Пытаемся определить тип. Сначала проверяем локальную БД.
    sensorDb, err := h.DB.GetSensorByID(context.Background(), id)

	if err == nil && sensorDb.Type == models.Gate {
		// Для gate обновляем через Gate Service
		updatedGate, err := h.GateClient.UpdateGate(context.Background(), id, gateclient.GateUpdate{
			Name:     sensorUpdate.Name,
			Location: sensorUpdate.Location,
			HouseId:  "",
		})
		if err != nil {
			c.JSON(http.StatusNotFound, gin.H{"error": "Sensor not found or update failed"})
			return
		}
		
		c.JSON(http.StatusOK, models.Sensor{
			ID:          updatedGate.ID,
			Name:        updatedGate.Name,
			Type:        models.Gate,
			Location:    updatedGate.Location,
			Value:       0,
			Unit:        "",
			Status:      updatedGate.Status,
			LastUpdated: updatedGate.LastUpdated,
			CreatedAt:   updatedGate.CreatedAt,
		})

		_, err = h.DB.UpdateSensor(context.Background(), id, sensorUpdate)
        if err != nil {
            c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
            return
        }
    }

    if err == nil && sensorDb.Type == models.Temperature {
        // Обновляем температурный сенсор
        sensor, err := h.DB.UpdateSensor(context.Background(), id, sensorUpdate)
        if err != nil {
            c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
            return
        }
        c.JSON(http.StatusOK, sensor)
        return
    }
}

// DeleteSensor handles DELETE /api/v1/sensors/:id
func (h *SensorHandler) DeleteSensor(c *gin.Context) {
    id, err := strconv.Atoi(c.Param("id"))
    if err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid sensor ID"})
        return
    }

    // Проверяем тип.
    sensorDb, err := h.DB.GetSensorByID(context.Background(), id)

    if err == nil && sensorDb.Type == models.Gate {
		err = h.GateClient.DeleteGate(context.Background(), id)
		if err != nil {
			c.JSON(http.StatusNotFound, gin.H{"error": "Sensor not found"})
			return
		}
	}
	
    err = h.DB.DeleteSensor(context.Background(), id)
    if err == nil {
        // Удалили температурный сенсор
        c.JSON(http.StatusOK, gin.H{"message": "Sensor deleted successfully"})
        return
    }

    c.JSON(http.StatusOK, gin.H{"message": "Sensor deleted successfully"})
}

// UpdateSensorValue handles PATCH /api/v1/sensors/:id/value
func (h *SensorHandler) UpdateSensorValue(c *gin.Context) {
    id, err := strconv.Atoi(c.Param("id"))
    if err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid sensor ID"})
        return
    }

    var request struct {
        Value  float64 `json:"value" binding:"required"`
        Status string  `json:"status" binding:"required"`
    }

    if err := c.ShouldBindJSON(&request); err != nil {
        c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
        return
    }

    // Проверяем тип. Сначала локальная БД.
    sensorDb, err := h.DB.GetSensorByID(context.Background(), id)

    if err == nil && sensorDb.Type == models.Temperature {
        // Обновляем значение температурного сенсора
        err = h.DB.UpdateSensorValue(context.Background(), id, request.Value, request.Status)
        if err != nil {
            c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
            return
        }
        c.JSON(http.StatusOK, gin.H{"message": "Sensor value updated successfully"})
        return
    }

    if err == nil && sensorDb.Type == models.Gate {
	    // Для gate вызываем open/close
		if request.Status == "open" {
			err = h.GateClient.OpenGate(context.Background(), id)
		} else if request.Status == "closed" {
			err = h.GateClient.CloseGate(context.Background(), id)
		} else {
			c.JSON(http.StatusBadRequest, gin.H{"error": "Invalid status for gate"})
			return
		}
	}

    if err != nil {
        c.JSON(http.StatusInternalServerError, gin.H{"error": "Failed to update gate status"})
        return
    }

    c.JSON(http.StatusOK, gin.H{"message": "Gate status updated successfully"})
}
