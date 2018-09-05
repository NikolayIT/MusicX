namespace MusicX.Data.Seeding
{
    using System;

    public interface ISeeder
    {
        void Seed(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
    }
}
