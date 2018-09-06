namespace MusicX.Web.Shared.Playlists
{
    using System.Collections.Generic;

    public class GetAllPlaylistResponse
    {
        public IEnumerable<PlaylistInfo> Playlists { get; set; }
    }
}
