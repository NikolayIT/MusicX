namespace MusicX.Web.Shared.Songs
{
    using System.Collections.Generic;

    public class GetSongsByIdsRequest
    {
        public IEnumerable<int> SongIds { get; set; }
    }
}
