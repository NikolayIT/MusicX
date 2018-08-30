namespace MusicX.Data.Models
{
    using MusicX.Data.Common.Models;
    using MusicX.Data.Models.Interfaces;

    public class PlaylistSong : BaseModel<int>, IHaveOrder
    {
        public int PlaylistId { get; set; }

        public virtual Playlist Playlist { get; set; }

        public int SongId { get; set; }

        public virtual Song Song { get; set; }

        public int Order { get; set; }
    }
}
