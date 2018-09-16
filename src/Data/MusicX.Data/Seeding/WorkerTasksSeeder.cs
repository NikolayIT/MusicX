namespace MusicX.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MusicX.Data.Models;

    public class WorkerTasksSeeder : ISeeder
    {
        public void Seed(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var workerTasks = new List<WorkerTask>
            {
                new WorkerTask
                {
                    TypeName = "MusicX.Worker.Tasks.DbCleanupTask",
                    Parameters = "{\"Recreate\":true}",
                    Priority = 0,
                },
                new WorkerTask
                {
                    TypeName = "MusicX.Worker.Tasks.FindLyricsForSongTask",
                    Parameters = "{\"Recreate\":true,\"SongId\":1}",
                    Priority = 4000,
                },
                new WorkerTask
                {
                    TypeName = "MusicX.Worker.Tasks.FindVideoForSongTask",
                    Parameters = "{\"Recreate\":true,\"SongId\":1}",
                    Priority = 5000,
                },
                new WorkerTask
                {
                    TypeName = "MusicX.Worker.Tasks.CheckForDeletedVideosTask",
                    Parameters = "{\"Recreate\":true}",
                    Priority = 10000,
                },
                new WorkerTask
                {
                    TypeName = "MusicX.Worker.Tasks.Top40ChartsAddNewSongsTask",
                    Parameters = "{\"Recreate\":true,\"SourceId\":1}",
                    Priority = 20000,
                },
                new WorkerTask
                {
                    TypeName = "MusicX.Worker.Tasks.UpdateSongsSystemDataTask",
                    Parameters = "{\"Recreate\":true}",
                    Priority = 30000,
                },
            };

            foreach (var workerTask in workerTasks)
            {
                if (!dbContext.WorkerTasks.Any(x => x.TypeName == workerTask.TypeName))
                {
                    dbContext.WorkerTasks.Add(workerTask);
                }
            }
        }
    }
}
