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

        int CountSongs(Expression<Func<Song, bool>> predicate = null);

        IEnumerable<SongArtistsTitleAndMetadata> GetSongsInfo(
            Expression<Func<Song, bool>> predicate = null,
            Expression<Func<Song, object>> orderBySelector = null,
            int? skip = null,
            int? take = null);

        IEnumerable<SongArtistsTitleAndMetadata> GetRandomSongs(int count, Expression<Func<Song, bool>> predicate = null);

        int CreateSong(SongArtistsTitleAndMetadata songInfo, string sourceName, string sourceItemId);
    }
}
