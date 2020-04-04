namespace MusicX.Services.CronJobs
{
    using System.Threading.Tasks;

    using MusicX.Data.Common;
    using MusicX.Data.Models;

    // TODO: var runAfter = (currentTask.RunAfter ?? DateTime.UtcNow).AddDays(7).Date.AddHours(19); // 19:00 after 7 days
    public class DbCleanupTaskJob
    {
        private readonly IDbQueryRunner dbQueryRunner;

        public DbCleanupTaskJob(IDbQueryRunner dbQueryRunner)
        {
            this.dbQueryRunner = dbQueryRunner;
        }

        public async Task Work()
        {
            await this.dbQueryRunner.RunQueryAsync(
                $"ALTER INDEX [PK_{nameof(SongMetadata)}] ON [dbo].[{nameof(SongMetadata)}] REBUILD;");

            await this.dbQueryRunner.RunQueryAsync(
                $"ALTER INDEX [PK_{nameof(Song)}s] ON [dbo].[{nameof(Song)}s] REBUILD;");
        }
    }
}
