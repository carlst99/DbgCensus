# CHANGELOG

Date format: YYYY/MM/DD (ISO)

## 2023/07/11

#### EventStream-v2.7.1

- Thanks @thelostv - Fixed `EventNames#PlayerLogout` pointing to the *PlayerLogin* event.
- Fixed the thread-safety of `BaseEventStreamClient#SendCommandAsync`.
- `IEcho` and `IServiceMessage` now support covariant generics.

#### EventHandlers-v3.5.1

- Updated dependency on `DbgCensus.EventStream` to v2.7.1.
- Thanks @thelostv - Fixed *PlayerLogin* events being deserialized to `PlayerLogout` types when using the default 
event type registration.

#### Rest-v3.7.1

- Improved pagination reliability on multi-database collections by using `WithLimitPerDatabase`.

⚠️ Date format changes to: DD/MM/YYYY

## 02/04/2023

#### Core-v2.1.0, EventStream-v2.7.0, EventHandlers-v3.5.0, Rest-v3.7.0

- Multi-target .NET 6.0 and .NET 7.0.
- Updated NuGet dependencies.

## 02/04/2023

#### Rest-v3.6.0

- Added the `IQueryBuilder#WithCustomParameter` function.
- An exception is now thrown when attempting to use both `ShowFields` and `HideFields` on a `QueryBuilder`.
- Updated dependency on `DbgCensus.Core` to v2.0.1.

## 23/03/2023

#### Core-v2.0.1

- Added the `Optional#GetValueOrDefault` function.

## 11/03/2023

#### Rest-v3.5.0

- Removed the circuit breaker policy. Per information from Census developers, requests are not rate-limited in any way
  and errors/timeouts are likely to be collection-specific.

## 03/03/2023

#### Rest-v3.4.0

- `ICensusRestClient.cs#GetPaginatedAsync` can no longer return null pages.
- Added a default value of `int.MaxValue` for `ICensusRestClient.cs#GetPaginatedAsync(pageCount)`.

## 08/03/2023

#### EventStream-v2.6.0, EventHandlers-v3.4.0

- Removed the `IContinentUnlock` event - this was never actually implemented on the Census end.
- Correctly renamed `IDeath#AttackerFireMode` to `AttackerFireModeID`.

## 03/02/2022

#### Rest-v3.3.0

- Fixed `QueryType#Count` returning the wrong verb, and hence the `ICensusRestClient#CountAsync` method.
- Added an `ICensusRestClient.cs#CountAsync` overload to count results of query, rather than entire collection.

## 06/10/2022

#### Core-v2.0.0

- ⚠ Fixed the definitions for _Indar_ and _Amerish_ meltdown metagame events.
- Added the new 'Haunted Bastion' metagame event definitions

#### EventStream-v2.5.0, EventHandlers-v3.3.0, Rest-v3.2.0

- Updated dependency on `DbgCensus.Core` to v2.0.0.

## 08/09/2022

#### Core-v1.5.0

Added the new 'Sudden Death' metagame event definitions.

#### EventStream-v2.4.0, EventHandlers-v3.2.0

- Added the `AttackerTeamID` and `TeamID` properties to the `IVehicleDestroy` event.
- Added the `TeamID` property to the `IGainExperience` event.
- Updated dependency on `DbgCensus.Core` to v1.5.0

#### Rest-v3.1.0

- Updated dependency on `DbgCensus.Core` to v1.5.0

## 27/08/2022

#### EventStream-v2.3.9, EventHandlers-v3.1.5

- Added the `AttackerTeamID`, `IsCritical` and `TeamID` properties to the `IDeath` event.

## 19/08/2022

#### EventStream-v2.3.8, EventHandlers-v3.1.4

- Added the `IItemAdded` event stream type.

## 07/08/2022

#### Core-v1.4.2

- Consider digit sequences as words in the snake-case JSON name converter.

#### EventStream-v2.3.7, EventHandlers-v3.1.3, Rest-v3.0.5

- Updated dependency on `DbgCensus.Core` to v1.4.2.

## 16/06/2022

#### Core-v1.4.1

- Added metagame definitions for Air Anomalies.
- Added XML documentation for various metagame definitions.
- Added the `MetagameEventDefinition#GetAlertDuration` extension method.

#### EventStream-v2.3.6, EventHandlers-v3.1.2, Rest-v3.0.4

