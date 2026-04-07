```puml
@startuml title Telemetry Service - Component Diagram

!includeurl https://raw.githubusercontent.com/RicardoNiepel/C4-PlantUML/master/C4_Component.puml

System_Ext(telemetry_db, "Telemetry DB", "TimescaleDB", "Хранение исторических данных телеметрии")
System_Ext(message_broker, "Message Broker", "RabbitMQ", "Обмен событиями между микросервисами")
System_Ext(device_gateway, "Device Gateway", "MQTT Broker", "Потоковая телеметрия от устройств")

Container_Boundary(telemetry_service, "Telemetry Service", ".NET") {
    Component(telemetry_controller, "Telemetry Controller", "ASP.NET Core", "REST API для запросов телеметрии (по устройствам, по времени, агрегация)")

    Component(business_logic, "Telemetry Business Logic Layer", ".NET", "Обработка входящих данных, запросов о телеметрии")
    Component(repository, "Repository", ".NET / EF Core", "Доступ к базе данных Telemetry DB для чтения/записи")

    Component(mqtt_subscriber, "MQTT Subscriber", ".NET / MQTTnet", "Подписка на топики телеметрии устройств")
    Component(rabbit_subscriber, "RabbitMQ Subscriber", ".NET / RabbitMQ.Client", "Подписка на события об изменении состояния устройств от сервисов устройств")
}

Rel(mqtt_subscriber, business_logic, "Передача полученных данных телеметрии", "")
Rel(rabbit_subscriber, business_logic, "Передача событий об изменении состояния устройств", "")
Rel(business_logic, repository, "Сохранение обработанных данных", "")
Rel(telemetry_controller, business_logic, "Обработка клиентских запросов", "")

Rel(mqtt_subscriber, device_gateway, "Подписка на топики", "MQTT")
Rel(rabbit_subscriber, message_broker, "Подписка на события", "AMQP")
Rel(repository, telemetry_db, "CRUD", "SQL")

@enduml
```