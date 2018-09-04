namespace MusicX.Web.Server.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Authorization;

    using MusicX.Services.Data.Songs;
    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Songs;

    [AllowAnonymous]
    public class SongsController : BaseController
    {
        private readonly ISongsService songsService;

        public SongsController(ISongsService songsService)
        {
            this.songsService = songsService;
        }

        public ApiResponse<SongsListResponseModel> GetList(int page = 1)
        {
            var songs = this.songsService.GetSongsInfo(song => true).Select(
                x => new SongListItem { SongName = x.ToString(), PlayableUrl = x.PlayableUrl }).ToList();
            var response = new SongsListResponseModel { Count = songs.Count, Page = page, ItemsPerPage = 50 };
            response.Songs = songs.Skip((page - 1) * response.ItemsPerPage).Take(response.ItemsPerPage);
            return response.ToApiResponse();
        }
    }
}
