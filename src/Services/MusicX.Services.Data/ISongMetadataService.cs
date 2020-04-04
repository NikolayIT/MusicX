namespace MusicX.Services.Data
{
    using System.Threading.Tasks;

    using MusicX.Common.Models;

    public interface ISongMetadataService
    {
        Task AddMetadataInfoAsync(int songId, SongAttributes songAttributes, string sourceName, string sourceItemId);
    }
}
