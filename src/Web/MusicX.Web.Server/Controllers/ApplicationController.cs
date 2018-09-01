namespace MusicX.Web.Server.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using MusicX.Data.Models;
    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Application;

    [AllowAnonymous]
    public class ApplicationController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;

        public ApplicationController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        public ApiResponse<ApplicationStartResponseModel> Start()
        {
            return new ApplicationStartResponseModel { Username = this.User?.Identity?.Name }.ToApiResponse();
        }

        [HttpPost]
        public ApplicationStopResponseModel Stop(ApplicationStopRequestModel model)
        {
            return new ApplicationStopResponseModel();
        }
    }
}
