namespace MusicX.Services.Data.Songs
{
    using System.Threading.Tasks;

    using MusicX.Common.Models;

    public interface ISongMetadataService
    {
        Task AddMetadataInfoAsync(int songId, SongAttributes songAttributes, string sourceName, string sourceItemId);
    }
}
