namespace MusicX.Services.Data
{
    using System.Collections.Generic;

    using MusicX.Common.Models;

    public interface ISongNameSplitter
    {
        SongArtistsAndTitle Split(string inputString);

        SongArtistsAndTitle SplitSongName(string artistAndSongName);

        IList<string> SplitArtistName(string inputString);
    }
}
