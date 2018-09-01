namespace MusicX.Web.Server.Controllers
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using MusicX.Web.Shared;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseController : Controller
    {
        protected ApiResponse<T> Error<T>(string item, string message)
        {
            return new ApiResponse<T>(new ApiError(item, message));
        }

        protected ApiResponse<T> ModelStateErrors<T>()
        {
            if (this.ModelState == null || this.ModelState.Count == 0)
            {
                return new ApiResponse<T>(new ApiError("Model", "Empty or null model."));
            }

            var errors = new List<ApiError>();
            foreach (var item in this.ModelState)
            {
                foreach (var error in item.Value.Errors)
                {
                    errors.Add(new ApiError(item.Key, error.ErrorMessage));
                }
            }

            return new ApiResponse<T>(errors);
        }
    }
}
