namespace MusicX.Services.DataProviders
{
    using System.Net.Http;

    using MusicX.Common;
    using MusicX.Common.Models;

    public class Top40ChartsDataProvider
    {
        private const string Top40ChartsSongLinksFormat = "http://top40-charts.com/song.php?sid={0}";

        private readonly HttpClient http;

        public Top40ChartsDataProvider()
        {
            this.http = new HttpClient();
        }

        public SongArtistAndTitle GetArtistAndSongTitle(int id)
        {
            var url = string.Format(Top40ChartsSongLinksFormat, id);

            var response = this.http.GetAsync(url).GetAwaiter().GetResult();
            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            var songTitle =
                responseContent.GetStringBetween(@"<td class=biggerblue height=30 valign=top>", "</td>")
                    .StripHtmlTags()
                    .Trim();

            var songArtist =
                responseContent.GetStringBetween(@"<td height=20 colspan=2 valign=top><font color=586973>", "</td>")
                    .StripHtmlTags()
                    .Trim();

            if (string.IsNullOrWhiteSpace(songTitle) || string.IsNullOrWhiteSpace(songArtist))
            {
                return null;
            }

            return new SongArtistAndTitle(songArtist, songTitle);
        }
    }
}
