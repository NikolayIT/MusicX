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

    public class FindLyricsForSongTask : BaseTask<FindLyricsForSongTask.Input, FindLyricsForSongTask.Output>
    {
        private readonly IDeletableEntityRepository<Song> songsRepository;

        private readonly ISongsService songsService;

        private readonly ISongMetadataService songMetadataService;

        private readonly LyricsPluginDataProvider lyricsPluginDataProvider;

        private int nextSongId;

        public FindLyricsForSongTask(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.songsRepository = serviceProvider.GetService<IDeletableEntityRepository<Song>>();
            this.songsService = serviceProvider.GetService<ISongsService>();
            this.songMetadataService = serviceProvider.GetService<ISongMetadataService>();
            this.lyricsPluginDataProvider = new LyricsPluginDataProvider();
        }

        protected override async Task<Output> DoWork(Input input)
        {
            var songId = this.songsRepository.All().OrderBy(x => x.Id)
                .Where(x => x.Id >= input.SongId && x.Metadata.All(m => m.Type != SongMetadataType.Lyrics))
                .Select(x => new { x.Id }).FirstOrDefault();
            if (songId != null)
            {
                var song = this.songsService.GetSongsInfo(x => x.Id == songId.Id).FirstOrDefault();
                this.nextSongId = song.Id + 1;

                var lyrics = this.lyricsPluginDataProvider.GetLyrics(song.Artist, song.Title);
                if (string.IsNullOrWhiteSpace(lyrics))
                {
                    Console.WriteLine($"Lyrics for song #{song.Id} \"{song.Artist} - {song.Title}\" => Not found");
                    return new Output { SongId = song.Id, Found = false };
                }
                else
                {
                    await this.songMetadataService.AddMetadataInfoAsync(
                        song.Id,
                        new SongAttributes(SongMetadataType.Lyrics, lyrics),
                        SourcesNames.LyricsPlugin,
                        null);
                    Console.WriteLine($"Lyrics for song #{song.Id} \"{song.Artist} - {song.Title}\" => Ok");
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
            var runAfter = DateTime.UtcNow.AddSeconds(120); // after 2 minutes
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
