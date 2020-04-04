namespace MusicX.Web.Server.Controllers
{
    using System.Linq;

    using Microsoft.AspNetCore.Authorization;

    using MusicX.Common.Models;
    using MusicX.Services.Data;
    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Home;
    using MusicX.Web.Shared.Songs;

    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly ISongsService songsService;

        public HomeController(ISongsService songsService)
        {
            this.songsService = songsService;
        }

        public ApiResponse<IndexListsResponseModel> GetIndexLists()
        {
            var response = new IndexListsResponseModel
                           {
                               NewestSongs =
                                   this.songsService
                                       .GetSongsInfo(
                                           song => song.Metadata.Any(x => x.Type == SongMetadataType.YouTubeVideoId),
                                           song => -song.Id,
                                           null,
                                           8).Select(
                                           x => new SongListItem
                                                {
                                                    Id = x.Id,
                                                    SongName = x.ToString(),
                                                    PlayableUrl = x.PlayableUrl,
                                                    ImageUrl = x.ImageUrl
                                                }).ToList(),
                               RandomSongs = this.songsService
                                   .GetRandomSongs(
                                       8,
                                       song => song.Metadata.Any(x => x.Type == SongMetadataType.YouTubeVideoId)).Select(
                                       x => new SongListItem
                                            {
                                                Id = x.Id,
                                                SongName = x.ToString(),
                                                PlayableUrl = x.PlayableUrl,
                                                ImageUrl = x.ImageUrl
                                            }).ToList(),
                           };
            return response.ToApiResponse();
        }
    }
}
