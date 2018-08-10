using MusicX.Web.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicX.Web.Server.Controllers
{
    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;

    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly IRepository<ApplicationRole> rolesRepository;

        public SampleDataController(IRepository<ApplicationRole> rolesRepository)
        {
            this.rolesRepository = rolesRepository;
        }

        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var rng = new Random();
            var range = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });

            range = range.Append(new WeatherForecast() { Summary = this.rolesRepository.All().FirstOrDefault()?.Name });

            return range;
        }
    }
}
