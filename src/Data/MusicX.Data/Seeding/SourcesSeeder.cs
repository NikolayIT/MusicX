namespace MusicX.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MusicX.Common;
    using MusicX.Data.Models;

    public class SourcesSeeder : ISeeder
    {
        public void Seed(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var sources = new List<string>
                          {
                              SourcesNames.Top40Charts,
                              SourcesNames.LyricsPlugin,
                              SourcesNames.YouTube,
                              SourcesNames.User,
                          };

            foreach (var sourceName in sources)
            {
                if (!dbContext.Sources.Any(x => x.Name == sourceName))
                {
                    var source = new Source { Name = sourceName };
                    dbContext.Sources.Add(source);
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
