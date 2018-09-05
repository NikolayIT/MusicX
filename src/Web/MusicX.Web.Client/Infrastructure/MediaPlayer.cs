namespace MusicX.Web.Client.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MusicX.Web.Shared.Songs;

    // TODO: Do we need some locking?
    public class MediaPlayer : IMediaPlayer
    {
        private readonly Random random = new Random();

        public MediaPlayer()
        {
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
            JsInterop.MediaPlayerInitialize();
        }

        public void AddAndPlay(MediaPlayerPlaylistItem song)
        {
            var existingSong =
                this.Playlist.FirstOrDefault(x => x.Title == song.Title && x.PlayableUrl == song.PlayableUrl);
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

            JsInterop.MediaPlayerSetSource(song.PlayableUrl);
            JsInterop.MediaPlayerPlay();
            this.OnChange?.Invoke();
        }

        public void AddAndPlay(SongListItem song)
        {
            this.AddAndPlay(new MediaPlayerPlaylistItem(song));
        }

        public void Add(SongListItem song)
        {
            if (this.Playlist.Any(x => x.Title == song.SongName && x.PlayableUrl == song.PlayableUrl))
            {
                // Song already in the playlist
                return;
            }

            this.Playlist.Add(new MediaPlayerPlaylistItem(song));
            if (this.Playlist.Count == 1)
            {
                JsInterop.MediaPlayerSetSource(song.PlayableUrl);
            }

            this.OnChange?.Invoke();
        }

        public void RemoveSong(MediaPlayerPlaylistItem song)
        {
            var songToRemove = this.Playlist.FirstOrDefault(x => x.Title == song.Title && x.PlayableUrl == song.PlayableUrl);
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

            this.OnChange?.Invoke();
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
            JsInterop.MediaPlayerSetSource(song.PlayableUrl);
            JsInterop.MediaPlayerPlay();
            this.OnChange?.Invoke();
        }

        private int GetNextSongIndex()
        {
            if (this.Repeat)
            {
                return this.CurrentIndexInThePlaylist;
            }

            if (this.Shuffle)
            {
                // TODO: when songsCount > 1 do not repeat the song
                return this.random.Next(0, this.Playlist.Count);
            }
            else
            {
                return (this.CurrentIndexInThePlaylist + 1) % this.Playlist.Count;
            }
        }
    }
}
