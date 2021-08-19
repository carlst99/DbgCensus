<img title="DbgCensus Icon" alt="DbgCensus Icon" src="https://github.com/carlst99/DbgCensus/blob/main/Assets/Icon_128.png?raw=true" align="left" />

# DbgCensus

DbgCensus is a C# wrapper for [Daybreak Game Company's Census API](https://census.daybreakgames.com). It was built with PlanetSide 2's endpoints in mind, but should work across all namespaces.

**This package is unofficial and is not affiliated with Daybreak Games Company in any way.**

[![Nuget | DbgCensus.Core](https://img.shields.io/nuget/v/DbgCensus.Core?label=DbgCensus.Core)](https://www.nuget.org/packages/DbgCensus.Core)
[![Nuget | DbgCensus.Rest](https://img.shields.io/nuget/v/DbgCensus.Rest?label=DbgCensus.Rest)](https://www.nuget.org/packages/DbgCensus.Rest)
[![Nuget | DbgCensus.EventStream](https://img.shields.io/nuget/v/DbgCensus.EventStream?label=DbgCensus.EventStream)](https://www.nuget.org/packages/DbgCensus.EventStream)
[![Nuget | DbgCensus.EventStream.EventHandlers](https://img.shields.io/nuget/v/DbgCensus.EventStream?label=DbgCensus.EventStream.EventHandlers)](https://www.nuget.org/packages/DbgCensus.EventStream.EventHandlers)

***

- Fluent query building API with full coverage of the Census query interface.
- Fully asynchronous.
- Highly extendable - core components can be extended, replaced and used individually.
- Built around the `Microsoft.Extensions` framework.
- Compiled for .NET 5.0.

> :warning: DbgCensus is currently in a pre-release state. This means that:
>
> - The code has been 'tested' by my own workloads, but not thoroughly hand or unit tested.
> - The API is liable to change, although it is beginning to reach a more mature point.
> - Documentation is light on the ground, although the code is fully XML documented.

***

## Getting Started

Before you do anything, you should consider getting a custom *Census Service ID*. The process is free and it generally only takes a few hours to hear back about your registration, which you can do [here](https://census.daybreakgames.com/#devSignup).

Note that you can use the `example` service ID, however you will be rate-limited to 10 requests per minute, per client IP address.

You will also need to have a good understanding of how the Census API works. I highly recommend making your way through these excellent official/community docs:

- [The official Census API documentation.](https://census.daybreakgames.com)
- [Leonhard's Census API Primer.](https://github.com/leonhard-s/auraxium/wiki/Census-API-Primer)
- [The community API issue tracker/info repository](https://github.com/cooltrain7/Planetside-2-API-Tracker)
- [Leonhard's unofficial docs for PlanetSide 2 endpoints.](https://ps2-api-docs.readthedocs.io/en/latest/openapi.html)
- [#api-dev channel in the PlanetSide 2 Community Discord server for general API questions.](https://discord.me/planetside)

## Examples

Check out the [samples](Samples) to get up and running quickly with DbgCensus. These demonstrate typical usage of the libraries within the Generic Host framework.

The `EventStreamSample` utilises DbgCensus' event handling framework. If you'd prefer to use another method of dispatching and handling events, you'll need to extend the `BaseEventStreamClient` instead, and register it yourself using the `AddCensusEventStreamServices` extension method.

## Interacting with Census Query Endpoints

Check out [REST Sample](Samples/RestSample) as you read through this.

Start off by creating a new project. I would highly recommend using a template that implements the [Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host), such as a *Worker Service* or an *ASP.NET Core* project.

Then, install the REST package, either by using the Visual Studio Package Manager:

```powershell
# Visual Studio Package Manager
Install-Package DbgCensus.Rest
# dotnet console
dotnet add package DbgCensus.Rest
```

If your project integrates with the `Microsoft.Extensions` framework, you can easily register the required services to the container with the `AddCensusRestServices` extension method:

```csharp
using DbgCensus.Rest.Extensions;

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((c, services) =>
        {
            services.AddCensusRestServices();
        });
```

If you aren't using the `Microsoft.Extensions` framework, take a look at [this file](DbgCensus.Rest/Extensions/IServiceCollectionExtensions.cs) to see how the required services are setup.

### Customising Query Options

You will need configure an instance of the `CensusQueryOptions` class to ensure that your service ID is utilised. I like to register my options from a configuration source (usually a section of `appsettings.json`) to retrieve any secrets that shouldn't be stored with the code, and then follow up with any additional configuration.

```csharp
.ConfigureServices((hostContext, services) =>
    {
        services.Configure<CensusQueryOptions>(hostContext.Configuration.GetSection(nameof(CensusQueryOptions)));
        // AND/OR
        services.Configure<CensusQueryOptions>(o => o.DeserializationOptions = new JsonSerializerOptions(...));
    });
```

### Performing Queries

Grab an `IQueryService` instance. This is a wrapper around the registered `IQueryBuilderFactory` and `ICensusRestClient` objects, which you can use if you need slightly more control over your queries.

```csharp
IQueryBuilder query = _queryService.CreateQuery()
    .OnCollection("character")
    .Where("name.first_lower", SearchModifier.Equals, "falconeye36");

try
{
    Character? character = await _queryService.GetAsync<Character>(query, ct).ConfigureAwait(false);
    if (character is null)
    {
        _logger.LogInformation("That character does not exist.");
        return;
    }
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to retrieve character.");
}
```