namespace MusicX.Data.Models
{
    using System.Collections.Generic;

    using MusicX.Data.Common.Models;

    public class Song : BaseDeletableModel<int>
    {
        public Song()
        {
            this.Artists = new HashSet<SongArtist>();
        }

        public string Name { get; set; }

        public virtual ICollection<SongArtist> Artists { get; set; }
    }
}
