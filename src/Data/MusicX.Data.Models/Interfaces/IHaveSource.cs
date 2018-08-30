namespace MusicX.Data.Models.Interfaces
{
    public interface IHaveSource
    {
        int? SourceId { get; set; }

        Source Source { get; set; }

        string SourceItemId { get; set; }
    }
}
