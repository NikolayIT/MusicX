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

        IEnumerable<SongArtistsTitleAndMetadata> GetSongsInfo(Expression<Func<Song, bool>> predicate);

        int CreateSong(SongArtistsTitleAndMetadata songInfo);
    }
}
