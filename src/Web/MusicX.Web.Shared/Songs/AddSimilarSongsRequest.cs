namespace MusicX.Web.Shared.Songs
{
    using System.ComponentModel.DataAnnotations;

    public class AddSimilarSongsRequest
    {
        [Required]
        public int SongId { get; set; }
    }
}
