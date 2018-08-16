namespace MusicX.Services.Data.Songs
{
    using System.Collections.Generic;

    using MusicX.Common.Models;

    public interface ISongNameSplitter
    {
        SongArtistsAndTitle Split(string inputString);

        SongArtistAndTitle SplitSongName(string artistAndSongName);

        IList<string> SplitArtistName(string inputString);
    }
}
