namespace MusicX.Web.Shared.Home
{
    using System.Collections.Generic;

    using MusicX.Web.Shared.Songs;

    public class IndexListsResponseModel
    {
        public IEnumerable<SongListItem> NewestSongs { get; set; }

        public IEnumerable<SongListItem> PopularSongs { get; set; }

        public IEnumerable<SongListItem> RandomSongs { get; set; }
    }
}
