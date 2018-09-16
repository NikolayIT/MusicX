namespace MusicX.Web.Shared.Songs
{
    using System.ComponentModel.DataAnnotations;

    public class AddSongRequest
    {
        [Required]
        [MinLength(2)]
        public string SongName { get; set; }

        [Required]
        [MinLength(2)]
        public string Artists { get; set; }
    }
}
