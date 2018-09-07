namespace MusicX.Services.Data.Songs
{
    using System.Threading.Tasks;

    using MusicX.Common.Models;

    public interface ISongMetadataService
    {
        Task AddMetadataInfo(int songId, SongAttributes songAttributes, string sourceName, string sourceItemId);
    }
}
