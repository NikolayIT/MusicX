namespace MusicX.Worker.Tasks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;

    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;
    using MusicX.Services.Data.Songs;
    using MusicX.Worker.Common;

    public class UpdateSongsSystemDataTask : BaseTask<UpdateSongsSystemDataTask.Input, UpdateSongsSystemDataTask.Output>
    {
        private readonly ISongsService songsService;

        private readonly IDeletableEntityRepository<Song> songsRepository;

        public UpdateSongsSystemDataTask(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.songsService = serviceProvider.GetService<ISongsService>();
            this.songsRepository = serviceProvider.GetService<IDeletableEntityRepository<Song>>();
        }

        protected override async Task<Output> DoWork(Input input)
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

            return new Output();
        }

        protected override WorkerTask Recreate(WorkerTask currentTask, Input parameters)
        {
            var runAfter = DateTime.UtcNow.AddDays(1).Date.AddHours(2); // 2:00 after 1 day
            return new WorkerTask(currentTask, runAfter);
        }

        public class Input : BaseTaskInput
        {
        }

        public class Output : BaseTaskOutput
        {
        }
    }
}
