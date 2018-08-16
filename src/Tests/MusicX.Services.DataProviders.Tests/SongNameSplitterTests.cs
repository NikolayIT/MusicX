namespace MusicX.Services.DataProviders.Tests
{
    using Xunit;

    public class SongNameSplitterTests
    {
        [Theory]
        [InlineData(1337, "Ginie Line", "Le Dilemme")]
        [InlineData(30745, "Zaz", "Je Veux")]
        public void GetArtistAndSongTitleShouldWorkCorrectly(int id, string expectedArtist, string expectedSongTitle)
        {
            var songsSearcher = new Top40ChartsDataProvider();

            var result = songsSearcher.GetArtistAndSongTitle(id);

            Assert.NotNull(result);
            Assert.Equal(expectedArtist, result.Artist);
            Assert.Equal(expectedSongTitle, result.Title);
        }
    }
}
