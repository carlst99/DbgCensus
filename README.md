![DbgCensus Icon](Assets/Icon_128.png)

# DbgCensus

DbgCensus is a C# wrapper for [Daybreak Game Company's Census API](https://census.daybreakgames.com), and other unofficial Census-compatible implementations.

Want to chat with developers of all things Census related? Come say hello in the Planetside Community Developers Discord!\
![Discord](https://img.shields.io/discord/1019343142471880775?color=blue&label=Planetside%20Community%20Developers&logo=discord&logoColor=%2302B4FF)

**This package is not affiliated with Daybreak Games Company in any way.**

[![Nuget | DbgCensus.Core](https://img.shields.io/nuget/v/DbgCensus.Core?label=DbgCensus.Core)](https://www.nuget.org/packages/DbgCensus.Core) - Core data types and utilities.\
[![Nuget | DbgCensus.Rest](https://img.shields.io/nuget/v/DbgCensus.Rest?label=DbgCensus.Rest)](https://www.nuget.org/packages/DbgCensus.Rest) - Services for interacting with the query endpoints.\
[![Nuget | DbgCensus.EventStream](https://img.shields.io/nuget/v/DbgCensus.EventStream?label=DbgCensus.EventStream)](https://www.nuget.org/packages/DbgCensus.EventStream) - Base services for interacting with the event streaming API.\
[![Nuget | DbgCensus.EventStream.EventHandlers](https://img.shields.io/nuget/v/DbgCensus.EventStream.EventHandlers?label=DbgCensus.EventStream.EventHandlers)](https://www.nuget.org/packages/DbgCensus.EventStream.EventHandlers) - An abstraction of `DbgCensus.EventStream` providing an
asynchronous and decoupled event handling model.

***

## Features

- Fluent query building API.
- Event dispatch/handling system and built-in event stream types.
- Fully asynchronous.
- Built around the `Microsoft.Extensions` framework.
- Native AOT Compatibility (Core and Rest only).
- Targeting .NET 6.0, .NET8.0 and .NET 9.0.

***

## Documentation

Head over to the [documentation website](https://carlst99.github.io/DbgCensus/) to get started with DbgCensus.

### Samples

Check out the [samples](Samples) to get up and running quickly with DbgCensus. These demonstrate typical usage of the
libraries within the Generic Host framework.

The `EventStreamSample` utilises DbgCensus' event handling framework. If you'd prefer to use another method of
dispatching and handling events, you'll need to extend the `BaseEventStreamClient` instead, and register it yourself
using the `AddCensusEventStreamServices` extension method.

### Event Streaming

I haven't gotten around to documenting this yet! Please check out the [Event Stream Sample](Samples/EventStreamSample) in the meantime
:slightly_smiling_face:.
