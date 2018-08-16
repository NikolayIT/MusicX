namespace MusicX.Data.Models
{
    using MusicX.Data.Common.Models;

    public class MediaLink : BaseDeletableModel<int>, IHaveSource
    {
        public int SongId { get; set; }

        public virtual Song Song { get; set; }

        public string Url { get; set; }

        public MediaType Type { get; set; }

        public int? SourceId { get; set; }

        public virtual Source Source { get; set; }
    }
}
