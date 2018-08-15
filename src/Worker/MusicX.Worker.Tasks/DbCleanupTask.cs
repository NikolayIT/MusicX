namespace MusicX.Worker.Tasks
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;

    using MusicX.Data.Common;
    using MusicX.Data.Models;
    using MusicX.Worker.Common;

    public class DbCleanupTask : BaseTask
    {
        private readonly IDbQueryRunner queryRunner;

        public DbCleanupTask(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.queryRunner = serviceProvider.GetService<IDbQueryRunner>();
        }

        public override Task<string> DoWork(string parameters)
        {
            return this.DoWork<Input, Output>(parameters, this.DoWork);
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

        private Task<Output> DoWork(Input input)
        {
            try
            {
                this.queryRunner.RunQuery(
                    $"ALTER INDEX [PK_{nameof(ApplicationUser)}s] ON [dbo].[{nameof(ApplicationUser)}s] REBUILD;");
                Console.WriteLine($"Index [PK_{nameof(ApplicationUser)}s] rebuilt.");

                return Task.FromResult(new Output { Success = true });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Task.FromResult(new Output { Success = false, Exception = e.ToString() });
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
