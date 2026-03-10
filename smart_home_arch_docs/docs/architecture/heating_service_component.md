```puml
@startuml title Heating Service - Component Diagram

top to bottom direction

!includeurl https://raw.githubusercontent.com/RicardoNiepel/C4-PlantUML/master/C4_Component.puml

System_Ext(heating_db, "Heating DB", "PostgreSQL", "Данные об устройствах системы отопления")
System_Ext(message_broker, "Message Broker", "RabbitMQ", "Обмен событиями между микросервисами")
System_Ext(device_gateway, "Device Gateway", "MQTT Broker", "Связь с физическими устройствами системы отопления")

Container_Boundary(heating_service, "Heating Service", ".NET") {
    Component(device_mgmt_controller, "Device Management Controller", "ASP.NET Core", "REST API для регистрации, обновления, удаления устройств системы отопления")
    Component(device_cmd_controller, "Device Command Controller", "ASP.NET Core", "REST API для отправки команд управления на устройства систумы отопления")

    Component(business_logic, "Business Logic Layer", ".NET", "Реализует сценарии администрирования и управления устройствами отопления, валидацию модели")
    Component(repository, "Repository", ".NET / EF Core", "Доступ к базе данных Heating DB")
    
    Component(mqtt_publisher, "MQTT Publisher", ".NET / MQTTnet", "Отправляет команды устройствам системы отопления через Device Gateway")
    Component(mqtt_subscriber, "MQTT Subscriber", ".NET / MQTTnet", "Подписывается на топики подтверждений исполнения команд устройств отопления")
    
    Component(rabbit_publisher, "RabbitMQ Publisher", ".NET / RabbitMQ.Client", "Публикует события об изменении состояния устройств системы отопления")
    Component(rabbit_subscriber, "RabbitMQ Subscriber", ".NET / RabbitMQ.Client", "Подписывается на события исполнения команд устройствами отопления от микросервиса управления сценариями")
}

Rel(device_mgmt_controller, business_logic, "Вызывает методы управления метаданными", "")
Rel(device_cmd_controller, business_logic, "Вызывает методы выполнения команд", "")

Rel(business_logic, mqtt_publisher, "1. Отправляет команду устройству", "")
Rel(mqtt_subscriber, business_logic, "2. Уведомляет о подтверждении выполнения команды от устройства", "")
Rel(business_logic, rabbit_publisher, "3. Публикует событие после подтверждения", "")

Rel(business_logic, repository, "Читает/пишет/обновляет данные", "")
Rel(rabbit_subscriber, business_logic, "Уведомляет о входящих событиях - командах на исполнения, например, от микросервиса управления сценариями", "")

Rel(repository, heating_db, "CRUD", "SQL")
Rel(mqtt_publisher, device_gateway, "Публикация команд для устройств системы отопления", "MQTT")
Rel(mqtt_subscriber, device_gateway, "Подписка на топики подтверждения исполнения команд устройств системы отопления", "MQTT")
Rel(rabbit_publisher, message_broker, "Публикация сообщений", "AMQP")
Rel(rabbit_subscriber, message_broker, "Прослушивание сообщений", "AMQP")

@enduml
```