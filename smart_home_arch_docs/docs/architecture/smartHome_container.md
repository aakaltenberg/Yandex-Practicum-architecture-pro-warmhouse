```puml
@startuml
title SmartHome Container Diagram

top to bottom direction

!includeurl https://raw.githubusercontent.com/RicardoNiepel/C4-PlantUML/master/C4_Component.puml

Person(user, "User", "A user of the smart home system")
Person(admin, "Administrator", "An administrator managing the system")
System(SmartHomeSystem, "SmartHome System", "System managing houses, devices, devices state, scenarios, getting telemetry data")

Container_Boundary(SmartHomeSystem, "SmartHome System") {
  Container(HouseService, "House Service", ".NET, ASP.NET Core", "Service managing houses")
  Container(DeviceService, "Device Service", ".NET, ASP.NET Core", "Service managing devices")
  Container(ScenarioService, "Scenario Service", ".NET, ASP.NET Core", "Service managing scenarios")
  Container(TelemetryService, "Telemetry Service", ".NET, ASP.NET Core", "Getting telemetry data")

  Container(HouseDatabase, "House Database", "PostgreSQL", "Stores house data")
  Container(DeviceDatabase, "Device Database", "PostgreSQL", "Stores devices, device state data")
  Container(ScenarioDatabase, "Scenario Database", "PostgreSQL", "Stores scenario data")
  Container(TelemetryDatabase, "Telemetry Database", "PostgreSQL", "Stores telemetry data")
  
  Container(MessageBroker, "Message Broker", "RabbitMQ", "Async integration betw DeviceService and TelemetryService")

  Container(ApiGateway, "ApiGateway", "Kong", "Authorization, authentification, routing")
}

System_Ext(TemperatureService, "Third-Party temperature device API", "External API for temperature devices integration", "Uses REST API, JSON data format")
System_Ext(LightService, "Third-Party light device API", "External API for light devices integration", "Uses REST API, JSON data format")
System_Ext(AutoGatesService, "Third-Party auto gates device API", "External API for auto gates devices integration", "Uses REST API, JSON data format")
System_Ext(CameraService, "Third-Party camera device API", "External API for camera devices integration", "Uses REST API, JSON data format")

Rel(user, ApiGateway, "Getting telemetry, setting target value, on/off device, create scenarios") 
Rel(admin, ApiGateway, "Adding new house, devices, update info about devices, delete devices") 

Rel(ApiGateway,HouseService,"redirection")
Rel(ApiGateway,DeviceService,"redirection")
Rel(ApiGateway,ScenarioService,"redirection")
Rel(ApiGateway,TelemetryService,"redirection")

Rel(TelemetryService,TemperatureService,"getting sync telemetry realtime data for temperature devices")
Rel(TelemetryService,LightService,"getting sync telemetry realtime data for light devices")
Rel(TelemetryService,AutoGatesService,"getting sync telemetry realtime data for auto gates devices")
Rel(TelemetryService,CameraService,"getting sync telemetry realtime data for camera devices")

Rel(DeviceService,TemperatureService,"setting target state/value for temperature devices")
Rel(DeviceService,LightService,"setting target state/value for light devices")
Rel(DeviceService,AutoGatesService,"setting target state/value for auto gates devices")
Rel(DeviceService,CameraService,"setting target state/value for camera devices")

Rel(HouseService,HouseDatabase,"CRUD house data")
Rel(DeviceService,DeviceDatabase,"CRUD devices (temperature, light, autoGates, camera) data")
Rel(ScenarioService,ScenarioDatabase,"CRUD scenarios (scenario, actions, conditions) data")
Rel(TelemetryService,TelemetryDatabase,"CRUD telemetry (temperature, light, autoGates, camera) data")

Rel(DeviceService, MessageBroker, "publishing events about changes in devices (temperature, light, autoGates, camera) state/value")
Rel(TelemetryService, MessageBroker, "subscribe on events about changes in devices (temperature, light, autoGates, camera) state/value")

@enduml
```