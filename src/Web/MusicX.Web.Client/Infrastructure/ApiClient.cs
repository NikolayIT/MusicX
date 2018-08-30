namespace MusicX.Web.Client.Infrastructure
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Blazor;

    using MusicX.Web.Shared;

    public class ApiClient : IApiClient
    {
        private readonly HttpClient httpClient;

        public ApiClient(HttpClient httpClient, IApplicationState applicationState)
        {
            this.httpClient = httpClient;
            if (applicationState.IsLoggedIn)
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", applicationState.UserToken);
            }
        }

        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts()
        {
            return await this.httpClient.GetJsonAsync<IEnumerable<WeatherForecast>>("api/SampleData/WeatherForecasts");
        }
    }
}
