namespace MusicX.Web.Server.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;
    using MusicX.Web.Server.Infrastructure;
    using MusicX.Web.Shared;
    using MusicX.Web.Shared.TelemetryData;

    [AllowAnonymous]
    public class TelemetryDataController : BaseController
    {
        private readonly IRepository<SongPlay> songPlaysRepository;

        public TelemetryDataController(IRepository<SongPlay> songPlaysRepository)
        {
            this.songPlaysRepository = songPlaysRepository;
        }

        [HttpPost]
        public async Task<ApiResponse<SongPlayTelemetryResponse>> SongPlay([FromBody]SongPlayTelemetryRequest request)
        {
            await this.songPlaysRepository.AddAsync(
                new SongPlay
                    {
                        OwnerId = this.User?.GetId(),
                        SongId = request.SongId,
                        PlayedByUser = request.PlayedByUser,
                        SessionId = request.SessionId,
                    });
            await this.songPlaysRepository.SaveChangesAsync();
            return new SongPlayTelemetryResponse().ToApiResponse();
        }
    }
}
