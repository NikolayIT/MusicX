namespace MusicX.Web.Client.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Blazor;
    using Microsoft.JSInterop;

    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Account;
    using MusicX.Web.Shared.Application;
    using MusicX.Web.Shared.Songs;

    public class ApiClient : IApiClient
    {
        private readonly HttpClient httpClient;

        private readonly IApplicationState applicationState;

        public ApiClient(HttpClient httpClient, IApplicationState applicationState)
        {
            this.httpClient = httpClient;
            this.applicationState = applicationState;
        }

        public async Task<ApiResponse<IEnumerable<SongListItem>>> GetSongsList()
        {
            return await this.GetJson<IEnumerable<SongListItem>>("api/Songs/GetList");
        }

        public async Task<ApiResponse<ApplicationStartResponseModel>> ApplicationStart()
        {
            return await this.GetJson<ApplicationStartResponseModel>("api/Application/Start");
        }

        public async Task<ApiResponse<ApplicationStopResponseModel>> ApplicationStop(ApplicationStopRequestModel request)
        {
            return await this.PostJson<ApplicationStopResponseModel>("api/Application/Stop", request);
        }

        public async Task<ApiResponse<UserRegisterResponseModel>> UserRegister(UserRegisterRequestModel request)
        {
            return await this.PostJson<UserRegisterResponseModel>("api/Account/Register", request);
        }

        public async Task<ApiResponse<UserLoginResponseModel>> UserLogin(UserLoginRequestModel request)
        {
            try
            {
                var response = await this.httpClient.PostAsync(
                                   "api/account/login",
                                   new FormUrlEncodedContent(
                                       new List<KeyValuePair<string, string>>
                                       {
                                           new KeyValuePair<string, string>("email", request.Email),
                                           new KeyValuePair<string, string>("password", request.Password),
                                       }));
                var responseString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<UserLoginResponseModel>(new ApiError("Server error " + (int)response.StatusCode, responseString));
                }

                var responseObject = Json.Deserialize<UserLoginResponseModel>(responseString);
                return new ApiResponse<UserLoginResponseModel>(responseObject);
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserLoginResponseModel>(new ApiError("HTTP Client", ex.Message));
            }
        }

        private async Task<ApiResponse<T>> PostJson<T>(string url, object request)
        {
            if (this.applicationState.IsLoggedIn)
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.applicationState.UserToken);
            }

            try
            {
                return await this.httpClient.PostJsonAsync<ApiResponse<T>>(url, request);
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>(new ApiError("HTTP Client", ex.Message));
            }
        }

        private async Task<ApiResponse<T>> GetJson<T>(string url)
        {
            if (this.applicationState.IsLoggedIn)
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.applicationState.UserToken);
            }

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
