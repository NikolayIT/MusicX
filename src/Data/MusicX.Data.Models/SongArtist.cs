namespace MusicX.Data.Models
{
    using MusicX.Data.Common.Models;
    using MusicX.Data.Models.Interfaces;

    public class SongArtist : BaseDeletableModel<int>, IHaveOrder
    {
        public int SongId { get; set; }

        public virtual Song Song { get; set; }

        public int ArtistId { get; set; }

        public virtual Artist Artist { get; set; }

        public int Order { get; set; }
    }
}
