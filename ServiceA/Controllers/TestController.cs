using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceA.ThirdService;
using WebApiEasyInvoker.Config;

namespace ServiceA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IServiceB _serviceB;
        private readonly IServiceB1 _serviceB1;
        private readonly IServiceB2 _serviceB2;
        private readonly IConsulServiceB _consulServiceB;

        public TestController(ILogger<TestController> logger,
            IServiceB serviceB,
            IServiceB1 serviceB1,
            IServiceB2 serviceB2,
            IConsulServiceB consulServiceB)
        {
            _logger = logger;
            //config if necessary
            _serviceB = serviceB;
            _serviceB1 = serviceB1;
            _serviceB2 = serviceB2;
            _consulServiceB = consulServiceB;
        }

        [HttpGet]
        public string Get()
        {
            var sb = new StringBuilder();
            var r1 = _serviceB.GetWeathers();
            sb.AppendLine(JsonConvert.SerializeObject(r1));
            sb.AppendLine();

            var r2 = _serviceB.GetWeather(1);
            sb.AppendLine(JsonConvert.SerializeObject(r2));
            sb.AppendLine();

            var testWeather = new WeatherForecast
            {
                Date = DateTime.Now,
                Summary = "xxx",
                TemperatureC = 33
            };
            var r3 = _serviceB.Add(testWeather);
            sb.AppendLine(r3.ToString());
            sb.AppendLine();

            var r4 = _serviceB.Update(3, testWeather);
            sb.AppendLine(r4.ToString());
            sb.AppendLine();

            sb.AppendLine("---------------");

            var r1_1 = _serviceB1.GetWeathers();
            sb.AppendLine(JsonConvert.SerializeObject(r1_1));
            sb.AppendLine();

            var r1_2 = _serviceB1.GetWeather(1);
            sb.AppendLine(JsonConvert.SerializeObject(r1_2));
            sb.AppendLine();

            var testWeather1 = new WeatherForecast
            {
                Date = DateTime.Now,
                Summary = "yyy",
                TemperatureC = 32
            };
            var r1_3 = _serviceB1.Add(testWeather1);
            sb.AppendLine(r1_3.ToString());
            sb.AppendLine();

            var r1_4 = _serviceB1.Update(3, testWeather1);
            sb.AppendLine(r1_4.ToString());
            sb.AppendLine();

            sb.AppendLine("---------------");

            var r2_1 = _serviceB2.GetWeathers();
            sb.AppendLine(JsonConvert.SerializeObject(r2_1));
            sb.AppendLine();

            var r2_2 = _serviceB2.GetWeather(1);
            sb.AppendLine(JsonConvert.SerializeObject(r2_2));
            sb.AppendLine();

            var testWeather2 = new WeatherForecast
            {
                Date = DateTime.Now,
                Summary = "zzzz",
                TemperatureC = 32
            };
            var r2_3 = _serviceB2.Add(testWeather2);
            sb.AppendLine(r2_3.ToString());
            sb.AppendLine();

            var r2_4 = _serviceB2.Update(3, testWeather2);
            sb.AppendLine(r2_4.ToString());
            sb.AppendLine();

            return sb.ToString();
        }

        [HttpGet("/test2")]
        public string Test2()
        {
            var sb = new StringBuilder();
            var r1 = _consulServiceB.GetWeathers();
            sb.AppendLine(JsonConvert.SerializeObject(r1));
            sb.AppendLine();

            var r2 = _consulServiceB.GetWeather(1);
            sb.AppendLine(JsonConvert.SerializeObject(r2));
            sb.AppendLine();

            var testWeather = new WeatherForecast
            {
                Date = DateTime.Now,
                Summary = "xxx",
                TemperatureC = 33
            };
            var r4 = _consulServiceB.Add(testWeather);
            sb.AppendLine(r4.ToString());
            sb.AppendLine();

            var r5 = _consulServiceB.Update(3, testWeather);
            sb.AppendLine(r5.ToString());
            sb.AppendLine();

            return sb.ToString();
        }
    }
}