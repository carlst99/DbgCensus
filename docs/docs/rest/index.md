# Setup

This document will guide you through installing and setting up `DbgCensus.Rest`, the package
for interacting with Census' query interface.

:::tip
Check out the [REST Sample](https://github.com/carlst99/DbgCensus/tree/main/Samples/RestSample) as you read through this guide.

It's recommended that you use a template that wires up the [Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host),
such as a *Worker Service* or an *ASP.NET Core* project.
:::

:::info
`DbgCensus.Rest` configures a wait-and-retry [Polly](https://github.com/App-vNext/Polly) policies by default.
This will perform a jittered exponential backoff up to four (by default) times when a query fails.

See [Advanced Configuration](advanced.md) for more information.
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

```csharp{16,25}
using DbgCensus.Rest;
using DbgCensus.Rest.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace RestSample;

public static class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<CensusQueryOptions>(hostContext.Configuration.GetSection(nameof(CensusQueryOptions)));

                // AND/OR
                services.Configure<CensusQueryOptions>(o =>
                {
                    o.LanguageCode = CensusLanguage.English
                    // Etc.
                });

                services.AddCensusRestServices();
                ...
            })
            .Build();

        await host.RunAsync();
    }
}
```

`appsettings.json`:

<<< @/../../Samples/RestSample/appsettings.json
