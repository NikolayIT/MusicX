namespace Sandbox
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using CommandLine;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using MusicX.Common;
    using MusicX.Common.Models;
    using MusicX.Data;
    using MusicX.Data.Common;
    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;
    using MusicX.Data.Repositories;
    using MusicX.Data.Seeding;
    using MusicX.Services.Data;
    using MusicX.Services.DataProviders;

    using Newtonsoft.Json;

    using Sandbox.Options;

    public static class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine($"{typeof(Program).Namespace} ({string.Join(" ", args)}) starts working...");
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider(true);

            // Seed data on application startup
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
                ApplicationDbContextSeeder.Seed(dbContext, serviceScope.ServiceProvider);
            }

            using (var serviceScope = serviceProvider.CreateScope())
            {
                serviceProvider = serviceScope.ServiceProvider;

                return Parser.Default.ParseArguments<InitialSeedOptions, SandboxOptions>(args)
                    .MapResult(
                        (SandboxOptions opts) => SandboxCode(opts, serviceProvider),
                        (InitialSeedOptions opts) => InitialSeed(opts, serviceProvider),
                        _ => 255);
            }
        }

        private static int SandboxCode(SandboxOptions options, IServiceProvider serviceProvider)
        {
            var sw = Stopwatch.StartNew();

            var songsRepository = serviceProvider.GetService<IDeletableEntityRepository<Song>>();
            var songMetadataRepository = serviceProvider.GetService<IDeletableEntityRepository<SongMetadata>>();
            var songsService = serviceProvider.GetService<ISongsService>();
            var metadataService = serviceProvider.GetService<ISongMetadataService>();
            var provider = new Top40ChartsDataProvider();
            var splitter = new SongNameSplitter();
            var youTubeDataProvider = new YouTubeDataProvider();
            var lyricsPluginDataProvider = new LyricsPluginDataProvider();

            var songIds = songsRepository.All().Select(x => x.Id).ToList();
            for (var index = 0; index < songIds.Count; index++)
            {
                var songId = songIds[index];
                songsService.UpdateSongsSystemDataAsync(songId).GetAwaiter().GetResult();
                if (index % 100 == 0)
                {
                    songsRepository.DetachAll();
                }
            }

            Console.WriteLine(sw.Elapsed);
            return 0;
        }

        private static int InitialSeed(InitialSeedOptions opts, IServiceProvider serviceProvider)
        {
            var sw = Stopwatch.StartNew();

            // Step 1. Seed songs from top40 charts (5 minutes for 1000 songs, so 50000 should be 4-5 hours)
            Console.Title = "Top40 charts songs seed";
            var songsRepository = serviceProvider.GetService<IDeletableEntityRepository<Song>>();
            var songsService = serviceProvider.GetService<ISongsService>();
            var metadataService = serviceProvider.GetService<ISongMetadataService>();
            var provider = new Top40ChartsDataProvider();
            var splitter = new SongNameSplitter();
            for (var i = 1; i <= 50000; i++)
            {
                try
                {
                    var song = provider.GetSong(i);
                    if (song == null)
                    {
                        Console.WriteLine($"{SourcesNames.Top40Charts}#{i} => not found!");
                        continue;
                    }

                    var artists = splitter.SplitArtistName(song[SongMetadataType.Artist]).ToList();
                    var songId = songsService.CreateSongAsync(
                        song[SongMetadataType.Title],
                        artists,
                        SourcesNames.Top40Charts,
                        i.ToString()).GetAwaiter().GetResult();

                    metadataService.AddMetadataInfoAsync(songId, song, SourcesNames.Top40Charts, i.ToString()).GetAwaiter().GetResult();
                    songsService.UpdateSongsSystemDataAsync(songId).GetAwaiter().GetResult();

                    // For better performance
                    if (i % 100 == 0)
                    {
                        songsRepository.DetachAll();
                    }

                    Console.WriteLine($"{SourcesNames.Top40Charts}#{i} => ({songId}) {song}");
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{SourcesNames.Top40Charts}#{i} => {e.Message}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine(sw.Elapsed);
            return 0;
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                    .UseLoggerFactory(new LoggerFactory()));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();
            services.AddScoped<ISongsService, SongsService>();
            services.AddScoped<ISongMetadataService, SongMetadataService>();
        }

        private static void Dump(this object obj)
        {
            Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }
    }
}
