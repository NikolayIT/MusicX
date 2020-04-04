namespace MusicX.Services.CronJobs
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using MusicX.Common;
    using MusicX.Common.Models;
    using MusicX.Services.Data;
    using MusicX.Services.DataProviders;

    public class Top40ChartsAddNewSongsJob
    {
        private readonly ISongsService songsService;

        private readonly ISongMetadataService metadataService;

        private readonly ITop40ChartsDataProvider provider;

        private readonly ISongNameSplitter splitter;

        public Top40ChartsAddNewSongsJob(
            ISongsService songsService,
            ISongMetadataService metadataService,
            ITop40ChartsDataProvider provider,
            ISongNameSplitter splitter)
        {
            this.songsService = songsService;
            this.metadataService = metadataService;
            this.provider = provider;
            this.splitter = splitter;
        }

        // TODO: Save nextSourceId in settings?
        public async Task Work(int inputSourceId)
        {
            var nextSourceId = inputSourceId;
            for (var i = inputSourceId; i <= inputSourceId + 100; i++)
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

                nextSourceId = i + 1;
                Console.WriteLine($"{SourcesNames.Top40Charts}#{i} => ({songId}) {song}");
            }
        }
    }
}
