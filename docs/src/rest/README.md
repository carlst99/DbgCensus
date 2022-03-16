# Getting started with Querying

## Before you begin

Check out the [REST Sample](https://github.com/carlst99/DbgCensus/tree/main/Samples/RestSample) as you read through this guide.

Note that `DbgCensus.Rest` configures two [Polly](https://github.com/App-vNext/Polly) policies by default. These are:

- Wait and Retry: Performs a jittered exponential backoff up to four times when a query fails.
- Circuit Breaker: Throws an exception on any queries made in a 15sec window, after having four queries fail consecutively.

## Setup

::: tip
It's recommended that you use a template that wires up the [Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host), such as a *Worker Service* or an *ASP.NET Core* project.
:::

1. Install `DbgCensus.Rest`:

    ```powershell
    # Visual Studio Package Manager
    Install-Package DbgCensus.Rest
    # dotnet console
    dotnet add package DbgCensus.Rest
    ```

2. Register the required types to an `IServiceCollection` with the `AddCensusRestServices` extension method.\
If you aren't using `Microsoft.Extensions.DependencyInjection`, take a look at [this file](https://github.com/carlst99/DbgCensus/blob/main/DbgCensus.Rest/Extensions/IServiceCollectionExtensions.cs) to see how the required services are setup.

3. Configure an instance of the `CensusQueryOptions` class to ensure that your service ID is utilised.\
Typically, you'd register your options from a configuration source (such as a section of `appsettings.json`) to retrieve any secrets that shouldn't be stored with the code (i.e. - the service ID!), and then follow up with any additional runtime configuration.

**Example**

```csharp{18-23}
using DbgCensus.Rest;
using DbgCensus.Rest.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace RestSample;

public static class Program
{
    public static async Task Main(string[] args)
        => await CreateHostBuilder(args).Build().RunAsync().ConfigureAwait(false);

    public static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddCensusRestServices();

                services.Configure<CensusQueryOptions>
                (
                    hostContext.Configuration.GetSection(nameof(CensusQueryOptions))
                );

                // AND/OR
                services.Configure<CensusQueryOptions>
                (
                    o =>
                    {
                        o.LanguageCode = CensusLanguage.English
                        // Etc.
                    }
                );

                ...
            });
}
```

## Components of a Query

### IQueryBuilder

This is the meat of any query. The `IQueryBuilder` represents a fluent interface for designing a valid query URL that can then be used with an `ICensusRestClient`, or any alternative means, to retrieve data from the Census query endpoints.

```csharp
IQueryBuilder myQuery = new QueryBuilder()
            .OnCollection("outfit")
            .Where("alias.first_lower", SearchModifier.Equals, myOutfitTag.ToLower());

Uri queryEndpoint = myQuery.ConstructEndpoint();
```

### IQueryBuilderFactory

The `IQueryBuilderFactory` interface, and its default implementation, provide the means to retrieve an `IQueryBuilder` instance that is preconfigured according to the registered `CensusQueryOptions`.

As such, it is recommended that you only create instances of an `IQueryBuilder` through this factory, as you won't have to set your service ID every time.

### ICensusRestClient

The `ICensusRestClient` interface provides functions to perform queries on the Census REST API, and deserialize the response. The default implementation also checks the response for errors and un-nests the actual data, allowing your data model to map exactly to the collection structure.


## Making a Query

1. Begin by injecting an `IQueryService` instance. This is a wrapper around the most common functions of the `IQueryBuilderFactory` and `ICensusRestClient` interfaces that were discussed earlier.

2. Call the `CreateQuery` method on the `IQueryService` instance, and define your query. Usually, it's easiest to build and test your query URL beforehand by making manual calls to the API, and then translate it.

3. Call the `GetAsync` method on the `IQueryService` instance, and pass the query you want to retrieve. Note that the result of this call can be null if you've made a query for a singular item that doesn't exist.

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
    private readonly IQueryService _queryService;

    public RestExample(IQueryService queryService)
    {
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

Using the `count` verb to retrieve the number of elements in a collection can be done in two ways. Firstly, by using the shortcut on the `ICensusRestClient`, or secondly by making a query as per usual and defining the query type. Just remember to deserialise to a number type!

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

There is a shortcut for the `c:distinct` parameter on the `ICensusRestClient` interface which directly returns the list of unique values, preventing you from having to define a custom model for the response.

The generic type argument of the method should match the model of the field you are querying.

```csharp
ICensusRestClient client = ...;
IReadOnlyList<int>? distinctValues = await client.DistinctAsync<int>("item", "max_stack_size", ct);
```
