using DbgCensus.Rest;
using DbgCensus.Rest.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DbgCensus.Demo
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddCensusRestServices();
            builder.Services.Configure<CensusQueryOptions>((o) =>
            {
                o.LanguageCode = CensusLanguage.ENGLISH;
                o.Namespace = CensusNamespace.PS2;
                o.Limit = 100;
                o.ServiceId = "example";
            });

            await builder.Build().RunAsync().ConfigureAwait(false);
        }
    }
}
