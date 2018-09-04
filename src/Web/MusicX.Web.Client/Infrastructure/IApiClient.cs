namespace MusicX.Web.Client.Infrastructure
{
    using System.Threading.Tasks;

    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Account;
    using MusicX.Web.Shared.Application;
    using MusicX.Web.Shared.Songs;

    public interface IApiClient
    {
        Task<ApiResponse<SongsListResponseModel>> GetSongsList(int page);

        Task<ApiResponse<ApplicationStartResponseModel>> ApplicationStart();

        Task<ApiResponse<ApplicationStopResponseModel>> ApplicationStop(ApplicationStopRequestModel request);

        Task<ApiResponse<UserRegisterResponseModel>> UserRegister(UserRegisterRequestModel request);

        Task<ApiResponse<UserLoginResponseModel>> UserLogin(UserLoginRequestModel request);
    }
}
