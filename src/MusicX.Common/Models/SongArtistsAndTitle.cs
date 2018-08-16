namespace MusicX.Common.Models
{
    using System.Collections.Generic;

    public class SongArtistsAndTitle
    {
        public IEnumerable<string> Artist { get; set; }

        public string Title { get; set; }
    }
}
