namespace MusicX.Data.Models
{
    public interface IHaveSource
    {
        int? SourceId { get; set; }

        Source Source { get; set; }
    }
}
