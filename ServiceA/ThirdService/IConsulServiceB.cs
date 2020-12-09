using System.Collections.Generic;
using WebApiEasyInvoker;
using WebApiEasyInvoker.Attributes;
using WebApiEasyInvoker.Consul.Attributes;

namespace ServiceA.ThirdService
{
    [ConsulService("serviceB")]
    public interface IConsulServiceB : IWebApiInvoker<IConsulServiceB>
    {
        [ConsulPath("/WeatherForecast", HttpMethodKind.Get)]
        List<WeatherForecast> GetWeathers();
    }
}
