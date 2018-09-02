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

        public ApiResponse<IEnumerable<SongListItem>> GetList()
        {
            var songs = this.songsService.GetSongsInfo(song => true).Select(
                x => new SongListItem { SongName = x.ToString(), PlayableUrl = x.PlayableUrl });
            return songs.ToApiResponse();
        }
    }
}