- Updated dependency on `DbgCensus.Core` to v1.4.1

## 08/06/2022

#### Core-v1.4.0

- Added the `GlobalizedString` object.

#### EventStream-v2.3.5, EventHandlers-v3.1.1, Rest-v3.0.3

- Updated dependency on `DbgCensus.Core` to v1.4.0

## 22/04/2022

#### Rest-v3.0.2

- Fixed being unable to hide join fields.

## 02/04/2022

#### EventStream.EventHandlers-v3.1.0

- Shift all payload dispatch logic to the DefaultPayloadDispatchService.

## 23/03/2022

#### EventStream-2.3.4, EventStream.EventHandlers-v3.0.4

- Updated dependency on DbgCensus.Core to v1.3.2

## 22/03/2022

#### Rest-v3.0.1

- Fixed being unable to use the `hide` query command.
- Added missing `limit` parameter to IQueryService#DistinctAsync.

## 17/03/2022

#### Rest-v3.0.0

- Added a service ID rotation Polly policy. Populate the `CensusQueryOptions.ServiceIDs` array with all service IDs that you want to rotate through, and they will be rotated in order when a Census REST request fails. May be useful if you are seeing a large number of connection resets.
- Fixed deserialization to enumerable types when only one element was returned by the query.
- Added support for changing query options at runtime in the QueryBuilderFactory by using IOptionsMonitor.
- Shifted the `CensusLanguage` and `CensusNamespace` types to the `DbgCensus.Rest.Objects` namespace.
- Use of Polly policies can now be configured when calling `AddCensusRestServices`.
- Removed the `QueryBuilder(string serviceID, string censusNamespace, string rootEndpoint)` constructor.
- The `CensusRestClient` is no longer disposable.
- Allow a limit to be specified in `ICensusRestClient#DistinctAsync`.
- Updated dependency on `DbgCensus.Core` v1.3.2

## 15/03/2022

#### Core-v1.3.2

- Added the Oshur Conquest alert definition (226).

## 12/03/2022

#### Rest-v2.2.3

- `CensusRestClient#GetPaginatedAsync` now breaks when the results count is less than the page size.

---

#### Core-v1.3.1

- Fixed the `OptionalJsonConverter` for cases where the `Optional<T>` contained no value.

#### EventStream-v2.3.3, EventStream.EventHandlers-v3.0.3, Rest-v2.2.2

- Updated dependency on `DbgCensus.Core` to v1.3.1.

## 10/03/2022

#### Core-v1.3.0

- Added the `Optional<TValue>` struct, which can be used in Census data models to indicate fields that may be missing (read: optional) in the response.

#### EventStream-v2.3.2, EventStream.EventHandlers-v3.0.2, Rest-v2.2.1

- Updated dependency on `DbgCensus.Core` to v1.3.0.

## 09/03/2022

#### EventStream-v2.3.1, EventStream.EventHandlers-v3.0.1

- Fixed `PlayerFacilityDefend` inheriting from the wrong interface

---

#### Rest-v2.2.0

- An exception is no longer thrown when deserializing multiple query elements to a non-enumerable type. The new behaviour is as follows:
  - A Census response containing no elements returns the `default` type.
  - A Census response containing only one element returns the deserialized value of that element.
  - A Census response containing multiple elements returns the deserialized value of the entire element array.
- Converted many `uint` parameters on query interfaces to `int`s.

---

#### EventStream.EventHandlers-v3.0.0

- Added the `DefaultPayloadDispatchService`, which implements the required logic for payload dispatching. This service is registered as a singleton and can be retrieved by injecting an `IPayloadDispatchService` instance.
- Correspondingly, the `EventHandlingEventStreamClient` no longer implements the `IPayloadDispatchService` interface.

#### Rest-v2.1.0

- `ICensusRestClient` no longer inherits from `IDisposable`.
- `CensusRestClient` now directly implements `IDisposable`.

## 08/03/2022

#### EventStream.EventHandlers-v2.4.0

- Fixed event dispatching!
- Added pre-dispatch event handlers.

---

#### EventStream-v2.3.0

- Added the `InstanceID` and `MetagameEventStateName` properties to `IMetagameEvent`.
- Added `EventNames#GetExperienceEventName` as a util to retrieve a valid event name for a given experience event ID.
- `BaseEventStreamClient#HandlePayloadAsync` now returns a ValueTask.

