namespace MusicX.Data.Models
{
    using System.Collections.Generic;

    using MusicX.Data.Common.Models;
    using MusicX.Data.Models.Interfaces;

    public class Song : BaseDeletableModel<int>, IHaveSource
    {
        public Song()
        {
            this.Artists = new HashSet<SongArtist>();
            this.Metadata = new HashSet<SongMetadata>();
        }

        public string Name { get; set; }

        public virtual ICollection<SongArtist> Artists { get; set; }

        public virtual ICollection<SongMetadata> Metadata { get; set; }

        public virtual ICollection<PlaylistSong> Playlists { get; set; }

        public int? SourceId { get; set; }

        public virtual Source Source { get; set; }

        public string SourceItemId { get; set; }

        public string SearchTerms { get; set; }
    }
}
