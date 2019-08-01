namespace MusicX.Web.Client.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;

    using Microsoft.JSInterop;

    using MusicX.Web.Shared.Songs;
    using MusicX.Web.Shared.TelemetryData;

    // TODO: Make it async?
    // TODO: Do we need some locking?
    public class MediaPlayer : IMediaPlayer
    {
        private readonly Random random = new Random();

        private readonly IApplicationState applicationState;

        private readonly IApiClient apiClient;
        private readonly IJSRuntime jsRuntime;

        public MediaPlayer(IApplicationState applicationState, IApiClient apiClient, IJSRuntime jsRuntime)
        {
            this.applicationState = applicationState;
            this.apiClient = apiClient;
            this.jsRuntime = jsRuntime;
            this.Playlist = new List<MediaPlayerPlaylistItem>();
            this.CurrentIndexInThePlaylist = 0;
            JsInterop.PlayerEndedPlaybackEvent += this.PlayNext;
        }

        public event Action OnChange;

        public IList<MediaPlayerPlaylistItem> Playlist { get; }

        public MediaPlayerPlaylistItem CurrentSong =>
            this.CurrentIndexInThePlaylist >= this.Playlist.Count
                ? new MediaPlayerPlaylistItem()
                : this.Playlist[this.CurrentIndexInThePlaylist];

        public int CurrentIndexInThePlaylist { get; private set; }

        public bool Repeat { get; set; } = false;

        public bool Shuffle { get; set; } = true;

        public void Initialize()
        {
            this.jsRuntime.MediaPlayerInitialize();
        }

        public void ClearPlaylist()
        {
            this.CurrentIndexInThePlaylist = 0;
            this.Playlist.Clear();
            this.Change();
        }

        public void AddAndPlay(MediaPlayerPlaylistItem song)
        {
            var existingSong = this.Playlist.FirstOrDefault(x => x.Id == song.Id);
            if (existingSong != null)
            {
                // Song already in the playlist
                this.CurrentIndexInThePlaylist = this.Playlist.IndexOf(existingSong);
            }
            else
            {
                this.Playlist.Add(song);
                this.CurrentIndexInThePlaylist = this.Playlist.Count - 1;
            }

            this.Play(song, true);
        }

        public void AddAndPlay(SongListItem song)
        {
            this.AddAndPlay(new MediaPlayerPlaylistItem(song));
        }

        public void Add(SongListItem song)
        {
            if (this.Playlist.Any(x => x.Id == song.Id))
            {
                // Song already in the playlist
                return;
            }

            this.Playlist.Add(new MediaPlayerPlaylistItem(song));

            this.Change();
        }

        public void RemoveSong(MediaPlayerPlaylistItem song)
        {
            var songToRemove = this.Playlist.FirstOrDefault(x => x.Id == song.Id);
            if (songToRemove == null)
            {
                return;
            }

            var index = this.Playlist.IndexOf(songToRemove);
            this.Playlist.Remove(songToRemove);
            if (index < this.CurrentIndexInThePlaylist)
            {
                this.CurrentIndexInThePlaylist--;
            }

            this.Change();
        }

        public void PlayNext()
        {
            if (this.Playlist.Count == 0)
            {
                return;
            }

            var index = this.GetNextSongIndex();
            var song = this.Playlist[index];
            this.CurrentIndexInThePlaylist = index;
            this.Play(song, false);
        }

        private void Play(MediaPlayerPlaylistItem song, bool playedByUser)
        {
            this.jsRuntime.MediaPlayerSetSource(song.PlayableUrl);
            this.jsRuntime.MediaPlayerPlay();
            this.Change();
            try
            {
                this.apiClient.TelemetrySongPlay(
                    new SongPlayTelemetryRequest
                    {
                        SessionId = this.applicationState.SessionId, SongId = song.Id, PlayedByUser = playedByUser,
                    });
            }
            catch
            {
            }
        }

        private int GetNextSongIndex()
        {
            if (this.Repeat)
            {
                return this.CurrentIndexInThePlaylist;
            }

            if (this.Shuffle)
            {
                var nextSong = this.random.Next(0, this.Playlist.Count);
                while (this.Playlist.Count > 1 && nextSong == this.CurrentIndexInThePlaylist)
                {
                    nextSong = this.random.Next(0, this.Playlist.Count);
                }

                return nextSong;
            }
            else
            {
                return (this.CurrentIndexInThePlaylist + 1) % this.Playlist.Count;
            }
        }

        private void Change()
        {
            this.jsRuntime.StorageSave("NowPlayingSongs", JsonSerializer.Serialize(this.Playlist.Select(x => x.Id)));
            this.OnChange?.Invoke();
        }
    }
}
