namespace MusicX.Web.Client.Infrastructure
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MusicX.Web.Shared;

    public interface IApiClient
    {
        Task<IEnumerable<WeatherForecast>> GetWeatherForecasts();
    }
}
