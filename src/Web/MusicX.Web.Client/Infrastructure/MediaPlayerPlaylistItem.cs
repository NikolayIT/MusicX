namespace MusicX.Web.Client.Infrastructure
{
    using MusicX.Web.Shared.Songs;

    public class MediaPlayerPlaylistItem
    {
        public MediaPlayerPlaylistItem()
        {
        }

        public MediaPlayerPlaylistItem(SongListItem songListItem)
        {
            this.Id = songListItem.Id;
            this.Title = songListItem.SongName;
            this.PlayableUrl = songListItem.PlayableUrl;
            this.ImageUrl = songListItem.ImageUrl;
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public string PlayableUrl { get; set; }
    }
}
