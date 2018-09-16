namespace MusicX.Web.Shared.Songs
{
    public class GetSongByIdResponse : SongListItem
    {
        public string SongTitle { get; set; }

        public string Artists { get; set; }

        public string Lyrics { get; set; }
    }
}
