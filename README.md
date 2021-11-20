<img title="DbgCensus Icon" alt="DbgCensus Icon" src="https://github.com/carlst99/DbgCensus/blob/main/Assets/Icon_128.png?raw=true" align="left"></img>

# DbgCensus

DbgCensus is a C# wrapper for [Daybreak Game Company's Census API](https://census.daybreakgames.com). It was built with PlanetSide 2's endpoints in mind, but should work across all namespaces.

**This package is unofficial and is not affiliated with Daybreak Games Company in any way.**

[![Nuget | DbgCensus.Core](https://img.shields.io/nuget/v/DbgCensus.Core?label=DbgCensus.Core)](https://www.nuget.org/packages/DbgCensus.Core) - Core data types and utilities.\
[![Nuget | DbgCensus.Rest](https://img.shields.io/nuget/v/DbgCensus.Rest?label=DbgCensus.Rest)](https://www.nuget.org/packages/DbgCensus.Rest) - Services for interacting with the query endpoints.\
[![Nuget | DbgCensus.EventStream](https://img.shields.io/nuget/v/DbgCensus.EventStream?label=DbgCensus.EventStream)](https://www.nuget.org/packages/DbgCensus.EventStream) - Base services for interacting with the event streaming API.\
[![Nuget | DbgCensus.EventStream.EventHandlers](https://img.shields.io/nuget/v/DbgCensus.EventStream.EventHandlers?label=DbgCensus.EventStream.EventHandlers)](https://www.nuget.org/packages/DbgCensus.EventStream.EventHandlers) - An abstraction of DbgCensus.EventStream providing an asynchronous and decoupled event handling model.

***

# Features

- Fluent query building API.
- Event dispatch/handling model and built-in event stream types.
- Fully asynchronous.
- Built around the `Microsoft.Extensions` framework.
- Targeting .NET 6.0.


***

### Table of Contents

- [Getting Started](#getting-started)
- [Core Components](#core-components)
- [Performing Queries](#performing-queries)
- [Event Streaming](#event-streaming)

# Getting Started

Before you do anything, you should consider getting a custom *Census Service ID*. The process is free and it generally only takes a few hours to hear back about your registration, [which you can do here](https://census.daybreakgames.com/#devSignup).

Note that you can use the `example` service ID, however you will be rate-limited to 10 requests per minute, per client IP address.

You will also need to have a good understanding of how the Census API works. I highly recommend making your way through these excellent official/community resources:

- [The official Census API documentation.](https://census.daybreakgames.com)
- [Leonhard's Census API Primer.](https://github.com/leonhard-s/auraxium/wiki/Census-API-Primer)
- [The community API issue tracker/info repository.](https://github.com/cooltrain7/Planetside-2-API-Tracker)
- [Leonhard's unofficial docs for PlanetSide 2 endpoints.](https://ps2-api-docs.readthedocs.io/en/latest/openapi.html)

### Examples

Check out the [samples](Samples) to get up and running quickly with DbgCensus. These demonstrate typical usage of the libraries within the Generic Host framework.

The `EventStreamSample` utilises DbgCensus' event handling framework. If you'd prefer to use another method of dispatching and handling events, you'll need to extend the `BaseEventStreamClient` instead, and register it yourself using the `AddCensusEventStreamServices` extension method.

# Core Components

The *Core* library contains common types and extensions. Of these, it is likely you will find the Census types useful (`DbgCensus.Core.Objects`). There are:

- Enumerations of the faction, world, zone and metagame (definition and state) IDs that Census uses.
- A `ZoneID` record that represents Census' special zone ID format - [see here](https://github.com/cooltrain7/Planetside-2-API-Tracker/wiki/Zone-ID-Tutorial) for more info. JSON converters are registered by default for this type, so you can use it anywhere that you would normally use an integer zone ID in your models.

There are also converters, extensions and naming policies for `System.Text.Json` that you may find useful should you decide to perform your own JSON deserialisation.

# Performing Queries

Check out the [REST Sample](Samples/RestSample) as you read through this.

## Setup

Start off by creating a new project. I would highly recommend using a template that implements the [Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host), such as a *Worker Service* or an *ASP.NET Core* project.

Then, install the REST package:

```powershell
# Visual Studio Package Manager
Install-Package DbgCensus.Rest
# dotnet console
dotnet add package DbgCensus.Rest
```

Register the required services to the container with the `AddCensusRestServices` extension method. If you aren't using the `Microsoft.Extensions` framework, take a look at [this file](DbgCensus.Rest/Extensions/IServiceCollectionExtensions.cs) to see how the required services are setup.

You will also need to configure an instance of the `CensusQueryOptions` class to ensure that your service ID is utilised. Typically, you'd register your options from a configuration source (usually a section of `appsettings.json`) to retrieve any secrets that shouldn't be stored with the code, and then follow up with any additional configuration.

```csharp
using DbgCensus.Rest.Extensions;

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((configuration, services) =>
        {
            services.AddCensusRestServices();

            services.Configure<CensusQueryOptions>
            (
                hostContext.Configuration.GetSection(nameof(CensusQueryOptions))
            );
        
            // AND/OR
            services.Configure<CensusQueryOptions>
            (
                o => o.DeserializationOptions = new JsonSerializerOptions(...)
            );
        });
```

## Making a Query

Grab an `IQueryService` instance. This is a wrapper around the registered `IQueryBuilderFactory` and `ICensusRestClient` objects, which you can use individually if you need slightly more control over your queries.

Then simply proceed to define your query, and call `GetAsync` to retrieve it. Note that the result can be null if you've made a query for a singular item that doesn't exist, or Census simply didn't return any data.

```csharp
public class RestExample
{
    private readonly IQueryService _queryService;

    public RestExample(IQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Character?> GetCharacter(string name)
    {
        IQueryBuilder query = _queryService.CreateQuery()
            .OnCollection("character")
            .Where("name.first_lower", SearchModifier.Equals, name.ToLower());

        try
        {
            Character? character = await _queryService.GetAsync<Character>(query, ct).ConfigureAwait(false);
            if (character is null)
                _logger.LogInformation("That character does not exist.");

            return character;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve character.");
            return null;
        }
    }
}
```

:warning: An important distinction to notice when defining queries is that filtering a property is split into two methods. If you'd like to filter a property by a singular value, use the `Where` method:

```csharp
query.Where("property", SearchModifier.LessThan, "value")
```

If you'd like to filter a property by multiple values, use the `WhereAll` method:

```csharp
int[] values = new[] { 1, 2, 3 };
query.WhereAll("property", SearchModifier.Equals, values);
```

### Retrieving collection counts

Using the `count` verb to retrieve the number of elements in a collection can be done in two ways. Firstly, by using the shortcut on the `ICensusRestClient`, or secondly by making a query as per usual and defining the query type. Just remember to de-serialise to a number type!

```csharp
ICensusRestClient client = ...;
ulong count = await client.CountAsync("character", ct);

// OR

IQueryBuilder query = ...;
query.OnCollection("character")
    .OfQueryType(QueryType.Count);

ulong count = await _queryService.GetAsync<ulong>(query, ct).ConfigureAwait(false);
```

### Retrieving distinct field values

There is a shortcut for the `c:distinct` parameter on the `ICensusRestClient` interface which directly returns the list of unique values, preventing you from having to define a custom model.

The generic type argument of the method should match the model of the field you are querying.

```csharp
ICensusRestClient client = ...;
IReadOnlyList<int>? distinctValues = await client.DistinctAsync<int>("item", "max_stack_size" ct);
```

# Event Streaming

I haven't gotten around to documenting this yet! Please check out the [Event Stream Sample](Samples/EventStreamSample) in the meantime :slightly_smiling_face:.