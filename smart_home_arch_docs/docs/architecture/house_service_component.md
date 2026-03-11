```puml
@startuml
title House Service - Component Diagram

!includeurl https://raw.githubusercontent.com/RicardoNiepel/C4-PlantUML/master/C4_Component.puml

System_Ext(house_db, "House DB", "PostgreSQL", "Данные о домах, пользователях, правах")
System_Ext(message_broker, "Message Broker", "RabbitMQ", "Обмен событиями между микросервисами")

Container_Boundary(house_service, "House Service", ".NET") {
    Component(house_controller, "House Controller", "ASP.NET Core", "REST API для управления домами")
    Component(user_controller, "User Controller", "ASP.NET Core", "REST API для управления пользователями, ролями")

    Component(business_logic, "Business Logic Layer", ".NET", "Реализует операции с домами и пользователями")
    Component(repository, "Repository", ".NET / EF Core", "Доступ к базе данных House DB")
    
    Component(rabbit_publisher, "RabbitMQ Publisher", ".NET / RabbitMQ.Client", "Публикует события об изменении домов/пользователей")
}

Rel(house_controller, business_logic, "Вызывает методы для работы с домами", "")
Rel(user_controller, business_logic, "Вызывает методы для работы с пользователями", "")
Rel(business_logic, repository, "Читает/пишет/обновляет данные", "")
Rel(business_logic, rabbit_publisher, "Публикует события (после успешного изменения, добавления, удаления домов/пользователей)", "")

Rel(repository, house_db, "CRUD", "SQL")
Rel(rabbit_publisher, message_broker, "Публикация событий", "AMQP")

@enduml
```