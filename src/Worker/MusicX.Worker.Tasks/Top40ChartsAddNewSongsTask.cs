namespace MusicX.Worker.Tasks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;

    using MusicX.Common;
    using MusicX.Common.Models;
    using MusicX.Data.Models;
    using MusicX.Services.Data.Songs;
    using MusicX.Services.DataProviders;
    using MusicX.Worker.Common;

    using Newtonsoft.Json;

    public class Top40ChartsAddNewSongsTask : BaseTask<Top40ChartsAddNewSongsTask.Input, Top40ChartsAddNewSongsTask.Output>
    {
        private readonly ISongsService songsService;

        private readonly ISongMetadataService metadataService;

        private readonly Top40ChartsDataProvider provider;

        private readonly SongNameSplitter splitter;

        private int nextSourceId = 0;

        public Top40ChartsAddNewSongsTask(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.songsService = serviceProvider.GetService<ISongsService>();
            this.metadataService = serviceProvider.GetService<ISongMetadataService>();
            this.provider = new Top40ChartsDataProvider();
            this.splitter = new SongNameSplitter();
        }

        protected override async Task<Output> DoWork(Input input)
        {
            this.nextSourceId = input.SourceId;
            var songsFound = 0;
            for (var i = input.SourceId; i <= input.SourceId + 100; i++)
            {
                var song = this.provider.GetSong(i);
                if (song == null)
                {
                    Console.WriteLine($"{SourcesNames.Top40Charts}#{i} => not found!");
                    continue;
                }

                var artists = this.splitter.SplitArtistName(song[SongMetadataType.Artist]).ToList();
                var title = song[SongMetadataType.Title];
                var songId = await this.songsService.CreateSongAsync(title, artists, SourcesNames.Top40Charts, i.ToString());

                await this.metadataService.AddMetadataInfoAsync(songId, song, SourcesNames.Top40Charts, i.ToString());
                await this.songsService.UpdateSongsSystemDataAsync(songId);

                songsFound++;
                this.nextSourceId = i + 1;
                Console.WriteLine($"{SourcesNames.Top40Charts}#{i} => ({songId}) {song}");
            }

            return new Output { NewSongs = songsFound };
        }

        protected override WorkerTask Recreate(WorkerTask currentTask, Input parameters)
        {
            var runAfter = (currentTask.RunAfter ?? DateTime.UtcNow).AddHours(4); // after 10 seconds
            parameters.SourceId = this.nextSourceId;
            return new WorkerTask(currentTask, JsonConvert.SerializeObject(parameters), runAfter);
        }

        public class Input : BaseTaskInput
        {
            public int SourceId { get; set; }
        }

        public class Output : BaseTaskOutput
        {
            public int NewSongs { get; set; }
        }
    }
}
