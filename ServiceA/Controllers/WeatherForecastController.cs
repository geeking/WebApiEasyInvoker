using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceA.ThirdService;
using WebApiEasyInvoker.Config;

namespace ServiceA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IServiceB _serviceB;
        private readonly IConsulServiceB _consulServiceB;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IServiceB serviceB,
            IConsulServiceB consulServiceB)
        {
            _logger = logger;
            //config if necessary
            _serviceB = serviceB;
            _consulServiceB = consulServiceB;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var serviceBData = _serviceB.GetWeathers();
            return serviceBData;
        }

        [HttpGet("/test2")]
        public IEnumerable<WeatherForecast> Test2()
        {
            var data = _consulServiceB.GetWeathers();
            return data;
        }
    }
}
