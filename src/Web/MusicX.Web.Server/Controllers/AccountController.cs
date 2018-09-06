namespace MusicX.Web.Server.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using MusicX.Common;
    using MusicX.Data.Models;
    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Account;

    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<ApiResponse<UserRegisterResponseModel>> Register([FromBody]UserRegisterRequestModel model)
        {
            if (model == null || !this.ModelState.IsValid)
            {
                return this.ModelStateErrors<UserRegisterResponseModel>();
            }

            var user = new ApplicationUser { Email = model.Email, UserName = model.Email };
            user.Playlists.Add(new Playlist { IsSystem = true, Name = PlaylistsConstants.CurrentPlaylistName });
            user.Playlists.Add(new Playlist { IsSystem = true, Name = PlaylistsConstants.LikesPlaylistName });

            var result = await this.userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return this.GetIdentityApiErrors<UserRegisterResponseModel>(result);
            }

            return new UserRegisterResponseModel { Id = user.Id }.ToApiResponse();
        }

        private ApiResponse<T> GetIdentityApiErrors<T>(IdentityResult identityResult)
        {
            return new ApiResponse<T>(identityResult.Errors.Select(x => new ApiError(x.Code, x.Description)).ToList());
        }
    }
}
