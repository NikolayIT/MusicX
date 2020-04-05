namespace MusicX.Web.Server.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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
        public async Task<ApiResponse<CreatePlaylistFromListResponse>> CreateFromList([FromBody]CreatePlaylistFromListRequest request)
        {
            Playlist playlist;
            var currentSongIds = new HashSet<int>();
            var lastOrder = 0;

            if (request.Id == 0)
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return this.Error<CreatePlaylistFromListResponse>(
                        "Name",
                        "Please select the name of your new playlist.");
                }

                var name = request.Name.Trim();

                if (this.playlistsRepository.All().Any(x => x.Name == name && x.OwnerId == this.User.GetId()))
                {
                    return this.Error<CreatePlaylistFromListResponse>(
                        "Name",
                        "You already have playlist with that name. Please select a different name.");
                }

                playlist = new Playlist { Name = name, IsSystem = false, OwnerId = this.User.GetId() };
                await this.playlistsRepository.AddAsync(playlist);
            }
            else
            {
                playlist = this.playlistsRepository.All()
                    .FirstOrDefault(x => x.Id == request.Id && x.OwnerId == this.User.GetId());

                if (playlist == null)
                {
                    return this.Error<CreatePlaylistFromListResponse>(
                        "Id",
                        "Playlist not found.");
                }

                currentSongIds = this.playlistsRepository.All().Where(x => x.Id == request.Id)
                    .SelectMany(x => x.Songs.Select(s => s.SongId)).ToList().ToHashSet();
                if (currentSongIds.Count > 0)
                {
                    lastOrder = this.playlistsRepository.All().Where(x => x.Id == request.Id).SelectMany(x => x.Songs)
                        .Max(s => s.Order);
                }
            }

            foreach (var songId in request.SongIds)
            {
                if (!currentSongIds.Contains(songId))
                {
                    playlist.Songs.Add(new PlaylistSong { SongId = songId, Order = ++lastOrder });
                    currentSongIds.Add(songId);
                }
            }

            await this.playlistsRepository.SaveChangesAsync();

            var response = new CreatePlaylistFromListResponse();
            return response.ToApiResponse();
        }

        [HttpGet]
        public ApiResponse<GetAllPlaylistResponse> GetAll()
        {
            var playlists = this.playlistsRepository.All().Where(x => !x.IsSystem && x.OwnerId == this.User.GetId())
                .OrderBy(x => x.Order).ThenBy(x => x.Id).Select(x => new PlaylistInfo { Id = x.Id, Name = x.Name, SongsCount = x.Songs.Count, });
            var response = new GetAllPlaylistResponse { Playlists = playlists };
            return response.ToApiResponse();
        }
    }
}
