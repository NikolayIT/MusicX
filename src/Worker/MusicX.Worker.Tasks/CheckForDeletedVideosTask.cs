namespace MusicX.Worker.Tasks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;

    using MusicX.Common.Models;
    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;
    using MusicX.Services.DataProviders;
    using MusicX.Worker.Common;

    public class CheckForDeletedVideosTask : BaseTask<CheckForDeletedVideosTask.Input, CheckForDeletedVideosTask.Output>
    {
        private readonly IDeletableEntityRepository<SongMetadata> songMetadataRepository;

        private readonly YouTubeDataProvider youTubeDataProvider;

        public CheckForDeletedVideosTask(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.songMetadataRepository = serviceProvider.GetService<IDeletableEntityRepository<SongMetadata>>();
            this.youTubeDataProvider = new YouTubeDataProvider();
        }

        public override WorkerTask Recreate(WorkerTask currentTask)
        {
            var runAfter = (currentTask.RunAfter ?? DateTime.UtcNow).AddDays(2).Date.AddHours(4); // 4:00 after 2 days

            var workerTask = new WorkerTask
                             {
                                 TypeName = currentTask.TypeName,
                                 Parameters = currentTask.Parameters,
                                 Priority = currentTask.Priority,
                                 RunAfter = runAfter,
                             };
            return workerTask;
        }

        protected override async Task<Output> DoWork(Input input)
        {
            try
            {
                var youtubeIds = this.songMetadataRepository.All().Where(x => x.Type == SongMetadataType.YouTubeVideoId).ToList();
                foreach (var songMetadata in youtubeIds)
                {
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

                return new Output { Success = true };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Output { Success = false, Exception = e.ToString() };
            }
        }

        public class Input
        {
        }

        public class Output : BaseTaskOutput
        {
        }
    }
}
