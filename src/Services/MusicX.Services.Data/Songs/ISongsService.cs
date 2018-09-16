namespace MusicX.Services.Data.Songs
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using MusicX.Common.Models;
    using MusicX.Data.Models;

    public interface ISongsService
    {
        int CountSongs(Expression<Func<Song, bool>> predicate = null);

        IEnumerable<SongArtistsTitleAndMetadata> GetSongsInfo(
            Expression<Func<Song, bool>> predicate = null,
            Expression<Func<Song, object>> orderBySelector = null,
            int? skip = null,
            int? take = null);

        IEnumerable<SongArtistsTitleAndMetadata> GetRandomSongs(int count, Expression<Func<Song, bool>> predicate = null);

        Task<int> CreateSongAsync(string songTitle, IList<string> songArtists, string sourceName, string sourceItemId);

        Task UpdateSongsSystemDataAsync(int songId);
    }
}
