namespace MusicX.Data.Models
{
    using System.Collections.Generic;

    using MusicX.Data.Common.Models;

    public class Song : BaseDeletableModel<int>, IHaveSource
    {
        public Song()
        {
            this.Artists = new HashSet<SongArtist>();
            this.MediaLinks = new HashSet<MediaLink>();
        }

        public string Name { get; set; }

        public virtual ICollection<SongArtist> Artists { get; set; }

        public virtual ICollection<MediaLink> MediaLinks { get; set; }

        public int? SourceId { get; set; }

        public virtual Source Source { get; set; }
    }
}
