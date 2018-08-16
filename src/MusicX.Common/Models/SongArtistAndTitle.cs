namespace MusicX.Common.Models
{
    public class SongArtistAndTitle
    {
        public SongArtistAndTitle(string songArtist, string songTitle)
        {
            this.Artist = songArtist;
            this.Title = songTitle;
        }

        public string Artist { get; set; }

        public string Title { get; set; }

        public override string ToString()
        {
            return $"{this.Artist} - {this.Title}";
        }
    }
}
