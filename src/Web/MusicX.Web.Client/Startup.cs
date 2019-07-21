namespace MusicX.Web.Client
{
    using Microsoft.AspNetCore.Components.Builder;
    using Microsoft.Extensions.DependencyInjection;

    using MusicX.Web.Client.Infrastructure;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IApplicationState, ApplicationState>();
            services.AddSingleton<IMediaPlayer, MediaPlayer>();
            services.AddTransient<IApiClient, ApiClient>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
