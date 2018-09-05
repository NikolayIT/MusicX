namespace MusicX.Web.Server.Controllers
{
    using System.IO;
    using System.Reflection;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Application;

    [AllowAnonymous]
    public class ApplicationController : BaseController
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public ApplicationController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public ApiResponse<ApplicationStartResponseModel> Start()
        {
            return new ApplicationStartResponseModel
                   {
                       Username = this.User?.Identity?.Name,
                       VersionBuiltOn =
                           new FileInfo(Assembly.GetEntryAssembly().Location).LastWriteTime.ToUniversalTime(),
                       EnvironmentName = this.hostingEnvironment.EnvironmentName,
                   }.ToApiResponse();
        }

        [HttpPost]
        public ApplicationStopResponseModel Stop([FromBody]ApplicationStopRequestModel model)
        {
            return new ApplicationStopResponseModel();
        }
    }
}
