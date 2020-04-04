namespace MusicX.Services.DataProviders
{
    public interface ILyricsPluginDataProvider
    {
        /// <summary>
        /// Returns lyrics by given artist and title.
        /// </summary>
        /// <param name="artist">The artist of the song.</param>
        /// <param name="title">The title of the song.</param>
        /// <returns>Returns lyrics if found, or return null if not found.</returns>
        string GetLyrics(string artist, string title);
    }
}
