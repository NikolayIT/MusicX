namespace MusicX.Web.Shared.Songs
{
    using System.Collections.Generic;

    public class GetSongsByIdsResponse
    {
        public IEnumerable<SongListItem> Songs { get; set; }
    }
}
