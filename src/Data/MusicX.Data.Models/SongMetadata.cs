namespace MusicX.Data.Models
{
    using MusicX.Common.Models;
    using MusicX.Data.Common.Models;
    using MusicX.Data.Models.Interfaces;

    public class SongMetadata : BaseDeletableModel<int>, IHaveSource
    {
        public int SongId { get; set; }

        public virtual Song Song { get; set; }

        public MetadataType Type { get; set; }

        public string Value { get; set; }

        public int? SourceId { get; set; }

        public Source Source { get; set; }

        public string SourceItemId { get; set; }
    }
}
