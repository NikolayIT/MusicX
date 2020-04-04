namespace MusicX.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using MusicX.Common.Models;
    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;

    public class SongMetadataService : ISongMetadataService
    {
        private readonly IDeletableEntityRepository<SongMetadata> songMetadataRepository;

        private readonly IRepository<Source> sourcesRepository;

        public SongMetadataService(
            IDeletableEntityRepository<SongMetadata> songMetadataRepository,
            IRepository<Source> sourcesRepository)
        {
            this.songMetadataRepository = songMetadataRepository;
            this.sourcesRepository = sourcesRepository;
        }

        public async Task AddMetadataInfoAsync(int songId, SongAttributes songAttributes, string sourceName, string sourceItemId)
        {
            var sourceId = this.sourcesRepository.All().FirstOrDefault(x => x.Name == sourceName)?.Id;

            foreach (var metadata in songAttributes.Where(x => x.Key != SongMetadataType.Artist && x.Key != SongMetadataType.Title))
            {
                foreach (var value in metadata.Value)
                {
                    var songMetadata = new SongMetadata
                                       {
                                           SongId = songId,
                                           Type = metadata.Key,
                                           Value = value,
                                           SourceId = sourceId,
                                           SourceItemId = sourceItemId,
                                       };
                    await this.songMetadataRepository.AddAsync(songMetadata);
                }
            }

            await this.songMetadataRepository.SaveChangesAsync();
        }
    }
}
