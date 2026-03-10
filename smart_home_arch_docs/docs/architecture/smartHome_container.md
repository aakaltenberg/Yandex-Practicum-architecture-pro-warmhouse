```puml
@startuml
title SmartHome Container Diagram (Microservices)
!includeurl https://raw.githubusercontent.com/RicardoNiepel/C4-PlantUML/master/C4_Container.puml

Person(user, "Пользователь", "Управляет устройствами")
Person(admin, "Администратор", "Управляет настройками устройств")

Container_Boundary(smarthome, "SmartHome System") {
    Container(api_gateway, "API Gateway", "Kong", "Маршрутизация, аутентификация, переадресация")
    
    Container(house_service, "House Service", ".NET", "Управление домами и пользователями")
    Container(heating_service, "Heating Service", ".NET", "Управление отоплением")
    Container(lighting_service, "Lighting Service", ".NET", "Управление освещением (лампы, выключатели)")
    Container(camera_service, "Camera Service", ".NET", "Управление камерами")
    Container(gate_service, "Gate Service", ".NET", "Управление воротами")
    Container(scenario_service, "Scenario Service", ".NET", "Создание и выполнение сценариев автоматизации")
    Container(telemetry_service, "Telemetry Service", ".NET", "Сбор и хранение телеметрии")
    
    Container(house_db, "House DB", "PostgreSQL", "Данные о домах и пользователях")
    Container(heating_db, "Heating DB", "PostgreSQL", "Данные об устройствах отопления")
    Container(lighting_db, "Lighting DB", "PostgreSQL", "Данные об устройствах освещения")
    Container(camera_db, "Camera DB", "PostgreSQL", "Данные о камерах")
    Container(gate_db, "Gate DB", "PostgreSQL", "Данные о воротах")
    Container(scenario_db, "Scenario DB", "PostgreSQL", "Сценарии, правила, действия")
    Container(telemetry_db, "Telemetry DB", "TimescaleDB", "Данные телеметрии")
    
    Container(message_broker, "Message Broker", "RabbitMQ", "События об изменении состояния устройств, команды")
    
    Container(device_gateway, "Device Gateway", "MQTT Broker", "Подключение физических устройств по стандартным протоколам")
}

System_Ext(physical_devices, "Физические устройства", "MQTT", "Датчики отопления, лампы, камеры, ворота")

Rel(user, api_gateway, "Управление, просмотр", "HTTP")
Rel(admin, api_gateway, "Добавление новых типов устройств", "HTTP")

Rel(api_gateway, house_service, "Маршрутизация", "HTTP")
Rel(api_gateway, heating_service, "Маршрутизация", "HTTP")
Rel(api_gateway, lighting_service, "Маршрутизация", "HTTP")
Rel(api_gateway, camera_service, "Маршрутизация", "HTTP")
Rel(api_gateway, gate_service, "Маршрутизация", "HTTP")
Rel(api_gateway, scenario_service, "Маршрутизация", "HTTP")
Rel(api_gateway, telemetry_service, "Маршрутизация", "HTTP")

Rel(house_service, house_db, "CRUD", "SQL")
Rel(heating_service, heating_db, "CRUD", "SQL")
Rel(lighting_service, lighting_db, "CRUD", "SQL")
Rel(camera_service, camera_db, "CRUD", "SQL")
Rel(gate_service, gate_db, "CRUD", "SQL")
Rel(scenario_service, scenario_db, "CRUD", "SQL")
Rel(telemetry_service, telemetry_db, "CRUD", "SQL")

Rel(heating_service, message_broker, "Публикация событий/команд", "AMQP")
Rel(lighting_service, message_broker, "Публикация событий/команд", "AMQP")
Rel(camera_service, message_broker, "Публикация событий/команд", "AMQP")
Rel(gate_service, message_broker, "Публикация событий/команд", "AMQP")
Rel(telemetry_service, message_broker, "Подписка на телеметрию", "AMQP")
Rel(scenario_service, message_broker, "Подписка на события", "AMQP")

Rel(heating_service, device_gateway, "Отправка команд устройствам отопления", "MQTT")
Rel(lighting_service, device_gateway, "Отправка команд устройствам освещения", "MQTT")
Rel(camera_service, device_gateway, "Отправка команд камерам", "MQTT")
Rel(gate_service, device_gateway, "Отправка команд воротам", "MQTT")
Rel(telemetry_service, device_gateway, "Получение потоковых данных", "MQTT")

Rel(device_gateway, physical_devices, "Получение команд, публикация событий об изменении состояния (телеметрия)", "MQTT")

@enduml
```