# Advanced Query Setup

## Customizing Polly

It was mentioned in [Getting Started](index.md) that [wait-and-retry](https://github.com/App-vNext/Polly#wait-and-retry)
Polly policy is configured by default. This waits a certain amount of time after a failed query and then re-attempts it.
The number of times the query is retried can be configured when registering the Census REST services:

```csharp
services.AddCensusRestServices(maxRetryAttempts: 5);
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
    CensusQueryOptions sanctuaryQueryOptions = new()
    {
        RootEndpoint = "https://census.lithafalcon.cc"
    }
    ```
   
2. Then, simply pass in these options when creating a query builder.
    ```csharp
    QueryBuilder builder = new(sanctuaryQueryOptions);
    // OR
    IQueryBuilder builder = _queryService.CreateQuery(sanctuaryQueryOptions);
    ```
