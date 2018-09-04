namespace MusicX.Web.Client.Infrastructure
{
    using System;

    using MusicX.Web.Shared.Songs;

    public class MediaPlayer : IMediaPlayer
    {
        public event Action OnCurrentSongChanged;

        public string CurrentSongName { get; set; }

        public string CurrentSongImageUrl { get; set; }

        public void Initialize()
        {
            JsInterop.MediaPlayerInitialize();
        }

        public void Play()
        {
            JsInterop.MediaPlayerPlay();
        }

        public void Play(SongListItem song)
        {
            JsInterop.MediaPlayerSetSource(song.PlayableUrl);
            JsInterop.MediaPlayerPlay();
            this.CurrentSongName = song.SongName;
            this.CurrentSongImageUrl = song.ImageUrl;
            this.OnCurrentSongChanged?.Invoke();
        }
    }
}
