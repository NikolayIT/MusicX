namespace MusicX.Services.DataProviders.Tests
{
    using Xunit;

    public class LyricsPluginDataProviderTests
    {
        [Theory]
        [InlineData("Morandi", "Angels", "crying")]
        [InlineData("Спенс", "Писна ми", "демокрация")]
        public void GetLyricsShouldWorkCorrectly(string artist, string songTitle, string stringThatShouldBeContained)
        {
            var lyricsSearcher = new LyricsPluginDataProvider();
            var lyrics = lyricsSearcher.GetLyrics(artist, songTitle);

            Assert.NotNull(lyrics);
            Assert.Contains(stringThatShouldBeContained, lyrics);
        }

        [Theory]
        [InlineData("0iutrc324n095832", "0iutrc324n095832")]
        public void GetLyricsShouldReturnNullWhenNoLyricsAreFound(string artist, string songTitle)
        {
            var lyricsSearcher = new LyricsPluginDataProvider();
            var lyrics = lyricsSearcher.GetLyrics(artist, songTitle);

            Assert.Null(lyrics);
        }
    }
}
