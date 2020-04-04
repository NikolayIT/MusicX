namespace MusicX.Services.CronJobs
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;
    using MusicX.Services.Data;

    public class UpdateSongsSystemDataJob
    {
        private readonly ISongsService songsService;

        private readonly IDeletableEntityRepository<Song> songsRepository;

        public UpdateSongsSystemDataJob(ISongsService songsService, IDeletableEntityRepository<Song> songsRepository)
        {
            this.songsService = songsService;
            this.songsRepository = songsRepository;
        }

        public async Task Work()
        {
            var songIds = this.songsRepository.All().Select(x => x.Id).ToList();
            for (var index = 0; index < songIds.Count; index++)
            {
                var songId = songIds[index];
                await this.songsService.UpdateSongsSystemDataAsync(songId);
                if (index % 200 == 0 && index > 0)
                {
                    this.songsRepository.DetachAll();
                    Console.WriteLine($"Updated song system data for {index}/{songIds.Count} songs.");
                }
            }

            Console.WriteLine($"Updated song system data for {songIds.Count}/{songIds.Count} songs.");
        }
    }
}
