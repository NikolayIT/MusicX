namespace MusicX.Worker.Common
{
    using System;
    using System.Threading.Tasks;

    using MusicX.Data.Models;

    using Newtonsoft.Json;

    public abstract class BaseTask<TInput, TOutput> : ITask
        where TInput : BaseTaskInput
        where TOutput : BaseTaskOutput, new()
    {
        protected BaseTask(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        protected IServiceProvider ServiceProvider { get; }

        public async Task<string> DoWork(string parameters)
        {
            var taskParameters = JsonConvert.DeserializeObject<TInput>(parameters);
            TOutput taskResult;
            try
            {
                taskResult = await this.DoWork(taskParameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                taskResult = new TOutput { Success = false, Exception = ex.ToString() };
            }

            var taskResultAsString = JsonConvert.SerializeObject(taskResult);
            return taskResultAsString;
        }

        public WorkerTask Recreate(WorkerTask currentTask)
        {
            var taskParameters = JsonConvert.DeserializeObject<TInput>(currentTask.Parameters);
            return taskParameters.Recreate ? this.Recreate(currentTask, taskParameters) : null;
        }

        protected virtual WorkerTask Recreate(WorkerTask currentTask, TInput parameters) => null; // Returning null means no recreation

        protected abstract Task<TOutput> DoWork(TInput input);
    }
}
