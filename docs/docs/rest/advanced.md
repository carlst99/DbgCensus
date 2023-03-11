# Advanced Query Setup

## Customizing Polly

It was mentioned in [Getting Started](index.md) that [wait-and-retry](https://github.com/App-vNext/Polly#wait-and-retry)
Polly policy is configured by default. This waits a certain amount of time after a failed query and then re-attempts it.
The number of times the query is retried can be configured when registering the Census REST services:

```csharp
services.AddCensusRestServices(maxRetryAttempts: 5);
```

## Service ID Rotation

There has been observation by the community that rotating the service ID you use when querying, can
help to reduce the amount of failures. `DbgCensus` manages this automatically, by attempting to rotate
your service ID after the wait-and-retry policy fails its last retry attempt. In order to set this up,
you must configure the `ServiceIDs` property of your `CensusQueryOptions`, and provide multiple service
IDs to rotate through.

`appsettings.json`:

```json
{
  "CensusQueryOptions": {
    "ServiceId": "example",
    "ServiceIDs": [
      "example",
      "asdf"
    ]
  }
}
```

## Using Alternative Census Implementations

With Census being, at one point in its history, two years out of date on any static data, the API developer community began
to extract and share information through alternative means. Some of these efforts have included Census-like API implementations.
We'll be using [Sanctuary.Census](https://github.com/PS2Sanctuary/Sanctuary.Census) as one such example.

`DbgCensus` can be easily configured to use alternative Census implementations, provided their interface is similar.
The [REST Sample](https://github.com/carlst99/DbgCensus/tree/main/Samples/RestSample) demonstrates the following instruction
by means of configuring a *named options* instance, so that the configured third-party Census options can be injected anywhere.

1. Create a `CensusQueryOptions` instance, and point it towards the alternative Census implementation:
    ```csharp
    CensusQueryOptions falconQueryOptions = new()
    {
        RootEndpoint = "https://census.lithafalcon.cc"
    }
    ```
   
2. Then, simply pass in these options when creating a query builder.
    ```csharp
    QueryBuilder builder = new(falconQueryOptions);
    // OR
    IQueryBuilder builder = _queryService.CreateQuery(falconQueryOptions);
    ```
