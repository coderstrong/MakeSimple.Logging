# MakeSimple.Logging
Make Simple .NET logging config and tracing

[![Build status](https://ci.appveyor.com/api/projects/status/eau3dun5q5d7wwi9/branch/main?svg=true)](https://ci.appveyor.com/project/coderstrong/makesimple-logging/branch/main) [![NuGet Version](https://img.shields.io/nuget/v/MakeSimple.Logging.svg?style=flat)](https://www.nuget.org/packages/MakeSimple.Logging/) [![NuGet Downloads](https://img.shields.io/nuget/dt/MakeSimple.Logging.svg)](https://www.nuget.org/packages/MakeSimple.Logging/) 

# Get start

Install from Nuget
$ dotnet add package MakeSimple.Logging

Config Startup.cs
```
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

