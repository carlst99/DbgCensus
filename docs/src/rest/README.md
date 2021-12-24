# Getting started with Querying

## Before you begin

Check out the [REST Sample](https://github.com/carlst99/DbgCensus/tree/main/Samples/RestSample) as you read through this guide.

Note that `DbgCensus.Rest` configures two [Polly](https://github.com/App-vNext/Polly) policies by default. These are:

- Wait and Retry: Performs a jittered exponential backoff up to four times when a query fails.
- Circuit Breaker: Puts a hold on all queries for 30 seconds if five query attempts fail consecutively.

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

```csharp{17-22}
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

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
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
                        o.DeserializationOptions = new JsonSerializerOptions(...);
                        o.Limit = 100; // Optional: sets a default limit for each query.
                        // Etc.
                    }
                );
            });
}
```

## Making a Query

1. Inject an `IQueryService` instance. This is a wrapper around the registered `IQueryBuilderFactory` and `ICensusRestClient` objects, which you can use individually if you need slightly more control over your queries.

2. Call the `CreateQuery` method on the `IQueryService` instance, and define your query. Usually, it's easiest to build and test your query URL beforehand by making manual calls to the API, and then translate it.

3. Call the `GetAsync` method on the `IQueryService` instance, and pass the query you want to retrieve. Note that the result of this call can be null if you've made a query for a singular item that doesn't exist.

```csharp
public class RestExample
{
    private readonly IQueryService _queryService;

    public RestExample(IQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Character?> GetCharacter(string name)
    {
        IQueryBuilder query = _queryService.CreateQuery()
            .OnCollection("character")
            .Where("name.first_lower", SearchModifier.Equals, name.ToLower());

        try
        {
            Character? character = await _queryService.GetAsync<Character>(query, ct).ConfigureAwait(false);
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

Using the `count` verb to retrieve the number of elements in a collection can be done in two ways. Firstly, by using the shortcut on the `ICensusRestClient`, or secondly by making a query as per usual and defining the query type. Just remember to de-serialise to a number type!

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

There is a shortcut for the `c:distinct` parameter on the `ICensusRestClient` interface which directly returns the list of unique values, preventing you from having to define a custom model.

The generic type argument of the method should match the model of the field you are querying.

```csharp
ICensusRestClient client = ...;
IReadOnlyList<int>? distinctValues = await client.DistinctAsync<int>("item", "max_stack_size", ct);
```