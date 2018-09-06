namespace MusicX.Web.Shared.Songs
{
    using System.Collections.Generic;

    public class GetSongsInPlaylistResponse
    {
        public IEnumerable<SongListItem> Songs { get; set; }
    }
}
