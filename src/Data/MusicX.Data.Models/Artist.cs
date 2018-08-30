namespace MusicX.Data.Models
{
    using System.Collections.Generic;

    using MusicX.Data.Common.Models;

    public class Artist : BaseDeletableModel<int>
    {
        public Artist()
        {
            this.Songs = new HashSet<SongArtist>();
        }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public virtual ICollection<SongArtist> Songs { get; set; }
    }
}
