<img title="DbgCensus Icon" alt="DbgCensus Icon" src="https://github.com/carlst99/DbgCensus/blob/main/Assets/Icon_128.png?raw=true" align="left" />

# DbgCensus

DbgCensus is a low-level c# wrapper for [Daybreak Game Company's Census API](https://census.daybreakgames.com). It was built with PlanetSide 2's endpoints in mind, but should work across all namespaces.

**This package is unofficial and is not affiliated with Daybreak Games Company in any way.**

[![Nuget | DbgCensus.Core](https://img.shields.io/nuget/v/DbgCensus.Core?label=DbgCensus.Core)](https://www.nuget.org/packages/DbgCensus.Core)
[![Nuget | DbgCensus.Rest](https://img.shields.io/nuget/v/DbgCensus.Rest?label=DbgCensus.Rest)](https://www.nuget.org/packages/DbgCensus.Rest)

***

- Fluent query building API with full coverage of the Census query interface.
- Fully asynchronous.
- Highly extendable - core components can be extended, replaced and used individually.
- Built around the `Microsoft.Extensions` framework.
- Compiled for .NET 5.0

> :warning: DbgCensus is currently in a pre-release state. This means that:
> - Support for the Event Streaming API has not yet been implemented.
> - The code is largely untested.
> - Documentation is light on the ground.

***

## Getting Started

Before you do anything, you should consider getting a custom *Service ID*. The process is free and it generally only takes a few hours to hear back about your registration, which you can do [here](https://census.daybreakgames.com/#devSignup).

Note that you can also use the `example` service ID, however you will be rate-limited to 10 requests per minute, per client IP address.

You will also need to have a good understanding of how the Census API works. I highly recommend making your way through these excellent official/community docs:
- [The official Census API documentation.](https://census.daybreakgames.com)
- [Leonhard's Census API Primer.](https://github.com/leonhard-s/auraxium/wiki/Census-API-Primer)
- [The community API issue tracker/info repository](https://github.com/cooltrain7/Planetside-2-API-Tracker)
- [Leonhard's unofficial docs for PlanetSide 2 endpoints.](https://ps2-api-docs.readthedocs.io/en/latest/openapi.html)

### Interacting with Census Query Endpoints

> :warning: This section of documentation is a work in progress

Start off by installing the REST package. As it is currently a pre-release build, you will need to check the 'Include prerelease' option in Visual Studio, or add the `-IncludePrerelease` flag to your installation command.

```
Install-Package DbgCensus.Rest -IncludePrerelease
```

If your project makes use of the `Microsoft.Extensions` framework, you can easily register the requisite services to the container:

```csharp
using DbgCensus.Rest.Extensions;

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((c, services) =>
        {
            services.AddCensusRestServices();
        });
```

If you aren't using the `Microsoft.Extensions` framework, take a look at the [this file](DbgCensus.Rest/Extensions/IServiceCollectionExtensions.cs) to see how the required services are setup.

#### Building Queries

Use an `IQueryBuilder` instance. You can instantiate these as needed, or preferably get them from an `IQueryBuilderFactory` that you've injected, to take advantage of the options pattern.

#### Performing Queries

Use a singleton `CensusRestClient` object.

#### Customising Options

Utilise the `CensusQueryOptions` object. You can register this with the options pattern (`Microsoft.Extensions.Configuration`).

***

## Roadmap

- Support for the event stream.
- Proper documentation.
- Polly implementation for the REST interface.
- Complete unit testing coverage.