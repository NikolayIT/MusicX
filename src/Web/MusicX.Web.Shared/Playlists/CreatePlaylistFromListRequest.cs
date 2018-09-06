namespace MusicX.Web.Shared.Playlists
{
    using System.Collections.Generic;

    public class CreatePlaylistFromListRequest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<int> SongIds { get; set; }
    }
}
