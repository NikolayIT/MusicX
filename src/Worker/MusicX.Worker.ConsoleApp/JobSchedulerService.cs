namespace MusicX.Worker.ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using MusicX.Services.Data.WorkerTasks;
    using MusicX.Worker.Common;

    public class JobSchedulerService : IConsoleService
    {
        private readonly ICollection<TaskExecutor> taskExecutors = new List<TaskExecutor>();
        private readonly IList<Thread> threads = new List<Thread>();
        private readonly SynchronizedHashtable tasksSet = new SynchronizedHashtable();
        private readonly IServiceProvider serviceProvider;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger logger;

        public JobSchedulerService(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            var threadsCount = int.Parse(configuration["JobScheduler:ThreadsCount"]);
            for (var i = 1; i <= threadsCount; i++)
            {
                var thread = new Thread(this.CreateAndStartTaskExecutor) { Name = $"Thread #{i}" };
                this.threads.Add(thread);
            }

            this.serviceProvider = serviceProvider;
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger<JobSchedulerService>();
        }

        public void StartService(string[] args)
        {
            for (var i = 0; i < this.threads.Count; i++)
            {
                var thread = this.threads[i];

                this.logger.LogInformation($"Starting {thread.Name}...");
                thread.Start(i + 1);
                this.logger.LogInformation($"{thread.Name} started.");

                // Give the thread some time to start properly before starting another threads.
                // (Prevents "System.ArgumentException: An item with the same key has already been added" related to the DI framework)
                Thread.Sleep(300);
            }

            // TODO: Write in own log - this.log.WriteEntry($"JobSchedulerService started with {ThreadsCount} threads.");
            this.logger.LogInformation($"JobSchedulerService started with {this.threads.Count} threads.");
        }

        public void EndService()
        {
            foreach (var taskExecutor in this.taskExecutors)
            {
                taskExecutor.Stop();
            }

            // TODO: Write in own log - this.log.WriteEntry("JobSchedulerService stopped.");
            this.logger.LogInformation("JobSchedulerService stopped.");
        }

        private async void CreateAndStartTaskExecutor(object taskExecutorNumber)
        {
            var taskExecutorName = $"TaskExecutor #{taskExecutorNumber}";
            TaskExecutor taskExecutor = null;

            // This is an important exception handling because an exception in async void method would crash the app.
            // (https://blogs.msdn.microsoft.com/ptorr/2014/12/10/async-exceptions-in-c/)
            try
            {
                using (var serviceScope = this.serviceProvider.CreateScope())
                {
                    taskExecutor = new TaskExecutor(
                        taskExecutorName,
                        this.tasksSet,
                        serviceScope.ServiceProvider.GetRequiredService<IWorkerTasksDataService>(),
                        serviceScope.ServiceProvider,
                        this.loggerFactory);

                    this.taskExecutors.Add(taskExecutor);

                    await taskExecutor.Start();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(
                    taskExecutor == null
                        ? $"Exception during initialization of {taskExecutorName}: {ex}"
                        : $"Exception during {taskExecutorName}'s work: {ex}");
            }
        }
    }
}
