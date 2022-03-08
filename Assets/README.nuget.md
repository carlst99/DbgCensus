# DbgCensus

DbgCensus is a C# wrapper for [Daybreak Game Company's Census API](https://census.daybreakgames.com). It was built with PlanetSide 2's endpoints in mind, but should work across all namespaces.

**This package is unofficial and is not affiliated with Daybreak Games Company in any way.**

[![Nuget | DbgCensus.Core](https://img.shields.io/nuget/v/DbgCensus.Core?label=DbgCensus.Core)](https://www.nuget.org/packages/DbgCensus.Core) - Core data types and utilities.\
[![Nuget | DbgCensus.Rest](https://img.shields.io/nuget/v/DbgCensus.Rest?label=DbgCensus.Rest)](https://www.nuget.org/packages/DbgCensus.Rest) - Services for interacting with the query endpoints.\
[![Nuget | DbgCensus.EventStream](https://img.shields.io/nuget/v/DbgCensus.EventStream?label=DbgCensus.EventStream)](https://www.nuget.org/packages/DbgCensus.EventStream) - Base services for interacting with the event streaming API.\
[![Nuget | DbgCensus.EventStream.EventHandlers](https://img.shields.io/nuget/v/DbgCensus.EventStream.EventHandlers?label=DbgCensus.EventStream.EventHandlers)](https://www.nuget.org/packages/DbgCensus.EventStream.EventHandlers) - An abstraction of DbgCensus.EventStream providing an asynchronous and decoupled event handling model.

# Features

- Fluent query building API.
- Event dispatch/handling system and built-in event stream types.
- Fully asynchronous.
- Built around the `Microsoft.Extensions` framework.
- Targeting .NET 6.0.



# [Visit the GitHub repository page for more documentation.](https://github.com/carlst99/DbgCensus)

