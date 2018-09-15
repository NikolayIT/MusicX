namespace MusicX.Worker.Common
{
    using System;
    using System.Threading.Tasks;

    using MusicX.Data.Models;

    using Newtonsoft.Json;

    public abstract class BaseTask<TInput, TOutput> : ITask
    {
        protected BaseTask(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        protected IServiceProvider ServiceProvider { get; }

        public async Task<string> DoWork(string parameters)
        {
            var taskParameters = JsonConvert.DeserializeObject<TInput>(parameters);
            var taskResult = await this.DoWork(taskParameters);
            var taskResultAsString = JsonConvert.SerializeObject(taskResult);
            return taskResultAsString;
        }

        public virtual WorkerTask Recreate(WorkerTask currentTask) => null; // Returning null means no recreation

        protected abstract Task<TOutput> DoWork(TInput input);
    }
}
