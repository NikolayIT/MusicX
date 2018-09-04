namespace MusicX.Web.Shared.Songs
{
    using System.Collections.Generic;

    public class SongsListResponseModel : PaginatedResponse
    {
        public IEnumerable<SongListItem> Songs { get; set; }
    }
}
