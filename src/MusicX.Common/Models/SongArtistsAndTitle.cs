namespace MusicX.Common.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class SongArtistsAndTitle
    {
        public SongArtistsAndTitle()
        {
        }

        public SongArtistsAndTitle(IList<string> artists, string title)
        {
            this.Artists = artists;
            this.Title = title;
        }

        public IList<string> Artists { get; set; }

        public string Artist => this.GetSongArtistName(this.Artists);

        public string Title { get; set; }

        public override string ToString()
        {
            return $"{this.Artist} - {this.Title}";
        }

        /// <summary>
        /// Gets the song artist name as a single string.
        /// </summary>
        /// <param name="artists">
        /// Already sorted list of artist names.
        /// </param>
        /// <returns>
        /// The artist name as a <see cref="string"/>.
        /// </returns>
        private string GetSongArtistName(IList<string> artists)
        {
            if (artists == null || !artists.Any())
            {
                return string.Empty;
            }

            if (artists.Count == 1)
            {
                return artists[0];
            }

            if (artists.Count == 2)
            {
                return $"{artists[0]} & {artists[1]}";
            }

            var firstArtists = string.Join(", ", artists.Take(artists.Count - 1));
            return $"{firstArtists} & {artists[artists.Count - 1]}";
        }
    }
}
