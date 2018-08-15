namespace MusicX.Worker.Common
{
    using System;
    using System.Threading.Tasks;

    using MusicX.Data.Models;

    using Newtonsoft.Json;

    public abstract class BaseTask
    {
        protected BaseTask(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        protected IServiceProvider ServiceProvider { get; }

        public abstract Task<string> DoWork(string parameters);

        public virtual WorkerTask Recreate(WorkerTask currentTask) => null; // Returning null means no recreation

        protected async Task<string> DoWork<TInput, TOutput>(string parameters, Func<TInput, Task<TOutput>> doWorkFunc)
        {
            var taskParameters =
                JsonConvert.DeserializeObject<TInput>(parameters);
            var taskResult = await doWorkFunc(taskParameters);
            var taskResultAsString = JsonConvert.SerializeObject(taskResult);
            return taskResultAsString;
        }
    }
}
