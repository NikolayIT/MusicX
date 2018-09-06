namespace MusicX.Data.Models
{
    using System.Collections.Generic;

    using MusicX.Data.Common.Models;
    using MusicX.Data.Models.Interfaces;

    public class Playlist : BaseModel<int>, IHaveOrder, IHaveOwner
    {
        public Playlist()
        {
            this.Songs = new HashSet<PlaylistSong>();
        }

        public string Name { get; set; }

        public bool IsSystem { get; set; }

        public int Order { get; set; }

        public string OwnerId { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        public virtual ICollection<PlaylistSong> Songs { get; set; }
    }
}
