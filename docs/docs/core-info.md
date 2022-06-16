# Core Information

## Packages

[![Nuget | DbgCensus.Core](https://img.shields.io/nuget/v/DbgCensus.Core?label=DbgCensus.Core)](https://www.nuget.org/packages/DbgCensus.Core)
Core data types and utilities.

[![Nuget | DbgCensus.Rest](https://img.shields.io/nuget/v/DbgCensus.Rest?label=DbgCensus.Rest)](https://www.nuget.org/packages/DbgCensus.Rest)
Services for interacting with the query endpoints.

[![Nuget | DbgCensus.EventStream](https://img.shields.io/nuget/v/DbgCensus.EventStream?label=DbgCensus.EventStream)](https://www.nuget.org/packages/DbgCensus.EventStream)
Base services for interacting with the event streaming API.

[![Nuget | DbgCensus.EventStream.EventHandlers](https://img.shields.io/nuget/v/DbgCensus.EventStream.EventHandlers?label=DbgCensus.EventStream.EventHandlers)](https://www.nuget.org/packages/DbgCensus.EventStream.EventHandlers)
An abstraction of `DbgCensus.EventStream` providing an asynchronous and decoupled event handling model.

## Getting Started with Census

Before you do anything, you should consider getting a custom *Census Service ID*. The process is free and it generally only takes a few hours to hear back about your registration, [which you can do here](https://census.daybreakgames.com/#devSignup).

Note that you can use the `example` service ID, however you will be rate-limited to 10 requests per minute, per client IP address.

You will also need to have a good understanding of how the Census API works. I highly recommend making your way through these excellent official/community resources:

- [The official Census API documentation.](https://census.daybreakgames.com)
- [Leonhard's Census API Primer.](https://github.com/leonhard-s/auraxium/wiki/Census-API-Primer)
- [The community API issue tracker/info repository.](https://github.com/cooltrain7/Planetside-2-API-Tracker)
- [Leonhard's unofficial docs for PlanetSide 2 endpoints.](https://ps2-api-docs.readthedocs.io/en/latest/openapi.html)

## Core Library Components

The *Core* library contains common types and extensions. Of these, it is likely you will find the Census types useful (`DbgCensus.Core.Objects`). There are:

- Enumerations of the faction, world, zone and metagame (definition and state) IDs that Census uses.
- An `Optional<T>` structure w/ relevant JSON converters that you can use to represent optional Census fields in your models.
- A `ZoneID` struct that represents Census' special zone ID format - [see here](https://github.com/cooltrain7/Planetside-2-API-Tracker/wiki/Zone-ID-Tutorial) for more info. JSON converters are registered by default for this type, so you can use it anywhere that you would normally use an integer zone ID in your models.
- A `GlobalizedString` structure that represents Census' globalized string fields:
    ```json
    "name": {
        "de": "Spawn-Leuchte",
        "en": "Spawn Beacon",
        ...
    }
    ```

There are also converters, extensions and naming policies for `System.Text.Json` that you may find useful should you decide to perform your own JSON deserialization.
