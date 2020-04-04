namespace MusicX.Services.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Text;

    using MusicX.Common;

    public class LyricsPluginDataProvider : ILyricsPluginDataProvider
    {
        private const string BaseUrl = "http://www.lyricsplugin.com/";
        private const string UserAgent = "Lyrics Plugin/0.4 (Winamp build)";

        private string sid;
        private HttpClient http;
        private string secretCode;
        private bool initialized;

        /// <summary>
        /// Returns lyrics by given artist and title.
        /// </summary>
        /// <param name="artist">The artist of the song.</param>
        /// <param name="title">The title of the song.</param>
        /// <returns>Returns lyrics if found, or return null if not found.</returns>
        public string GetLyrics(string artist, string title)
        {
            this.Initialize();
            artist = artist.Trim();
            title = title.Trim();
            var htmlResult = this.AskForLyrics(artist, title);
            var lyrics = this.ParseLyricsFromHTML(htmlResult);
            if (string.IsNullOrWhiteSpace(lyrics))
            {
                return null;
            }

            return lyrics;
        }

        private static string GetCurrentTimestamp()
        {
            return ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString(CultureInfo.InvariantCulture);
        }

        private void Initialize()
        {
            if (this.initialized)
            {
                return;
            }

            this.initialized = true;
            this.sid = (DateTime.Now.Ticks + "magic unicorns").ToMd5Hash();
            this.http = new HttpClient();

            this.StartSessionAndSetSecretCode();

            // Default headers
            this.http.DefaultRequestHeaders.Clear();
            this.http.DefaultRequestHeaders.Add("UserAgent", UserAgent);
            this.http.DefaultRequestHeaders.Add(
                "Accept",
                "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*");
            this.http.DefaultRequestHeaders.Add("AcceptLanguage", "bg-BG");
            this.http.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            this.http.DefaultRequestHeaders.Add("CacheControl", "no-cache");
            this.http.DefaultRequestHeaders.Add("ProtocolVersion", "1.1");
        }

        private string AskForLyrics(string artist, string title)
        {
            try
            {
                var currentTimestamp = GetCurrentTimestamp();
                var pid = this.GetPid(currentTimestamp, artist, title);
                var postData = new List<KeyValuePair<string, string>>
                               {
                                   new KeyValuePair<string, string>("a", artist),
                                   new KeyValuePair<string, string>("t", title),
                                   new KeyValuePair<string, string>("i", currentTimestamp),
                                   new KeyValuePair<string, string>("pid", pid),
                                   new KeyValuePair<string, string>("sid", this.sid),
                                   new KeyValuePair<string, string>("bc", "526602"),
                                   new KeyValuePair<string, string>("tc", "9678777"),
                               };
                var result = this.RequestPost(BaseUrl + "plugin/0.4/winamp/plugin.php", postData);
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string ParseLyricsFromHTML(string html)
        {
            if (!html.Contains("<div id=\"lyrics\">"))
            {
                return string.Empty;
            }

            return html.GetStringBetween("<div id=\"lyrics\">", "</div>").Replace("<br />", string.Empty).Trim();
        }

        private void StartSessionAndSetSecretCode()
        {
            try
            {
                this.http.DefaultRequestHeaders.Clear();
                this.http.DefaultRequestHeaders.Add("UserAgent", UserAgent);
                this.http.DefaultRequestHeaders.Add("ProtocolVersion", "1.0");
                this.http.DefaultRequestHeaders.Add("Accept", string.Empty);
                this.http.DefaultRequestHeaders.Add("Connection", string.Empty);
                var postData = new List<KeyValuePair<string, string>>
                               {
                                   new KeyValuePair<string, string>("sid", this.sid),
                                   new KeyValuePair<string, string>("i", "wa"),
                                   new KeyValuePair<string, string>("v", "5060"),
                                   new KeyValuePair<string, string>("m", "0"),
                               };
                this.secretCode = this.RequestPost(BaseUrl + "plugin/session.php", postData);
            }
            catch (Exception)
            {
                // Default value for the secret code
                this.secretCode = "0123456789abcdef0123456789abcdef";
            }
        }

        private string RequestPost(string url, IEnumerable<KeyValuePair<string, string>> data)
        {
            var response = this.http.PostAsync(url, new FormUrlEncodedContent(data)).GetAwaiter().GetResult();
            var byteArray = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
            return responseString;
        }

        private string GetPid(string timestamp, string artist, string title)
        {
            var md5OfTimestamp = timestamp.ToMd5Hash();
            var md5OfTimestampParts = new string[8];
            for (var i = 0; i < 8; i++)
            {
                md5OfTimestampParts[i] = md5OfTimestamp.Substring(i * 4, 4);
            }

            var secretCodeEncrypted = this.secretCode.ToMd5Hash().ToMd5Hash();
            var secretCodePart0 = secretCodeEncrypted.Substring(0, 16);
            var secretCodePart1 = secretCodeEncrypted.Substring(16);
            return
                string.Format(
                    "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}",
                    md5OfTimestampParts[0],
                    secretCodePart0,
                    md5OfTimestampParts[2],
                    artist,
                    md5OfTimestampParts[4],
                    title,
                    md5OfTimestampParts[5],
                    this.sid,
                    md5OfTimestampParts[3],
                    secretCodePart1,
                    md5OfTimestampParts[1]).ToMd5Hash();
        }
    }
}
