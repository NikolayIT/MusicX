namespace MusicX.Services.DataProviders.Tests
{
    using MusicX.Common.Models;

    using Xunit;

    public class SongNameSplitterTests
    {
        [Theory]
        [InlineData(1337, "Ginie Line", "Le Dilemme")]
        [InlineData(30745, "Zaz", "Je Veux")]
        public void GetArtistAndSongTitleShouldWorkCorrectly(int id, string expectedArtist, string expectedSongTitle)
        {
            var songsSearcher = new Top40ChartsDataProvider();

            var result = songsSearcher.GetSong(id);

            Assert.NotNull(result);
            Assert.Equal(expectedArtist, result[MetadataType.Artist]);
            Assert.Equal(expectedSongTitle, result[MetadataType.Title]);
        }
    }
}
