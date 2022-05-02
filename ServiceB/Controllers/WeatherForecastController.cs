using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ServiceB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private static List<WeatherForecast> _weatherForecasts4Test;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;

            var rng = new Random();
            _weatherForecasts4Test = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToList();
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return _weatherForecasts4Test;
        }

        [HttpGet("{id}")]
        public WeatherForecast Get(int id)
        {
            if (id < 0 || id >= _weatherForecasts4Test.Count)
            {
                id = 0;
            }

            return _weatherForecasts4Test[id];
        }

        [HttpPost]
        public bool Add(WeatherForecast weatherForecast)
        {
            _weatherForecasts4Test.Add(weatherForecast);
            return true;
        }

        [HttpPut]
        public bool Update([FromQuery] int id, [FromBody] WeatherForecast weatherForecast)
        {
            if (id < 0 || id >= _weatherForecasts4Test.Count)
            {
                return false;
            }
            _weatherForecasts4Test[id] = weatherForecast;
            return true;
        }
    }
}