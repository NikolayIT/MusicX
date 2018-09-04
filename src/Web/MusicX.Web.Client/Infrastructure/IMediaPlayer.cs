namespace MusicX.Web.Client.Infrastructure
{
    using System;

    using MusicX.Web.Shared.Songs;

    public interface IMediaPlayer
    {
        event Action OnCurrentSongChanged;

        string CurrentSongName { get; set; }

        string CurrentSongImageUrl { get; set; }

        void Initialize();

        void Play();

        void Play(SongListItem song);
    }
}
