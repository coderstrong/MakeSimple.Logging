# MakeSimple.Logging

Make Simple .NET logging config and tracing in microservices architecture

[![Build status](https://ci.appveyor.com/api/projects/status/eau3dun5q5d7wwi9/branch/main?svg=true)](https://ci.appveyor.com/project/coderstrong/makesimple-logging/branch/main) [![NuGet Version](https://img.shields.io/nuget/v/MakeSimple.Logging.svg?style=flat)](https://www.nuget.org/packages/MakeSimple.Logging/) [![NuGet Downloads](https://img.shields.io/nuget/dt/MakeSimple.Logging.svg)](https://www.nuget.org/packages/MakeSimple.Logging/) 

# Get start

Install from Nuget
```
$ dotnet add package MakeSimple.Logging
```
Config Program.cs
```csharp
public static void Main(string[] args)
{
    Activity.DefaultIdFormat = ActivityIdFormat.W3C;
    ....
}
```
Config Startup.cs
```csharp
public void ConfigureServices(IServiceCollection services)
{
  ...
  services.AddMakeSimpleLoging(new LoggingOption()
  {
    IsOffLogSystem = bool.Parse(Environment.GetEnvironmentVariable("ISOFF_LOG_SYSTEM")),
    MinimumLevel = (LoggerLevel)int.Parse(Environment.GetEnvironmentVariable("LOG_LEVEL"))   
  });
  ...
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
  ...
  app.AddMakeSimpleLoging();
  ...
}
```
# Add log custom property

```csharp
LogContext.PushProperty("CustomerInfo", "Example");
```

# Log properties

| Property   |      Type      |  Description |
|----------|:-------------:|------:|
| Timestamp |  Datetime | [ISO8601](https://en.wikipedia.org/wiki/ISO_8601) |
| Level |    String   |    |
| Exception | String |     |
| Message | String |     |
| SpanId | String Id | [Trace context](https://www.w3.org/TR/trace-context/)  |
| TraceId | String Id | [Trace context](https://www.w3.org/TR/trace-context/) |
| ParentId | String Id | [Trace context](https://www.w3.org/TR/trace-context/) |
| Method | String |     |
| QueryString | String |     |
| Payload | String |     |
| RequestedOn | Datetime |     |
| Response | String |     |
| ResponseCode | String |     |
| RespondedOn | Datetime |     |
| ... | ... |     |

# Log level

| Level   |      Value      |  Description |
|----------|:-------------:|------:|
| Information |  0 |  |
| Verbose |  1 |  |
| Debug |  2 |  |
| Error |  3 |  |
| Fatal |  4 |  |
| Warning |  5 |  |
