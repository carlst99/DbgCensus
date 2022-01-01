# CHANGELOG

Date format: DD/MM/YYYY

## vNext

#### EventStream-vNext

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

#### EventStream.EventHandlers-vNext

- The `EventHandlingEventStreamClient` now automatically refreshes its current subscription at the interval set by the `EventHandlingClientOptions.SubscriptionRefreshIntervalMilliseconds` property. This helps with scenarios where the event stream drops certain components of a subscription.

- Added `EventHandlingClientOptions`. This currently only contains the `SubscriptionRefreshIntervalMilliseconds` property, that can be used
to configure how frequently the `EventHandlingEventStreamClient` will refresh its current subscription. This property defaults to 15min.

- The `EventHandlingEventStreamClient` now dispatches an `IUnknownPayload` when a payload fails to deserialize and/or dispatch.

#### Rest-vNext

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