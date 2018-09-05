namespace MusicX.Services.Data.Songs
{
    using MusicX.Common.Models;

    public interface ISongMetadataService
    {
        void AddMetadataInfo(int songId, SongAttributes songAttributes, string sourceName, string sourceItemId);
    }
}
