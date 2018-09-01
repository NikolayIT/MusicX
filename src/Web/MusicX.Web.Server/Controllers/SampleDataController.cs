namespace MusicX.Web.Server.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using MusicX.Services.Data.Songs;
    using MusicX.Web.Shared;

    [AllowAnonymous]
    public class SampleDataController : BaseController
    {
        private readonly ISongsService songsService;

        public SampleDataController(ISongsService songsService)
        {
            this.songsService = songsService;
        }

        [HttpGet]
        public ApiResponse<IEnumerable<WeatherForecast>> WeatherForecasts()
        {
            var songs = this.songsService.GetSongsInfo(song => true);
            return songs.Select(x => new WeatherForecast { Summary = x.ToString() }).ToApiResponse();
        }
    }
}
