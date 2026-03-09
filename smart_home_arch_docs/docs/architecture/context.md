```puml
@startuml title FitLife Context Diagram

top to bottom direction

!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Context.puml

Person(user, "User", "A user of the SmartHome system") 
Person(admin, "Administrator", "An administrator managing the system") 
System(SmartHomeSystem, "SmartHome System", "System managing temperature, devices (sensors)")

System_Ext(TemperatureService, "Third-Party device API", "External API for temperature devices integration", "Uses REST API, JSON data format") 
Rel(user, SmartHomeSystem, "Uses the system (getting actual temperature, setting target temperature, on/off heating system)") 
Rel(admin, SmartHomeSystem, "Manages the system (adding new devices, update info about devices, delete devices from system)") 
Rel(SmartHomeSystem, TemperatureService, "Fetches actual temperature data, setting target temperature value, on/off heating system (setting status)") 

@enduml
```