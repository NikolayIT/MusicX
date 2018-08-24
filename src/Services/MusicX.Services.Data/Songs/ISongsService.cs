namespace MusicX.Services.Data.Songs
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using MusicX.Common.Models;
    using MusicX.Data.Models;

    public interface ISongsService
    {
        SongArtistsAndTitle GetSongInfo(int id);

        IEnumerable<SongArtistsAndTitle> GetSongsInfo(Expression<Func<Song, bool>> predicate);

        int CreateSong(SongArtistsAndTitle songInfo);
    }
}
