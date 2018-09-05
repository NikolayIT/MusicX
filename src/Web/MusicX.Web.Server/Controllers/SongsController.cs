namespace MusicX.Web.Server.Controllers
{
    using System.Linq;

    using Microsoft.AspNetCore.Authorization;

    using MusicX.Common.Models;
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
            var response = new SongsListResponseModel
                           {
                               Count = this.songsService.CountSongs(
                                   song => song.Metadata.Any(x => x.Type == MetadataType.YouTubeVideoId)),
                               Page = page,
                               ItemsPerPage = 24
                           };
            var skip = (page - 1) * response.ItemsPerPage;
            var songs = this.songsService
                .GetSongsInfo(
                    song => song.Metadata.Any(x => x.Type == MetadataType.YouTubeVideoId),
                    song => song.Id,
                    skip,
                    response.ItemsPerPage).Select(
                    x => new SongListItem
                         {
                             Id = x.Id, SongName = x.ToString(), PlayableUrl = x.PlayableUrl, ImageUrl = x.ImageUrl, // TODO: Automapper
                         }).ToList();
            response.Songs = songs;
            return response.ToApiResponse();
        }
    }
}
