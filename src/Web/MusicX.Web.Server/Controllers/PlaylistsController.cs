namespace MusicX.Web.Server.Controllers
{
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;

    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;
    using MusicX.Web.Server.Infrastructure;
    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Playlists;

    public class PlaylistsController : BaseController
    {
        private readonly IRepository<Playlist> playlistsRepository;

        public PlaylistsController(IRepository<Playlist> playlistsRepository)
        {
            this.playlistsRepository = playlistsRepository;
        }

        [HttpPost]
        public ApiResponse<CreatePlaylistFromListResponse> CreateFromList([FromBody]CreatePlaylistFromListRequest request)
        {
            var response = new CreatePlaylistFromListResponse();
            return response.ToApiResponse();
        }

        [HttpGet]
        public ApiResponse<GetAllPlaylistResponse> GetAll()
        {
            var playlists = this.playlistsRepository.All().Where(x => !x.IsSystem && x.OwnerId == this.User.GetId())
                .OrderBy(x => x.Order).Select(x => new PlaylistInfo { Id = x.Id, Name = x.Name, SongsCount = x.Songs.Count, });
            var response = new GetAllPlaylistResponse { Playlists = playlists };
            return response.ToApiResponse();
        }
    }
}
