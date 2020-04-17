namespace MusicX.Web.Client
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    using MusicX.Web.Client.Infrastructure;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddSingleton(
                new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton<IApplicationState, ApplicationState>();
            builder.Services.AddSingleton<IMediaPlayer, MediaPlayer>();
            builder.Services.AddTransient<IApiClient, ApiClient>();

            await builder.Build().RunAsync();
        }
    }
}
