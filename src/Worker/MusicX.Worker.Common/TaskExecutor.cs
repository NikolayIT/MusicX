namespace MusicX.Worker.Common
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using MusicX.Data.Models;
    using MusicX.Services.Data.WorkerTasks;

    public class TaskExecutor
    {
        private const int IdleTimeInSeconds = 1;
        private const int WaitTimeOnErrorInSeconds = 30;

        private readonly string name;

        private readonly SynchronizedHashtable tasksSet;

        private readonly IWorkerTasksDataService workerTasksData;

        private readonly IServiceProvider serviceProvider;

        private readonly Assembly tasksAssembly;

        private readonly ILogger logger;

        private bool stopping;

        public TaskExecutor(
            string name,
            SynchronizedHashtable tasksSet,
            IWorkerTasksDataService workerTasksData,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            Assembly tasksAssembly)
        {
            this.name = name;
            this.tasksSet = tasksSet;
            this.workerTasksData = workerTasksData;
            this.serviceProvider = serviceProvider;
            this.tasksAssembly = tasksAssembly;
            this.logger = loggerFactory.CreateLogger<TaskExecutor>();
        }

        public async Task Start()
        {
            this.logger.LogInformation($"{this.name} starting...");
            while (!this.stopping)
            {
                await this.ExecuteNextTask();

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.Title = "Idle";
                }

                // Wait 1 second
                await Task.Delay(1000);
            }

            this.logger.LogInformation($"{this.name} stopped.");
        }

        public void Stop()
        {
            this.stopping = true;
        }

        private async Task ExecuteNextTask()
        {
            WorkerTask workerTask;
            try
            {
                workerTask = this.workerTasksData.GetForProcessing();
            }
            catch (Exception ex)
            {
                this.logger.LogCritical($"Unable to get task for processing. Exception: {ex}");

                await Task.Delay(WaitTimeOnErrorInSeconds * 1000);
                return;
            }

            if (workerTask == null)
            {
                // No task available. Wait few seconds and try again.
                await Task.Delay(IdleTimeInSeconds * 1000);
                return;
            }

            if (!this.tasksSet.Add(workerTask.Id))
            {
                // Other thread is processing the same task.
                // Wait the other thread to set Processing to true and then get new from the DB.
                await Task.Delay(100);
                return;
            }

            try
            {
                workerTask.Processing = true;
                this.workerTasksData.Update(workerTask);
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    $"Unable to set workerTask.{nameof(WorkerTask.Processing)} to true! Exception: {ex}");

                this.tasksSet.Remove(workerTask.Id);

                await Task.Delay(WaitTimeOnErrorInSeconds * 1000);
                return;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.Title = $"[{DateTime.UtcNow}] {workerTask.TypeName}";
            }

            this.logger.LogInformation($"{this.name} started work with task #{workerTask.Id}");

            // New scope is created for each task that's being executed
            using (var taskServiceScope = this.serviceProvider.CreateScope())
            {
                ITask task = null;
                try
                {
                    task = this.GetTaskInstance(workerTask.TypeName, taskServiceScope.ServiceProvider);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(
                        $"{nameof(this.GetTaskInstance)} on task #{workerTask.Id} has thrown an exception: {ex}");

                    workerTask.ProcessingComment = $"Exception in {nameof(this.GetTaskInstance)}: {ex}";
                }

                if (task != null)
                {
                    try
                    {
                        var stopwatch = Stopwatch.StartNew();

                        workerTask.Result = await task.DoWork(workerTask.Parameters);

                        workerTask.Duration = stopwatch.Elapsed.TotalDays >= 1.0
                                                  ? new TimeSpan(0, 23, 59, 59)
                                                  : stopwatch.Elapsed;

                        this.logger.LogInformation(
                            $"Task {workerTask.Id} has completed successfully in {stopwatch.Elapsed} ({DateTime.UtcNow})");

                        this.workerTasksData.Update(workerTask);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(
                            $"{nameof(ITask.DoWork)} on task #{workerTask.Id} has thrown an exception: {ex}");

                        workerTask.ProcessingComment = $"Exception in {nameof(ITask.DoWork)}: {ex}";
                    }

                    try
                    {
                        var nextTask = task.Recreate(workerTask);
                        if (nextTask != null)
                        {
                            this.workerTasksData.Add(nextTask);
                        }

                        this.workerTasksData.Update(workerTask);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(
                            $"{nameof(ITask.Recreate)} on task #{workerTask.Id} has thrown an exception: {ex}");

                        workerTask.ProcessingComment = $"Exception in {nameof(ITask.Recreate)}: {ex}";
                    }
                }
            }

            try
            {
                workerTask.Processed = true;
                workerTask.Processing = false;
                this.workerTasksData.Update(workerTask);

                // For re-runs
                this.tasksSet.Remove(workerTask.Id);
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    $"Unable to save final changes to the task #{workerTask.Id}! Exception: {ex}");

                await Task.Delay(20 * 1000);
            }
        }

        private ITask GetTaskInstance(string typeName, IServiceProvider scopedServiceProvider)
        {
            var type = this.tasksAssembly.GetType(typeName);
            if (!(Activator.CreateInstance(type, scopedServiceProvider) is ITask task))
            {
                throw new Exception($"Unable to create {nameof(ITask)} variable from \"{typeName}\"!");
            }

            return task;
        }
    }
}