#### EventStream.EventHandlers-v2.3.1

- Updated dependency on DbgCensus.EventStream to v2.3.0

---

#### Core-v1.2.0

- Converted `ZoneId` to a readonly struct, using computed properties for the `Instance` and `Definition` properties.

#### EventStream-v2.2.0, EventStream.EventHandlers-v2.3.0

- Updated dependency on DbgCensus.Core to v1.2.0

## 07/03/2022

#### EventStream.EventHandlers-v2.2.0

- Added the IPayloadDispatchService interface, and implemented it in the EventHandlingEventStreamClient. This allows custom payloads to be dispatched.

#### EventStream-v2.1.0, EventStream.EventHandlers-v2.1.0

- Fixed `IEventStreamClientFactory#GetClient` when retrieving a client for a dedicated generic consumer.
- The `BaseEventStreamClient` now disposes of the data stream passed to `HandlePayloadAsync`.
- `BaseEventStreamClient#Name` is now a readonly-property.

#### Rest-v2.0.0

- The `JoinBuilder`, `TreeBuilder` and `QueryBuilder` types are now sealed.
- The `SearchModifier` enumeration is now a `short` data type.
- `StringUtils#SafeToString` now expects a generically-typed object, rather than an `object` parameter.

## 04/03/2022

#### EventStream.EventHandlers-v2.0.3

- Significant performance improvement to event dispatch by caching the retrieved `MethodInfo`.

## 03/02/2022

#### Core-v1.1.0, EventStream-v2.0.2, EventStream.EventHandlers-v2.0.2, Rest-v1.2.1

- Added Oshur zone and metagame event (meltdown only) definitions.

## 21/01/2022

#### EventStream-v2.0.1, EventStream.EventHandlers-v2.0.1

- Fix reconnection flow and improve start/stop logic
- Improve disposable implementation.

## 02/01/2022

#### EventStream-v2.0.0

- The `Characters` and `Worlds` properties on the `ISubscribe` command now use a `OneOf<>` object to be stricter about the type of subscription
you can make. The properties accept either the special `All` value, or a list of their corresponding type. E.g. the `Worlds` property has
a type of `OneOf<All, IEnumerable<WorldDefinition>>`.

- The way that the `JsonSerializerOptions` used by the `BaseEventStreamClient` are configured has changed. Rather than being properties of the
`EventStreamOptions` class, with configuration completed in the `BaseEventStreamClient` itself, the `JsonSerializerOptions` are now `Configure`d
as named options directly to the service collection. The name of these options can be found in the new `Constants` class.

- The `BaseEventStreamClient` now properly serializes commands that were passed as an interface type, rather than their concrete type.

- The `IEventStreamClient` interface no longer inherits from `IDisposable`.

- `BaseEventStreamClient` and `EventStreamClientFactory` now implement `IAsyncDisposable`. The `EventStreamClientFactory` now manages its clients
lifetimes.

#### EventStream.EventHandlers-v2.0.0

- The `EventHandlingEventStreamClient` now automatically refreshes its current subscription at the interval set by the `EventHandlingClientOptions.SubscriptionRefreshIntervalMilliseconds` property. This helps with scenarios where the event stream drops certain components of a subscription.

- Added `EventHandlingClientOptions`. This currently only contains the `SubscriptionRefreshIntervalMilliseconds` property, that can be used
to configure how frequently the `EventHandlingEventStreamClient` will refresh its current subscription. This property defaults to 15min.

- The `EventHandlingEventStreamClient` now dispatches an `IUnknownPayload` when a payload fails to deserialize and/or dispatch.

#### Rest-v1.2.0

- The way that the `JsonSerializerOptions` used by the `CensusRestClient` are configured has changed. Rather than being properties of the
`CensusQueryOptions` class, the `JsonSerializerOptions` are now `Configure`d as named options directly to the service collection.
The name of these options can be found in the new `Constants` class.

## 30/12/2021

#### Core-v1.0.4

- Changelog begins.

#### EventStream-v1.0.2

- Changelog begins.

#### EventStream.EventHandlers-v1.0.2

- Changelog begins.

#### Rest-v1.1.1

- Changelog begins.

#### Samples

- Changelog begins.

#### Documentation Website

- Changelog begins.
