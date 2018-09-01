namespace MusicX.Web.Client.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Blazor;

    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Account;

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

        public async Task<ApiResponse<IEnumerable<WeatherForecast>>> GetWeatherForecasts()
        {
            return await this.Get<IEnumerable<WeatherForecast>>("api/SampleData/WeatherForecasts");
        }

        public async Task<ApiResponse<UserRegisterResponseModel>> UserRegister(UserRegisterRequestModel request)
        {
            return await this.Post<UserRegisterResponseModel>("api/Account/Register", request);
        }

        public async Task<ApiResponse<UserLoginResponseModel>> UserLogin(UserLoginRequestModel request)
        {
            return await this.Post<UserLoginResponseModel>("api/Account/Login", request);
        }

        private async Task<ApiResponse<T>> Post<T>(string url, object request)
        {
            try
            {
                return await this.httpClient.PostJsonAsync<ApiResponse<T>>(url, request);
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>(new ApiError("HTTP Client", ex.Message));
            }
        }

        private async Task<ApiResponse<T>> Get<T>(string url)
        {
            try
            {
                return await this.httpClient.GetJsonAsync<ApiResponse<T>>(url);
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>(new ApiError("HTTP Client", ex.Message));
            }
        }
    }
}
