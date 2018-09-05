namespace MusicX.Data.Seeding
{
    using System;
    using System.Collections.Generic;

    public static class ApplicationDbContextSeeder
    {
        public static void Seed(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var seeders = new List<ISeeder>
                          {
                              new RolesSeeder(),
                              new SourcesSeeder(),
                          };

            foreach (var seeder in seeders)
            {
                seeder.Seed(dbContext, serviceProvider);
                dbContext.SaveChanges();
            }
        }
    }
}
