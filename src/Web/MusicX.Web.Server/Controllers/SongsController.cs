namespace MusicX.Web.Server.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using MusicX.Common;
    using MusicX.Common.Models;
    using MusicX.Data.Models;
    using MusicX.Services.Data.Songs;
    using MusicX.Services.DataProviders;
    using MusicX.Web.Server.Infrastructure;
    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Songs;

    [AllowAnonymous]
    public class SongsController : BaseController
    {
        private readonly ISongsService songsService;

        private readonly ISongMetadataService songMetadataService;

        public SongsController(ISongsService songsService, ISongMetadataService songMetadataService)
        {
            this.songsService = songsService;
            this.songMetadataService = songMetadataService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ApiResponse<AddSongResponse>> AddSong([FromBody]AddSongRequest request)
        {
            if (request == null || !this.ModelState.IsValid)
            {
                return this.ModelStateErrors<AddSongResponse>();
            }

            var splitter = new SongNameSplitter(); // TODO: Move to constructor
            var artists = splitter.SplitArtistName(request.Artists).ToList();
            var songId = await this.songsService.CreateSongAsync(request.SongName, artists, SourcesNames.User, this.User.GetId());
            var song = new SongArtistsAndTitle(artists, request.SongName);

            // Find video if available
            var youTubeDataProvider = new YouTubeDataProvider(); // TODO: Move to constructor
            var videoId = youTubeDataProvider.SearchVideo(string.Join(" ", song.Artists), song.Title);
            if (videoId != null)
            {
                await this.songMetadataService.AddMetadataInfoAsync(
                    songId,
                    new SongAttributes(SongMetadataType.YouTubeVideoId, videoId),
                    SourcesNames.YouTube,
                    null);
            }

            // Find lyrics if available
            var lyricsPluginDataProvider = new LyricsPluginDataProvider(); // TODO: Move to constructor
            var lyrics = lyricsPluginDataProvider.GetLyrics(song.Artist, song.Title);
            if (!string.IsNullOrWhiteSpace(lyrics))
            {
                await this.songMetadataService.AddMetadataInfoAsync(
                    songId,
                    new SongAttributes(SongMetadataType.Lyrics, lyrics),
                    SourcesNames.LyricsPlugin,
                    null);
            }

            await this.songsService.UpdateSongsSystemDataAsync(songId);

            return new AddSongResponse { Id = songId, SongTitle = song.ToString() }.ToApiResponse();
        }

        public ApiResponse<GetSongByIdResponse> GetById(int id)
        {
            var song = this.songsService.GetSongsInfo(x => x.Id == id).Select(
                x => new GetSongByIdResponse
                     {
                         Id = x.Id,
                         SongName = x.ToString(),
                         PlayableUrl = x.PlayableUrl,
                         ImageUrl = x.ImageUrl, // TODO: Automapper
                         Artists = x.Artist,
                         SongTitle = x.Title,
                         Lyrics = x.SongAttributes[SongMetadataType.Lyrics],
                     }).FirstOrDefault();
            return song.ToApiResponse();
        }

        public ApiResponse<SongsListResponseModel> GetList(string searchTerms = null, int page = 1)
        {
            Expression<Func<Song, bool>> searchExpression =
                song => song.Metadata.Any(x => x.Type == SongMetadataType.YouTubeVideoId);

            if (!string.IsNullOrWhiteSpace(searchTerms))
            {
                var words = searchTerms.Split(' ');
                foreach (var word in words)
                {
                    searchExpression = searchExpression.AndAlso(song => song.SearchTerms.Contains(word));
                }
            }

            var response = new SongsListResponseModel
                           {
                               Count = this.songsService.CountSongs(searchExpression),
                               Page = page,
                               ItemsPerPage = 24
                           };
            var skip = (page - 1) * response.ItemsPerPage;

            var songs = this.songsService
                .GetSongsInfo(
                    searchExpression,
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

        [HttpGet]
        public ApiResponse<GetSongsInPlaylistResponse> GetSongsInPlaylist(int id)
        {
            var songs = this.songsService
                .GetSongsInfo(
                    song => song.Playlists.Any(x => x.PlaylistId == id && x.Playlist.OwnerId == this.User.GetId()))
                .Select(
                    x => new SongListItem
                         {
                             Id = x.Id,
                             SongName = x.ToString(),
                             PlayableUrl = x.PlayableUrl,
                             ImageUrl = x.ImageUrl, // TODO: Automapper
                         }).ToList();
            var response = new GetSongsInPlaylistResponse { Songs = songs };
            return response.ToApiResponse();
        }

        [HttpPost]
        public ApiResponse<GetSongsByIdsResponse> GetSongsByIds([FromBody]GetSongsByIdsRequest request)
        {
            var songIds = request?.SongIds?.Take(500) ?? new List<int>();
            var songs = this.songsService.GetSongsInfo(song => songIds.Contains(song.Id)).Select(
                x => new SongListItem
                     {
                         Id = x.Id,
                         SongName = x.ToString(),
                         PlayableUrl = x.PlayableUrl,
                         ImageUrl = x.ImageUrl, // TODO: Automapper
                     }).ToList();
            var response = new GetSongsByIdsResponse { Songs = songs };
            return response.ToApiResponse();
        }
    }
}
