namespace MusicX.Data.Models
{
    using MusicX.Data.Common.Models;

    public class SongArtist : BaseDeletableModel<int>
    {
        public int SongId { get; set; }

        public virtual Song Song { get; set; }

        public int ArtistId { get; set; }

        public virtual Artist Artist { get; set; }

        public int Order { get; set; }
    }
}
