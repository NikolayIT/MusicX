namespace MusicX.Services.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    using MusicX.Common;
    using MusicX.Common.Models;

    public class Top40ChartsDataProvider
    {
        private const string SongInfoUrlFormat = "http://top40-charts.com/song.php?sid={0}";
        private const string SongVideoUrlFormat = "http://top40-charts.com/songs/media.php?sid={0}";

        private readonly HttpClient http;

        public Top40ChartsDataProvider()
        {
            this.http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        }

        public SongAttributes GetSong(int id)
        {
            var url = string.Format(SongInfoUrlFormat, id);
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

            var attributes = new SongAttributes
                             {
                                 [MetadataType.Title] = songTitle,
                                 [MetadataType.Artist] = songArtist,
                             };

            // YouTube
            url = string.Format(SongVideoUrlFormat, id);
            response = this.http.GetAsync(url).GetAwaiter().GetResult();
            responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (responseContent.Contains(" src=\"http://www.youtube.com/embed/")
                && responseContent.Contains("?autoplay=1&rel=0"))
            {
                var youTubeVideoId = responseContent.GetStringBetween(
                    " src=\"http://www.youtube.com/embed/",
                    "?autoplay=1&rel=0");
                attributes[MetadataType.YouTubeVideoId] = youTubeVideoId;
            }

            return attributes;
        }
    }
}
