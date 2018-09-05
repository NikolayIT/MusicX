namespace MusicX.Web.Server.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;
    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Playlists;

    [AllowAnonymous]
    public class PlaylistsController : BaseController
    {
        private readonly IRepository<Playlist> playlistsRepository;

        public PlaylistsController(IRepository<Playlist> playlistsRepository)
        {
            this.playlistsRepository = playlistsRepository;
        }

        public ApiResponse<CreatePlaylistFromListResponse> CreateFromList([FromBody]CreatePlaylistFromListRequest request)
        {
            var response = new CreatePlaylistFromListResponse();
            return response.ToApiResponse();
        }
    }
}
