namespace Sandbox
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using CommandLine;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using MusicX.Common.Models;
    using MusicX.Data;
    using MusicX.Data.Common;
    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;
    using MusicX.Data.Repositories;
    using MusicX.Data.Seeding;
    using MusicX.Services.Data.Songs;
    using MusicX.Services.Data.WorkerTasks;
    using MusicX.Services.DataProviders;
    using MusicX.Worker.Common;

    using Newtonsoft.Json;

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

                return Parser.Default.ParseArguments<RunTaskOptions, SandboxOptions>(args).MapResult(
                    (RunTaskOptions opts) => RunTask(opts, serviceProvider),
                    (SandboxOptions opts) => SandboxCode(opts, serviceProvider),
                    _ => 255);
            }
        }

        private static int RunTask(RunTaskOptions options, IServiceProvider serviceProvider)
        {
            var typeName = $"MusicX.Worker.Tasks.{options.TaskName}";

            var type = typeof(BaseTask).Assembly.GetType(typeName);
            try
            {
                if (!(Activator.CreateInstance(type, serviceProvider) is BaseTask task))
                {
                    Console.WriteLine($"Unable to create instance of \"{typeName}\"!");
                    return 1;
                }

                var sw = Stopwatch.StartNew();
                task.DoWork(options.Parameters);
                Console.WriteLine($"Time elapsed: {sw.Elapsed}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 2;
            }

            return 0;
        }

        private static int SandboxCode(SandboxOptions options, IServiceProvider serviceProvider)
        {
            var sw = Stopwatch.StartNew();

            var songsService = serviceProvider.GetService<ISongsService>();
            var provider = new Top40ChartsDataProvider();
            var splitter = new SongNameSplitter();
            for (int i = 0; i < 100; i++)
            {
                var song = provider.GetArtistAndSongTitle(i);
                if (song == null)
                {
                    Console.WriteLine($"Top40: song with id {i} => not found!");
                    continue;
                }

                var artists = splitter.SplitArtistName(song.Artist).ToList();
                songsService.CreateSong(new SongArtistsAndTitle(artists, song.Title));

                Console.WriteLine($"Top40: song with id {i} => {song}");
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

            services.AddIdentity<ApplicationUser, ApplicationRole>(IdentityOptionsProvider.GetIdentityOptions)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserStore<ApplicationUserStore>()
                .AddRoleStore<ApplicationRoleStore>()
                .AddDefaultTokenProviders();

            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();
            services.AddScoped<IWorkerTasksDataService, WorkerTasksDataService>();
        }

        private static void Dump(this object obj)
        {
            Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }
    }
}
