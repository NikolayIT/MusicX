namespace MusicX.Web.Client.Infrastructure
{
    using System;
    using System.Collections.Generic;

    using MusicX.Web.Shared.Songs;

    public interface IMediaPlayer
    {
        event Action OnChange;

        IList<MediaPlayerPlaylistItem> Playlist { get; }

        MediaPlayerPlaylistItem CurrentSong { get; }

        int CurrentIndexInThePlaylist { get; }

        void Initialize();

        void ClearPlaylist();

        void AddAndPlay(MediaPlayerPlaylistItem song);

        void AddAndPlay(SongListItem song);

        void Add(SongListItem song);

        void RemoveSong(MediaPlayerPlaylistItem song);

        void PlayNext();
    }
}
