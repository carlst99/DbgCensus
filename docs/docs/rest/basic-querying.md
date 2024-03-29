# Performing Queries

## Components of a Query

### IQueryBuilder

This is the meat of any query. The `IQueryBuilder` represents a fluent interface for designing a valid query URL that can then be used with an `ICensusRestClient`, or any alternative means, to retrieve data from the Census query endpoints.

```csharp
IQueryBuilder myQuery = new QueryBuilder()
            .WithServiceId("example")
            .OnCollection("outfit")
            .Where("alias.first_lower", SearchModifier.Equals, myOutfitTag.ToLower());

Uri queryEndpoint = myQuery.ConstructEndpoint();
```

### IQueryBuilderFactory

The `IQueryBuilderFactory` interface, and its default implementation, provide the means to retrieve an
`IQueryBuilder` instance that is preconfigured according to the registered `CensusQueryOptions`.

As such, it is recommended that you only create instances of an `IQueryBuilder` through this factory, as you won't
have to set your service ID every time and can take advantage of having a default query language and `limit`.

### ICensusRestClient

The `ICensusRestClient` interface provides functions to perform queries on the Census REST API,
and deserialize the response. The default implementation also checks the response for errors and un-nests the actual data,
allowing your data model to map exactly to the collection structure.

## Making a Query

1. Begin by injecting an `IQueryService` instance. This is a wrapper around the most common functions of the
   `IQueryBuilderFactory` and `ICensusRestClient` interfaces that were discussed earlier.

2. Call the `CreateQuery` method on the `IQueryService` instance, and define your query. Usually, it's
   easiest to build and test your query URL beforehand by making manual calls to the API, and then translate it.

3. Call the `GetAsync` method on the `IQueryService` instance, and pass the query you want to retrieve. Note
   that the result of this call can be null if you've made a query for a singular item that doesn't exist.

```csharp
public record CharacterMinified
(
    ulong CharacterId,
    Name Name,
    FactionDefinition FactionId,
    int PrestigeLevel
);

public class RestExample
{
    private readonly ILogger<RestExample> _logger;
    private readonly IQueryService _queryService;

    public RestExample(ILogger<RestExample> logger, IQueryService queryService)
    {
        _logger = logger;
        _queryService = queryService;
    }

    public async Task<CharacterMinified?> GetCharacter(string name)
    {
        IQueryBuilder query = _queryService.CreateQuery()
            .OnCollection("character")
            .Where("name.first_lower", SearchModifier.Equals, name.ToLower())
            .Show("character_id", "name", "faction_id", "prestige_level");

        try
        {
            CharacterMinified? character = await _queryService.GetAsync<CharacterMinified>(query, ct).ConfigureAwait(false);
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

:::warning
An important distinction to notice when defining queries is that filtering a property is split into two methods. If you'd like to filter a property by a singular value, use the `Where` method:

```csharp
query.Where("property", SearchModifier.LessThan, "value")
```

If you'd like to filter a property by multiple values, use the `WhereAll` method:

```csharp
int[] values = new[] { 1, 2, 3 };
query.WhereAll("property", SearchModifier.Equals, values);
```
:::

## Retrieving collection counts

There is a shortcut for the `count` verb on the `ICensusRestClient` interface. It accepts either a query, or a raw collection name,
and directly returns the Census count value.

```csharp
ICensusRestClient client = ...;
ulong count = await client.CountAsync("character", ct);
```

## Retrieving distinct field values

There is a shortcut for the `c:distinct` parameter on the `ICensusRestClient` interface which directly returns the list of unique values, preventing you from having to define a custom model for the response.

The generic type argument of the method should match the model of the field you are querying.

```csharp
ICensusRestClient client = ...;
IReadOnlyList<int>? distinctValues = await client.DistinctAsync<int>("item", "max_stack_size", ct);
```
