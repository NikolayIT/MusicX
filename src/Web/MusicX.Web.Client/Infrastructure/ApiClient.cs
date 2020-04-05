namespace MusicX.Web.Client.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Account;
    using MusicX.Web.Shared.Application;
    using MusicX.Web.Shared.Home;
    using MusicX.Web.Shared.Playlists;
    using MusicX.Web.Shared.Songs;
    using MusicX.Web.Shared.TelemetryData;

    public class ApiClient : IApiClient
    {
        private readonly HttpClient httpClient;

        private readonly IApplicationState applicationState;
        private readonly IJSRuntime jsRuntime;

        public ApiClient(HttpClient httpClient, IApplicationState applicationState, IJSRuntime jsRuntime)
        {
            this.httpClient = httpClient;
            this.applicationState = applicationState;
            this.jsRuntime = jsRuntime;
        }

        public Task<ApiResponse<IndexListsResponseModel>> GetIndexLists() =>
            this.GetJson<IndexListsResponseModel>("api/Home/GetIndexLists");

        public Task<ApiResponse<GetSongByIdResponse>> GetSongById(int id) =>
            this.GetJson<GetSongByIdResponse>($"api/Songs/GetById?id={id}");

        public Task<ApiResponse<SongsListResponseModel>> GetSongsList(int page, string searchTerms) =>
            this.GetJson<SongsListResponseModel>($"api/Songs/GetList?page={page}&searchTerms={searchTerms}");

        public Task<ApiResponse<GetSongsByIdsResponse>> GetSongsByIds(GetSongsByIdsRequest request) =>
            this.PostJson<GetSongsByIdsRequest, GetSongsByIdsResponse>("api/Songs/GetSongsByIds", request);

        public Task<ApiResponse<AddSongResponse>> AddSong(AddSongRequest request) =>
            this.PostJson<AddSongRequest, AddSongResponse>("api/Songs/AddSong", request);

        public Task<ApiResponse<AddSimilarSongsResponse>> AddSimilarSongs(AddSimilarSongsRequest request) =>
            this.PostJson<AddSimilarSongsRequest, AddSimilarSongsResponse>("api/Songs/AddSimilarSongs", request);

        public Task<ApiResponse<GetSongsInPlaylistResponse>> GetSongsInPlaylist(int id) =>
            this.GetJson<GetSongsInPlaylistResponse>("api/Songs/GetSongsInPlaylist?id=" + id);

        public Task<ApiResponse<GetAllPlaylistResponse>> GetAllPlaylists() =>
            this.GetJson<GetAllPlaylistResponse>("api/Playlists/GetAll");

        public Task<ApiResponse<CreatePlaylistFromListResponse>> CreatePlaylistFromList(CreatePlaylistFromListRequest request) =>
            this.PostJson<CreatePlaylistFromListRequest, CreatePlaylistFromListResponse>("api/Playlists/CreateFromList", request);

        public Task<ApiResponse<ApplicationStartResponseModel>> ApplicationStart() =>
            this.GetJson<ApplicationStartResponseModel>("api/Application/Start");

        public Task<ApiResponse<ApplicationStopResponseModel>> ApplicationStop(ApplicationStopRequestModel request) =>
            this.PostJson<ApplicationStopRequestModel, ApplicationStopResponseModel>("api/Application/Stop", request);

        public Task<ApiResponse<SongPlayTelemetryResponse>> TelemetrySongPlay(SongPlayTelemetryRequest request) =>
            this.PostJson<SongPlayTelemetryRequest, SongPlayTelemetryResponse>("api/TelemetryData/SongPlay", request);

        public Task<ApiResponse<UserRegisterResponseModel>> UserRegister(UserRegisterRequestModel request) =>
            this.PostJson<UserRegisterRequestModel, UserRegisterResponseModel>("api/Account/Register", request);

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

                var responseObject = JsonSerializer.Deserialize<UserLoginResponseModel>(responseString);
                return new ApiResponse<UserLoginResponseModel>(responseObject);
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserLoginResponseModel>(new ApiError("HTTP Client", ex.Message));
            }
        }

        private async Task<ApiResponse<TResponse>> PostJson<TRequest, TResponse>(string url, TRequest request)
        {
            if (this.applicationState.IsLoggedIn)
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.applicationState.UserToken);
            }
            else if (await this.jsRuntime.ReadToken() != null)
            {
                // This is workaround for https://github.com/aspnet/Blazor/issues/1185
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.applicationState.UserToken);
            }

            try
            {
                var response = await this.httpClient.PostAsJsonAsync(url, request);
                var responseObject = await response.Content.ReadFromJsonAsync<ApiResponse<TResponse>>();
                return responseObject;
            }
            catch (Exception ex)
            {
                return new ApiResponse<TResponse>(new ApiError("HTTP Client", ex.Message));
            }
        }

        private async Task<ApiResponse<T>> GetJson<T>(string url)
        {
            if (this.applicationState.IsLoggedIn)
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.applicationState.UserToken);
            }
            else if (await this.jsRuntime.ReadToken() != null)
            {
                // This is workaround for https://github.com/aspnet/Blazor/issues/1185
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.applicationState.UserToken);
            }

            try
            {
                return await this.httpClient.GetFromJsonAsync<ApiResponse<T>>(url);
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>(new ApiError("HTTP Client", ex.Message));
            }
        }
    }
}
