# Setup

:::tip
Check out the [Event Streaming Sample](https://github.com/carlst99/DbgCensus/tree/main/Samples/EventStreamSample)
as you read through this guide.

It's recommended that you use a template that wires up the [Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host),
such as a *Worker Service* or an *ASP.NET Core* project.
:::

:::info
There are two `DbgCensus.*` packages available for assisting with event streaming.

- `DbgCensus.EventStream` is the base library. It contains event stream types and a client websocket implementation.
  However, it does not handle events and you must implement the abstract `BaseEventStreamClient` class to do so.

- `DbgCensus.EventStream.EventHandlers` builds off the base library to add an asynchronous event dispatch system
  and niceties like automatic subscription refreshing. The majority of the event streaming documentation will 
  assume that you are using this package.
:::

1. Install `DbgCensus.EventStream.EventHandlers`:

    ```powershell
    # Visual Studio Package Manager
    Install-Package DbgCensus.EventStream.EventHandlers
    # dotnet console
    dotnet add package DbgCensus.EventStream.EventHandlers
    ```

2. Register the required types to an `IServiceCollection` with the `AddCensusEventHandlingServices` extension method.
   If you aren't using `Microsoft.Extensions.DependencyInjection`, take a look at
   [this file](https://github.com/carlst99/DbgCensus/blob/main/DbgCensus.EventStream.EventHandlers/Extensions/IServiceCollectionExtensions.cs)
   to see how the required services are setup.

3. Configure an instance of the `EventStreamOptions` class to ensure that your service ID is utilised.
   Typically, you'd register your options from a configuration source (such as a section of `appsettings.json`)
   to retrieve any secrets that shouldn't be stored with the code (i.e. - the service ID!), and then follow up
   with any additional runtime configuration. You might also want to configure the `EventHandlingClientOptions`
   class to customise the automatic subscription refresh interval.

**Example**

You'll note here that we're adding various *handlers*. They'll be explained in more detail later.

```csharp{17,19-23}
using DbgCensus.EventStream;
using DbgCensus.EventStream.EventHandlers.Extensions;
using EventStreamSample.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace EventStreamSample;

public static class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<EventStreamOptions>(hostContext.Configuration.GetSection(nameof(EventStreamOptions)));

                services.AddCensusEventHandlingServices()
                        .RegisterPreDispatchHandler<DuplicatePreventionPreDispatchHandler>()
                        .AddPayloadHandler<ConnectionStateChangedPayloadHandler>()
                        .AddPayloadHandler<FacilityControlPayloadHandler>()
                        .AddPayloadHandler<UnknownPayloadHandler>();
            })
            .Build();
            
         await host.RunAsync();
    }
}
```

## Running the Client

TODO: Retrieve instance from factory.
Run client task.

## The big TODO

Please peruse the [Event Streaming Sample](https://github.com/carlst99/DbgCensus/tree/main/Samples/EventStreamSample)
in the meantime.

TODO:
- All about handlers
- Pre-dispatch handlers
  - Pre-dispatch handlers all run in the same scope per event, whereas payload handlers each have their own scope
