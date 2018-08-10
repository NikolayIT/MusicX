namespace MusicX.Services.Data.WorkerTasks
{
    using MusicX.Data.Models;

    public interface IWorkerTasksDataService
    {
        WorkerTask GetForProcessing();

        void Update(WorkerTask workerTask);

        void Add(WorkerTask workerTask);
    }
}
