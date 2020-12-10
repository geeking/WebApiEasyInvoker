using System.Collections.Generic;
using WebApiEasyInvoker;
using WebApiEasyInvoker.Attributes;
using WebApiEasyInvoker.Consul.Attributes;
using WebApiEasyInvoker.Consul.LoadBalance;

namespace ServiceA.ThirdService
{
    [ConsulService("serviceB", BalanceType.Round)]
    public interface IConsulServiceB : IWebApiInvoker<IConsulServiceB>
    {
        [ConsulPath("/WeatherForecast", HttpMethodKind.Get)]
        List<WeatherForecast> GetWeathers();
    }
}
