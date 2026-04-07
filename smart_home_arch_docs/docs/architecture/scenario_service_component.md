```puml
@startuml title Scenario Service - Component Diagram

!includeurl https://raw.githubusercontent.com/RicardoNiepel/C4-PlantUML/master/C4_Component.puml

Container_Boundary(scenario_service, "Scenario Service", ".NET") {
    Component(scenario_mgmt_controller, "Scenario Management Controller", "ASP.NET Core", "REST API для создания, редактирования, удаления, получения сценариев")
    Component(scenario_exec_controller, "Scenario Execution Controller", "ASP.NET Core", "REST API для ручного запуска/остановки сценариев")

    Component(business_logic, "Scenario Business Logic", ".NET", "Управление сценариями: чтение, сохранение, валидация")
    Component(execution_engine, "Execution Engine", ".NET", "Обрабатывает входящие события, проверяет условия, запускает действия по сценариям; также обрабатывает ручной запуск")
    Component(repository, "Repository", ".NET / EF Core", "Доступ к базе данных Scenario DB")

    Component(rabbit_subscriber, "RabbitMQ Subscriber", ".NET / RabbitMQ.Client", "Подписка на события-триггеры от других микросервисов")
    Component(rabbit_publisher, "RabbitMQ Publisher", ".NET / RabbitMQ.Client", "Публикация команд действий в сервисы устройств")
}

System_Ext(scenario_db, "Scenario DB", "PostgreSQL", "Хранит сценарии, правила, условия, действия")
System_Ext(message_broker, "Message Broker", "RabbitMQ", "Обмен событиями между микросервисами")

Rel(scenario_mgmt_controller, business_logic, "Вызывает методы управления сценариями", "")
Rel(scenario_exec_controller, execution_engine, "Запускает сценарий по запросу пользователя", "")
Rel(business_logic, repository, "Читает, пишет, обновляет сценарии", "")
Rel(rabbit_subscriber, execution_engine, "Передаёт входящие события-триггеры", "")
Rel(execution_engine, rabbit_publisher, "Отправляет команды действий", "")
Rel(execution_engine, repository, "Получение, отправка, агрегация сведений о сценариях", "")

Rel(repository, scenario_db, "CRUD", "SQL")
Rel(rabbit_subscriber, message_broker, "Подписка на события", "AMQP")
Rel(rabbit_publisher, message_broker, "Публикация команд", "AMQP")

@enduml
```