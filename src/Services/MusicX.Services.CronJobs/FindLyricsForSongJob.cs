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
    public class FindLyricsForSongJob
    {
        private readonly IDeletableEntityRepository<Song> songsRepository;

        private readonly ISongsService songsService;

        private readonly ISongMetadataService songMetadataService;

        private readonly ILyricsPluginDataProvider lyricsPluginDataProvider;

        public FindLyricsForSongJob(
            IDeletableEntityRepository<Song> songsRepository,
            ISongsService songsService,
            ISongMetadataService songMetadataService,
            ILyricsPluginDataProvider lyricsPluginDataProvider)
        {
            this.songsRepository = songsRepository;
            this.songsService = songsService;
            this.songMetadataService = songMetadataService;
            this.lyricsPluginDataProvider = lyricsPluginDataProvider;
        }

        // TODO: Save nextSongId in settings?
        public async Task Work(int inputSongId)
        {
            var nextSongId = 0;
            var songId = this.songsRepository.All().OrderBy(x => x.Id)
                .Where(x => x.Id >= inputSongId && x.Metadata.All(m => m.Type != SongMetadataType.Lyrics))
                .Select(x => new { x.Id }).FirstOrDefault();
            if (songId != null)
            {
                var song = this.songsService.GetSongsInfo(x => x.Id == songId.Id).FirstOrDefault();
                nextSongId = song.Id + 1;

                var lyrics = this.lyricsPluginDataProvider.GetLyrics(song.Artist, song.Title);
                if (string.IsNullOrWhiteSpace(lyrics))
                {
                    Console.WriteLine($"Lyrics for song #{song.Id} \"{song.Artist} - {song.Title}\" => Not found");
                }
                else
                {
                    await this.songMetadataService.AddMetadataInfoAsync(
                        song.Id,
                        new SongAttributes(SongMetadataType.Lyrics, lyrics),
                        SourcesNames.LyricsPlugin,
                        null);
                    Console.WriteLine($"Lyrics for song #{song.Id} \"{song.Artist} - {song.Title}\" => Ok");
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
