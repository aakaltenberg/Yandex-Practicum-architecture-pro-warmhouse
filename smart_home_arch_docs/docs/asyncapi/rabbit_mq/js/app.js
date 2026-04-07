
    const schema = {
  "asyncapi": "2.6.0",
  "info": {
    "title": "SmartHome (rabbitMQ) AsyncAPI",
    "version": "1.1.0",
    "description": "Асинхронные взаимодействия между микросервисами SmartHome System"
  },
  "servers": {
    "rabbitmq": {
      "url": "amqp://rabbitmq:5672",
      "protocol": "amqp",
      "description": "RabbitMQ брокер для событий и команд"
    }
  },
  "channels": {
    "event.house": {
      "description": "События изменения домов",
      "publish": {
        "operationId": "publishHouseEvent",
        "message": {
          "summary": "Событие, связанное с домом",
          "contentType": "application/json",
          "payload": {
            "type": "object",
            "properties": {
              "eventType": {
                "type": "string",
                "enum": [
                  "HouseCreated",
                  "HouseUpdated",
                  "HouseDeleted"
                ],
                "x-parser-schema-id": "<anonymous-schema-2>"
              },
              "timestamp": {
                "type": "string",
                "format": "date-time",
                "x-parser-schema-id": "<anonymous-schema-3>"
              },
              "houseId": {
                "type": "string",
                "format": "uuid",
                "x-parser-schema-id": "<anonymous-schema-4>"
              },
              "data": {
                "type": "object",
                "properties": {
                  "name": {
                    "type": "string",
                    "x-parser-schema-id": "<anonymous-schema-6>"
                  },
                  "address": {
                    "type": "string",
                    "x-parser-schema-id": "<anonymous-schema-7>"
                  },
                  "ownerId": {
                    "type": "string",
                    "format": "uuid",
                    "x-parser-schema-id": "<anonymous-schema-8>"
                  }
                },
                "x-parser-schema-id": "<anonymous-schema-5>"
              }
            },
            "x-parser-schema-id": "<anonymous-schema-1>"
          },
          "x-parser-message-name": "HouseEvent"
        }
      },
      "subscribe": {
        "operationId": "subscribeHouseEvent",
        "message": "$ref:$.channels.event.house.publish.message"
      }
    },
    "event.user": {
      "description": "События изменения пользователей",
      "publish": {
        "operationId": "publishUserEvent",
        "message": {
          "summary": "Событие, связанное с пользователем",
          "contentType": "application/json",
          "payload": {
            "type": "object",
            "properties": {
              "eventType": {
                "type": "string",
                "enum": [
                  "UserCreated",
                  "UserUpdated",
                  "UserDeleted"
                ],
                "x-parser-schema-id": "<anonymous-schema-10>"
              },
              "timestamp": {
                "type": "string",
                "format": "date-time",
                "x-parser-schema-id": "<anonymous-schema-11>"
              },
              "userId": {
                "type": "string",
                "format": "uuid",
                "x-parser-schema-id": "<anonymous-schema-12>"
              },
              "data": {
                "type": "object",
                "properties": {
                  "email": {
                    "type": "string",
                    "x-parser-schema-id": "<anonymous-schema-14>"
                  },
                  "fullName": {
                    "type": "string",
                    "x-parser-schema-id": "<anonymous-schema-15>"
                  }
                },
                "x-parser-schema-id": "<anonymous-schema-13>"
              }
            },
            "x-parser-schema-id": "<anonymous-schema-9>"
          },
          "x-parser-message-name": "UserEvent"
        }
      },
      "subscribe": {
        "operationId": "subscribeUserEvent",
        "message": "$ref:$.channels.event.user.publish.message"
      }
    },
    "event.heating": {
      "description": "События изменения состояния устройств отопления",
      "publish": {
        "operationId": "publishHeatingEvent",
        "message": {
          "summary": "Изменение состояния устройства",
          "contentType": "application/json",
          "payload": {
            "type": "object",
            "properties": {
              "deviceId": {
                "type": "string",
                "format": "uuid",
                "x-parser-schema-id": "<anonymous-schema-17>"
              },
              "houseId": {
                "type": "string",
                "format": "uuid",
                "x-parser-schema-id": "<anonymous-schema-18>"
              },
              "deviceType": {
                "type": "string",
                "enum": [
                  "heating",
                  "lighting",
                  "camera",
                  "gate"
                ],
                "x-parser-schema-id": "<anonymous-schema-19>"
              },
              "status": {
                "type": "string",
                "x-parser-schema-id": "<anonymous-schema-20>"
              },
              "value": {
                "type": "number",
                "x-parser-schema-id": "<anonymous-schema-21>"
              },
              "unit": {
                "type": "string",
                "x-parser-schema-id": "<anonymous-schema-22>"
              },
              "timestamp": {
                "type": "string",
                "format": "date-time",
                "x-parser-schema-id": "<anonymous-schema-23>"
              }
            },
            "x-parser-schema-id": "<anonymous-schema-16>"
          },
          "x-parser-message-name": "DeviceStateChanged"
        }
      },
      "subscribe": {
        "operationId": "subscribeHeatingEvent",
        "message": "$ref:$.channels.event.heating.publish.message"
      }
    },
    "event.lighting": {
      "description": "События изменения состояния устройств освещения",
      "publish": {
        "operationId": "publishLightingEvent",
        "message": "$ref:$.channels.event.heating.publish.message"
      },
      "subscribe": {
        "operationId": "subscribeLightingEvent",
        "message": "$ref:$.channels.event.heating.publish.message"
      }
    },
    "event.camera": {
      "description": "События изменения состояния камер",
      "publish": {
        "operationId": "publishCameraEvent",
        "message": "$ref:$.channels.event.heating.publish.message"
      },
      "subscribe": {
        "operationId": "subscribeCameraEvent",
        "message": "$ref:$.channels.event.heating.publish.message"
      }
    },
    "event.gate": {
      "description": "События изменения состояния ворот",
      "publish": {
        "operationId": "publishGateEvent",
        "message": "$ref:$.channels.event.heating.publish.message"
      },
      "subscribe": {
        "operationId": "subscribeGateEvent",
        "message": "$ref:$.channels.event.heating.publish.message"
      }
    },
    "command.heating": {
      "description": "Команды управления устройствами отопления",
      "publish": {
        "operationId": "publishHeatingCommand",
        "message": {
          "summary": "Команда управления устройством (от Scenario Service)",
          "contentType": "application/json",
          "payload": {
            "type": "object",
            "properties": {
              "commandId": {
                "type": "string",
                "format": "uuid",
                "x-parser-schema-id": "<anonymous-schema-25>"
              },
              "deviceId": {
                "type": "string",
                "format": "uuid",
                "x-parser-schema-id": "<anonymous-schema-26>"
              },
              "command": {
                "type": "string",
                "enum": [
                  "turn_on",
                  "turn_off",
                  "set_temperature",
                  "set_brightness",
                  "start_recording",
                  "open_gate",
                  "close_gate"
                ],
                "x-parser-schema-id": "<anonymous-schema-27>"
              },
              "parameters": {
                "type": "object",
                "additionalProperties": true,
                "x-parser-schema-id": "<anonymous-schema-28>"
              },
              "timestamp": {
                "type": "string",
                "format": "date-time",
                "x-parser-schema-id": "<anonymous-schema-29>"
              }
            },
            "x-parser-schema-id": "<anonymous-schema-24>"
          },
          "x-parser-message-name": "DeviceCommand"
        }
      },
      "subscribe": {
        "operationId": "subscribeHeatingCommand",
        "message": "$ref:$.channels.command.heating.publish.message"
      }
    },
    "command.lighting": {
      "description": "Команды управления устройствами освещения",
      "publish": {
        "operationId": "publishLightingCommand",
        "message": "$ref:$.channels.command.heating.publish.message"
      },
      "subscribe": {
        "operationId": "subscribeLightingCommand",
        "message": "$ref:$.channels.command.heating.publish.message"
      }
    },
    "command.camera": {
      "description": "Команды управления камерами",
      "publish": {
        "operationId": "publishCameraCommand",
        "message": "$ref:$.channels.command.heating.publish.message"
      },
      "subscribe": {
        "operationId": "subscribeCameraCommand",
        "message": "$ref:$.channels.command.heating.publish.message"
      }
    },
    "command.gate": {
      "description": "Команды управления воротами",
      "publish": {
        "operationId": "publishGateCommand",
        "message": "$ref:$.channels.command.heating.publish.message"
      },
      "subscribe": {
        "operationId": "subscribeGateCommand",
        "message": "$ref:$.channels.command.heating.publish.message"
      }
    },
    "event.device": {
      "description": "Общий канал для событий всех типов устройств",
      "publish": {
        "operationId": "publishDeviceEvent",
        "message": "$ref:$.channels.event.heating.publish.message"
      },
      "subscribe": {
        "operationId": "subscribeDeviceEvent",
        "message": "$ref:$.channels.event.heating.publish.message"
      }
    },
    "command.device": {
      "description": "Общий канал для команд всех типов устройств",
      "publish": {
        "operationId": "publishDeviceCommand",
        "message": "$ref:$.channels.command.heating.publish.message"
      },
      "subscribe": {
        "operationId": "subscribeDeviceCommand",
        "message": "$ref:$.channels.command.heating.publish.message"
      }
    }
  },
  "components": {
    "messages": {
      "HouseEvent": "$ref:$.channels.event.house.publish.message",
      "UserEvent": "$ref:$.channels.event.user.publish.message",
      "DeviceStateChanged": "$ref:$.channels.event.heating.publish.message",
      "DeviceCommand": "$ref:$.channels.command.heating.publish.message"
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
  