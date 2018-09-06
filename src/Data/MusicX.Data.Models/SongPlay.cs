namespace MusicX.Data.Models
{
    using MusicX.Data.Common.Models;
    using MusicX.Data.Models.Interfaces;

    public class SongPlay : BaseModel<int>, IHaveOwner
    {
        public int SongId { get; set; }

        public virtual Song Song { get; set; }

        public string SessionId { get; set; }

        public string OwnerId { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the song is played by the user or automatically.
        /// </summary>
        public bool PlayedByUser { get; set; }
    }
}
