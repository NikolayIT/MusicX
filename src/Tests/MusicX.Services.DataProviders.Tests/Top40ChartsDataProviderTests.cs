namespace MusicX.Services.DataProviders.Tests
{
    using MusicX.Common.Models;

    using Xunit;

    public class Top40ChartsDataProviderTests
    {
        [Theory]
        [InlineData(7424, "Blazin' Squad", "We Just Be Dreamin'")]
        [InlineData(48318, "Drake", "In My Feelings")]
        public void GetSongShouldWorkCorrectly(int id, string expectedArtist, string expectedSongTitle)
        {
            var songsSearcher = new Top40ChartsDataProvider();

            var result = songsSearcher.GetSong(id);

            Assert.NotNull(result);
            Assert.Equal(expectedArtist, result[SongMetadataType.Artist]);
            Assert.Equal(expectedSongTitle, result[SongMetadataType.Title]);
            Assert.NotNull(result[SongMetadataType.YouTubeVideoId]);
            Assert.NotNull(result[SongMetadataType.Lyrics]);
        }
    }
}
