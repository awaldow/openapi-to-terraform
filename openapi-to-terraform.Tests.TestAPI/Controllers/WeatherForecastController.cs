using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using openapi_to_terraform.Extensions.Attributes;

namespace openapi_to_terraform.Tests.TestAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    [ApiController]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// GetWeatherForecastById(Guid forecastId)
        /// </summary>
        /// <remarks>
        /// Returns WeatherForecast object for <paramref name="forecastId"/>
        /// </remarks>
        /// <param name="forecastId">WeatherForecast ID for the WeatherForecast to get</param>
        /// <returns>WeatherForecast object with id <paramref name="forecastId"/></returns>
        [HttpGet("{forecastId}", Name = "GetWeatherForecastById")]
        [MapToApiVersion("1")]
        [Revisions(1, 2)]
        [ProducesResponseType(typeof(WeatherForecast), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public WeatherForecast GetWeatherForecastById(Guid forecastId)
        {
            Random rng = new Random();
            return new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            };
        }

        /// <summary>
        /// GetWeatherForecasts()
        /// </summary>
        /// <remarks>
        /// Returns WeatherForecast list
        /// </remarks>
        /// <returns>WeatherForecast object</returns>
        [HttpGet(Name = "GetWeatherForecast")]
        [MapToApiVersion("1")]
        [Revisions(1)]
        [ProducesResponseType(typeof(List<WeatherForecast>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public List<WeatherForecast> GetWeatherForecast()
        {
            Random rng = new Random();
            return new List<WeatherForecast>
            {
                new WeatherForecast
                {
                    Date = DateTime.Now,
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                }
            };
        }
    }
}
