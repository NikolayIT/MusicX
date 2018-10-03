namespace MusicX.Web.Server
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Mime;
    using System.Reflection;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Blazor.Server;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using MusicX.Common;
    using MusicX.Data;
    using MusicX.Data.Common;
    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;
    using MusicX.Data.Repositories;
    using MusicX.Data.Seeding;
    using MusicX.Services.Data.Songs;
    using MusicX.Web.Server.Infrastructure.Mapping;
    using MusicX.Web.Server.Infrastructure.Middlewares.Authorization;
    using MusicX.Web.Shared;
    using MusicX.Web.Shared.Account;

    using Newtonsoft.Json;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Framework services
            // TODO: Add pooling when this bug is fixed: https://github.com/aspnet/EntityFrameworkCore/issues/9741
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            // JWT Authentication services
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JwtTokenValidation:Secret"]));
            services.Configure<TokenProviderOptions>(opts =>
            {
                opts.Audience = this.configuration["JwtTokenValidation:Audience"];
                opts.Issuer = this.configuration["JwtTokenValidation:Issuer"];
                opts.Path = "/api/account/login";
                opts.Expiration = TimeSpan.FromDays(15);
                opts.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>(IdentityOptionsProvider.GetIdentityOptions)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserStore<ApplicationUserStore>()
                .AddRoleStore<ApplicationRoleStore>()
                .AddDefaultTokenProviders();

            services
                .AddAuthentication()
                .AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters
                                                     {
                                                         ValidateIssuerSigningKey = true,
                                                         IssuerSigningKey = signingKey,
                                                         ValidateIssuer = true,
                                                         ValidIssuer = this.configuration["JwtTokenValidation:Issuer"],
                                                         ValidateAudience = true,
                                                         ValidAudience = this.configuration["JwtTokenValidation:Audience"],
                                                         ValidateLifetime = true
                                                     };
                });

            // Mvc services
            services.AddMvc();
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 443;
            });
            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.AddTransient<ISongsService, SongsService>();
            services.AddTransient<ISongMetadataService, SongMetadataService>();

            // Identity stores
            services.AddTransient<IUserStore<ApplicationUser>, ApplicationUserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, ApplicationRoleStore>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            AutoMapperConfig.RegisterMappings(typeof(UserLoginResponseModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();

                ApplicationDbContextSeeder.Seed(dbContext, serviceScope.ServiceProvider);
            }

            if (env.IsProduction())
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseExceptionHandler(
                alternativeApp =>
                {
                    alternativeApp.Run(
                        async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.ContentType = GlobalConstants.JsonContentType;
                            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                            if (exceptionHandlerFeature?.Error != null)
                            {
                                var ex = exceptionHandlerFeature.Error;
                                while (ex is AggregateException aggregateException
                                       && aggregateException.InnerExceptions.Any())
                                {
                                    ex = aggregateException.InnerExceptions.First();
                                }

                                //// TODO: Log it

                                await context.Response
                                    .WriteAsync(JsonConvert.SerializeObject(new ApiResponse<object>(new ApiError("GLOBAL", ex.Message))))
                                    .ConfigureAwait(continueOnCapturedContext: false);
                            }
                        });
                });

            app.UseResponseCompression();

            app.UseJwtBearerTokens(
                app.ApplicationServices.GetRequiredService<IOptions<TokenProviderOptions>>(),
                PrincipalResolver);

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "api/{controller}/{action}/{id?}");
            });

            app.UseBlazor<Client.Startup>();
        }

        private static async Task<GenericPrincipal> PrincipalResolver(HttpContext context)
        {
            Debug.WriteLine(context.Request.Form["email"]);
            var email = context.Request.Form["email"];
            Debug.WriteLine(email);
            //// TODO if (email empty or null => return null

            var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByEmailAsync(email);
            if (user == null || user.IsDeleted)
            {
                return null;
            }

            var password = context.Request.Form["password"];

            var isValidPassword = await userManager.CheckPasswordAsync(user, password);
            if (!isValidPassword)
            {
                return null;
            }

            var roles = await userManager.GetRolesAsync(user);

            var identity = new GenericIdentity(email, "Token");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

            return new GenericPrincipal(identity, roles.ToArray());
        }
    }
}
