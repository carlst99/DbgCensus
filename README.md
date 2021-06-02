<img title="DbgCensus Icon" alt="DbgCensus Icon" src="https://github.com/carlst99/DbgCensus/blob/main/Assets/Icon_128.png?raw=true" align="left" />

# DbgCensus

DbgCensus is a low-level c# wrapper for [Daybreak Game Company's Census API](https://census.daybreakgames.com). It was built with PlanetSide 2's endpoints in mind, but should work across all namespaces.

**This package is unofficial and is not affiliated with Daybreak Games Company in any way.**

![Nuget | DbgCensus.Core](https://img.shields.io/nuget/v/DbgCensus.Core?label=DbgCensus.Core)
![Nuget | DbgCensus.Rest](https://img.shields.io/nuget/v/DbgCensus.Rest?label=DbgCensus.Rest)

***

- Fluent query building API with full coverage of the Census query interface.
- Fully asynchronous.
- Highly extendable - core components can be extended, replaced and used individually.
- Built around the `Microsoft.Extensions` framework.
- Compiled for .NET 5.0

## Getting Started [WIP]

Before you do anything, you should consider getting a *Service ID*.

### Interacting with Census Query Endpoints

Obtaining data 

## Roadmap

- Support for the event stream.
- Proper documentation.
- Polly implementation for the REST interface.
- Complete unit testing coverage.