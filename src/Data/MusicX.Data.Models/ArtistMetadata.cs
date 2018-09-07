namespace MusicX.Data.Models
{
    using MusicX.Common.Models;
    using MusicX.Data.Common.Models;
    using MusicX.Data.Models.Interfaces;

    public class ArtistMetadata : BaseDeletableModel<int>, IHaveSource
    {
        public int ArtistId { get; set; }

        public virtual Artist Artist { get; set; }

        public ArtistMetadataType Type { get; set; }

        public string Value { get; set; }

        public int? SourceId { get; set; }

        public Source Source { get; set; }

        public string SourceItemId { get; set; }
    }
}
