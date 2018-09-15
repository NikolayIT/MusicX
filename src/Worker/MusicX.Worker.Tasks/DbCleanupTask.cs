namespace MusicX.Worker.Tasks
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;

    using MusicX.Data.Common;
    using MusicX.Data.Models;
    using MusicX.Worker.Common;

    public class DbCleanupTask : BaseTask<DbCleanupTask.Input, DbCleanupTask.Output>
    {
        private readonly IDbQueryRunner queryRunner;

        public DbCleanupTask(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.queryRunner = serviceProvider.GetService<IDbQueryRunner>();
        }

        public override WorkerTask Recreate(WorkerTask currentTask)
        {
            var runAfter = (currentTask.RunAfter ?? DateTime.UtcNow).AddDays(7).Date.AddHours(19); // 19:00 after 7 days

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
                await this.queryRunner.RunQueryAsync(
                    $"ALTER INDEX [PK_{nameof(SongMetadata)}] ON [dbo].[{nameof(SongMetadata)}] REBUILD;");
                Console.WriteLine($"Index [PK_{nameof(SongMetadata)}] rebuilt.");

                await this.queryRunner.RunQueryAsync(
                    $"ALTER INDEX [PK_{nameof(Song)}s] ON [dbo].[{nameof(Song)}s] REBUILD;");
                Console.WriteLine($"Index [PK_{nameof(Song)}s] rebuilt.");

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
