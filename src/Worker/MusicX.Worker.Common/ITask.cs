namespace MusicX.Worker.Common
{
    using System.Threading.Tasks;

    using MusicX.Data.Models;

    public interface ITask
    {
        Task<string> DoWork(string parameters);

        WorkerTask Recreate(WorkerTask currentTask);
    }
}
