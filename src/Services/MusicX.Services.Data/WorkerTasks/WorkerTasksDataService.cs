namespace MusicX.Services.Data.WorkerTasks
{
    using System;
    using System.Linq;

    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;

    public class WorkerTasksDataService : IWorkerTasksDataService
    {
        private readonly IDeletableEntityRepository<WorkerTask> workerTasks;

        public WorkerTasksDataService(IDeletableEntityRepository<WorkerTask> workerTasks)
        {
            this.workerTasks = workerTasks;
        }

        public WorkerTask GetForProcessing()
            => this.workerTasks
                .All()
                .Where(x => !x.Processed && !x.Processing && (x.RunAfter == null || x.RunAfter <= DateTime.UtcNow))
                .OrderByDescending(x => x.Priority)
                .ThenBy(x => x.Id)
                .FirstOrDefault();

        public void Update(WorkerTask workerTask)
        {
            if (workerTask == null)
            {
                throw new ArgumentNullException(nameof(workerTask), "Worker task to update is required.");
            }

            this.workerTasks.Update(workerTask);
            this.workerTasks.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public void Add(WorkerTask workerTask)
        {
            if (workerTask == null)
            {
                throw new ArgumentNullException(nameof(workerTask), "Worker task to add is required.");
            }

            this.workerTasks.Add(workerTask);
            this.workerTasks.SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}
