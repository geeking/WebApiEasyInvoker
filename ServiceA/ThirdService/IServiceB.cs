using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiEasyInvoker;
using WebApiEasyInvoker.Attributes;

namespace ServiceA.ThirdService
{
    [HttpHost("http://localhost:5402/WeatherForecast")]
    public interface IServiceB : IWebApiInvoker<IServiceB>
    {
        [HttpUrl("")]
        List<WeatherForecast> GetWeathers();
    }
}
