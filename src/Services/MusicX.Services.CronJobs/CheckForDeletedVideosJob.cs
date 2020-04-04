namespace MusicX.Services.CronJobs
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using MusicX.Common.Models;
    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;
    using MusicX.Services.DataProviders;

    // TODO: var runAfter = DateTime.UtcNow.AddDays(2).Date.AddHours(4); // 4:00 after 2 days
    public class CheckForDeletedVideosJob
    {
        private readonly IDeletableEntityRepository<SongMetadata> songMetadataRepository;

        private readonly IYouTubeDataProvider youTubeDataProvider;

        public CheckForDeletedVideosJob(
            IDeletableEntityRepository<SongMetadata> songMetadataRepository,
            IYouTubeDataProvider youTubeDataProvider)
        {
            this.songMetadataRepository = songMetadataRepository;
            this.youTubeDataProvider = youTubeDataProvider;
        }

        public async Task Work()
        {
            var youtubeIds = this.songMetadataRepository.All().Where(x => x.Type == SongMetadataType.YouTubeVideoId).ToList();
            foreach (var songMetadata in youtubeIds)
            {
                // TODO: Replace Console with logger
                Console.Write($"Checking video for song #{songMetadata.SongId} ({songMetadata.Value}) => ");
                if (!await this.youTubeDataProvider.CheckIfVideoExists(songMetadata.Value))
                {
                    this.songMetadataRepository.HardDelete(songMetadata);
                    await this.songMetadataRepository.SaveChangesAsync();
                    Console.WriteLine("DELETED");
                }
                else
                {
                    Console.WriteLine("OK");
                }
            }
        }
    }
}
