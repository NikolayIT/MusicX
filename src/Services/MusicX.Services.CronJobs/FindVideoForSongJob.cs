namespace MusicX.Services.CronJobs
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using MusicX.Common;
    using MusicX.Common.Models;
    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;
    using MusicX.Services.Data;
    using MusicX.Services.DataProviders;

    // TODO: var runAfter = DateTime.UtcNow.AddSeconds(120); // after 2 minutes
    public class FindVideoForSongJob
    {
        private readonly IDeletableEntityRepository<Song> songsRepository;

        private readonly ISongMetadataService songMetadataService;

        private readonly IYouTubeDataProvider youTubeDataProvider;

        public FindVideoForSongJob(
            IDeletableEntityRepository<Song> songsRepository,
            ISongMetadataService songMetadataService,
            IYouTubeDataProvider youTubeDataProvider)
        {
            this.songsRepository = songsRepository;
            this.songMetadataService = songMetadataService;
            this.youTubeDataProvider = youTubeDataProvider;
        }

        // TODO: Save nextSongId in settings?
        public async Task Work(int inputSongId)
        {
            var nextSongId = 0;
            var song = this.songsRepository.All().OrderBy(x => x.Id)
                .Where(x => x.Id >= inputSongId && x.Metadata.All(m => m.Type != SongMetadataType.YouTubeVideoId))
                .Select(x => new { x.Id, x.Name, Artists = x.Artists.Select(a => a.Artist.Name) }).FirstOrDefault();
            if (song != null)
            {
                nextSongId = song.Id + 1;
                var videoId = this.youTubeDataProvider.SearchVideo(string.Join(" ", song.Artists), song.Name);
                if (videoId == null)
                {
                    Console.WriteLine($"Checking video for song #{song.Id} => Not found");
                }
                else
                {
                    await this.songMetadataService.AddMetadataInfoAsync(
                        song.Id,
                        new SongAttributes(SongMetadataType.YouTubeVideoId, videoId),
                        SourcesNames.YouTube,
                        null);
                    Console.WriteLine($"Checking video for song #{song.Id} ({videoId}) => Ok");
                }
            }
            else
            {
                // Restart from 1
                nextSongId = 1;
            }
        }
    }
}
