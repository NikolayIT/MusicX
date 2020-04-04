namespace MusicX.Web.Client.Infrastructure
{
    using System.Threading.Tasks;

    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Account;
    using MusicX.Web.Shared.Application;
    using MusicX.Web.Shared.Home;
    using MusicX.Web.Shared.Playlists;
    using MusicX.Web.Shared.Songs;
    using MusicX.Web.Shared.TelemetryData;

    public interface IApiClient
    {
        Task<ApiResponse<IndexListsResponseModel>> GetIndexLists();

        Task<ApiResponse<GetSongByIdResponse>> GetSongById(int id);

        Task<ApiResponse<SongsListResponseModel>> GetSongsList(int page, string searchTerms);

        Task<ApiResponse<GetSongsByIdsResponse>> GetSongsByIds(GetSongsByIdsRequest request);

        Task<ApiResponse<AddSongResponse>> AddSong(AddSongRequest request);

        Task<ApiResponse<AddSimilarSongsResponse>> AddSimilarSongs(AddSimilarSongsRequest request);

        Task<ApiResponse<GetSongsInPlaylistResponse>> GetSongsInPlaylist(int id);

        Task<ApiResponse<GetAllPlaylistResponse>> GetAllPlaylists();

        Task<ApiResponse<CreatePlaylistFromListResponse>> CreatePlaylistFromList(CreatePlaylistFromListRequest request);

        Task<ApiResponse<ApplicationStartResponseModel>> ApplicationStart();

        Task<ApiResponse<ApplicationStopResponseModel>> ApplicationStop(ApplicationStopRequestModel request);

        Task<ApiResponse<SongPlayTelemetryResponse>> TelemetrySongPlay(SongPlayTelemetryRequest request);

        Task<ApiResponse<UserRegisterResponseModel>> UserRegister(UserRegisterRequestModel request);

        Task<ApiResponse<UserLoginResponseModel>> UserLogin(UserLoginRequestModel request);
    }
}
