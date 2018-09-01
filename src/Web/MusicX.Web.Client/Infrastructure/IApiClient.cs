namespace MusicX.Web.Client.Infrastructure
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Account;
    using MusicX.Web.Shared.Application;

    public interface IApiClient
    {
        Task<ApiResponse<IEnumerable<WeatherForecast>>> GetWeatherForecasts();

        Task<ApiResponse<ApplicationStartResponseModel>> ApplicationStart();

        Task<ApiResponse<ApplicationStopResponseModel>> ApplicationStop(ApplicationStopRequestModel request);

        Task<ApiResponse<UserRegisterResponseModel>> UserRegister(UserRegisterRequestModel request);

        Task<ApiResponse<UserLoginResponseModel>> UserLogin(UserLoginRequestModel request);
    }
}
