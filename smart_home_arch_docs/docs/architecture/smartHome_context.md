```puml
@startuml title SmartHome Context Diagram

top to bottom direction

!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Context.puml

Person(user, "Пользователь", "Управляет устройствами")
Person(admin, "Администратор", "Управляет настройками устройств")
System(SmartHomeSystem, "SmartHome System", "Система управления системой отопления, устройствами (датчиками)")

System_Ext(TemperatureService, "Third-Party device API", "Внешнее API для интеграции с устройствами системы отопления", "REST API, JSON") 
Rel(user, SmartHomeSystem, "Использует систему (получение актуальной информации о температуре; установка целевой температуры; включение/выключение устройств)") 
Rel(admin, SmartHomeSystem, "Управление системой отопления (добавление новых устройств, обновление информации об устройствах, удаление устройств из системы)") 
Rel(SmartHomeSystem, TemperatureService, "Получение актуальных данных о температуре, установление целевой температуры, включение/выключение устройств") 

@enduml
```