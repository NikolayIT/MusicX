namespace MusicX.Worker.Tasks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;

    using MusicX.Common;
    using MusicX.Common.Models;
    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;
    using MusicX.Services.Data.Songs;
    using MusicX.Services.DataProviders;
    using MusicX.Worker.Common;

    using Newtonsoft.Json;

    public class FindVideoForSongTask : BaseTask<FindVideoForSongTask.Input, FindVideoForSongTask.Output>
    {
        private readonly IDeletableEntityRepository<Song> songsRepository;

        private readonly ISongMetadataService songMetadataService;

        private readonly YouTubeDataProvider youTubeDataProvider;

        private int nextSongId;

        public FindVideoForSongTask(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.songsRepository = serviceProvider.GetService<IDeletableEntityRepository<Song>>();
            this.songMetadataService = serviceProvider.GetService<ISongMetadataService>();
            this.youTubeDataProvider = new YouTubeDataProvider();
        }

        protected override async Task<Output> DoWork(Input input)
        {
            var song = this.songsRepository.All().OrderBy(x => x.Id)
                .Where(x => x.Id >= input.SongId && x.Metadata.All(m => m.Type != SongMetadataType.YouTubeVideoId))
                .Select(x => new { x.Id, x.Name, Artists = x.Artists.Select(a => a.Artist.Name) }).FirstOrDefault();
            if (song != null)
            {
                this.nextSongId = song.Id + 1;
                var videoId = this.youTubeDataProvider.SearchVideo(string.Join(" ", song.Artists), song.Name);
                if (videoId == null)
                {
                    Console.WriteLine($"Checking video for song #{song.Id} => Not found");
                    return new Output { SongId = song.Id, Found = false };
                }
                else
                {
                    await this.songMetadataService.AddMetadataInfoAsync(
                        song.Id,
                        new SongAttributes(SongMetadataType.YouTubeVideoId, videoId),
                        SourcesNames.YouTube,
                        null);
                    Console.WriteLine($"Checking video for song #{song.Id} ({videoId}) => Ok");
                    return new Output { SongId = song.Id, Found = true };
                }
            }
            else
            {
                // Restart from 1
                this.nextSongId = 1;
                return new Output();
            }
        }

        protected override WorkerTask Recreate(WorkerTask currentTask, Input parameters)
        {
            var runAfter = (currentTask.RunAfter ?? DateTime.UtcNow).AddSeconds(10); // after 10 seconds
            parameters.SongId = this.nextSongId;
            return new WorkerTask(currentTask, JsonConvert.SerializeObject(parameters), runAfter);
        }

        public class Input : BaseTaskInput
        {
            public int SongId { get; set; }
        }

        public class Output : BaseTaskOutput
        {
            public int SongId { get; set; }

            public bool Found { get; set; }
        }
    }
}
