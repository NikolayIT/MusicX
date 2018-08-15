namespace MusicX.Services.Data.Songs
{
    using System.Collections.Generic;

    public interface ISongNameSplitter
    {
        (IEnumerable<string> Artists, string Name) Split(string inputString);

        (string Artist, string Name) SplitSongName(string artistAndSongName);

        IEnumerable<string> SplitArtistName(string inputString);
    }
}
