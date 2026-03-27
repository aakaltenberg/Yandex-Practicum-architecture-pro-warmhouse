
    const schema = {
  "asyncapi": "2.6.0",
  "info": {
    "title": "SmartHome (MQTT) AsyncAPI",
    "version": "1.1.0",
    "description": "Асинхронные взаимодействия между микросервисами SmartHome System"
  },
  "servers": {
    "mqtt": {
      "url": "mqtt://device-gateway:1883",
      "protocol": "mqtt",
      "description": "MQTT брокер для связи с физическими устройствами"
    }
  },
  "channels": {
    "devices/{device_id}/command": {
      "parameters": {
        "device_id": {
          "schema": {
            "type": "string",
            "x-parser-schema-id": "device_id"
          }
        }
      },
      "publish": {
        "operationId": "sendDeviceCommand",
        "message": {
          "summary": "Команда для физического устройства (MQTT)",
          "contentType": "application/json",
          "payload": {
            "type": "object",
            "properties": {
              "command": {
                "type": "string",
                "x-parser-schema-id": "<anonymous-schema-2>"
              },
              "parameters": {
                "type": "object",
                "x-parser-schema-id": "<anonymous-schema-3>"
              },
              "correlationId": {
                "type": "string",
                "x-parser-schema-id": "<anonymous-schema-4>"
              },
              "timestamp": {
                "type": "string",
                "format": "date-time",
                "x-parser-schema-id": "<anonymous-schema-5>"
              }
            },
            "x-parser-schema-id": "<anonymous-schema-1>"
          },
          "x-parser-message-name": "DeviceCommandMQTT"
        }
      },
      "subscribe": {
        "operationId": "receiveDeviceCommand",
        "message": "$ref:$.channels.devices/{device_id}/command.publish.message"
      }
    },
    "devices/{device_id}/telemetry": {
      "parameters": {
        "device_id": {
          "schema": {
            "type": "string",
            "x-parser-schema-id": "device_id"
          }
        }
      },
      "publish": {
        "operationId": "publishTelemetry",
        "message": {
          "summary": "Телеметрия от физического устройства",
          "contentType": "application/json",
          "payload": {
            "type": "object",
            "properties": {
              "deviceId": {
                "type": "string",
                "format": "uuid",
                "x-parser-schema-id": "<anonymous-schema-7>"
              },
              "deviceType": {
                "type": "string",
                "x-parser-schema-id": "<anonymous-schema-8>"
              },
              "value": {
                "type": "number",
                "x-parser-schema-id": "<anonymous-schema-9>"
              },
              "unit": {
                "type": "string",
                "x-parser-schema-id": "<anonymous-schema-10>"
              },
              "timestamp": {
                "type": "string",
                "format": "date-time",
                "x-parser-schema-id": "<anonymous-schema-11>"
              }
            },
            "x-parser-schema-id": "<anonymous-schema-6>"
          },
          "x-parser-message-name": "TelemetryData"
        }
      },
      "subscribe": {
        "operationId": "subscribeTelemetry",
        "message": "$ref:$.channels.devices/{device_id}/telemetry.publish.message"
      }
    },
    "devices/{device_id}/ack": {
      "parameters": {
        "device_id": {
          "schema": {
            "type": "string",
            "x-parser-schema-id": "device_id"
          }
        }
      },
      "publish": {
        "operationId": "publishCommandAck",
        "message": {
          "summary": "Подтверждение выполнения команды от устройства",
          "contentType": "application/json",
          "payload": {
            "type": "object",
            "properties": {
              "correlationId": {
                "type": "string",
                "x-parser-schema-id": "<anonymous-schema-13>"
              },
              "deviceId": {
                "type": "string",
                "format": "uuid",
                "x-parser-schema-id": "<anonymous-schema-14>"
              },
              "status": {
                "type": "string",
                "enum": [
                  "success",
                  "failure"
                ],
                "x-parser-schema-id": "<anonymous-schema-15>"
              },
              "result": {
                "type": "object",
                "x-parser-schema-id": "<anonymous-schema-16>"
              },
              "timestamp": {
                "type": "string",
                "format": "date-time",
                "x-parser-schema-id": "<anonymous-schema-17>"
              }
            },
            "x-parser-schema-id": "<anonymous-schema-12>"
          },
          "x-parser-message-name": "CommandAck"
        }
      },
      "subscribe": {
        "operationId": "subscribeCommandAck",
        "message": "$ref:$.channels.devices/{device_id}/ack.publish.message"
      }
    }
  },
  "components": {
    "messages": {
      "DeviceCommandMQTT": "$ref:$.channels.devices/{device_id}/command.publish.message",
      "TelemetryData": "$ref:$.channels.devices/{device_id}/telemetry.publish.message",
      "CommandAck": "$ref:$.channels.devices/{device_id}/ack.publish.message"
    }
  },
  "x-parser-spec-parsed": true,
  "x-parser-api-version": 3,
  "x-parser-spec-stringified": true
};
    const config = {"show":{"sidebar":true},"sidebar":{"showOperations":"byDefault"}};
    const appRoot = document.getElementById('root');
    AsyncApiStandalone.render(
        { schema, config, }, appRoot
    );
  