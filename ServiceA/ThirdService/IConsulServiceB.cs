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

        [ConsulPath("/WeatherForecast/{id}")]
        WeatherForecast GetWeather(int id);

        [ConsulPath("/WeatherForecast", HttpMethodKind.Post)]
        bool Add([ToBody] WeatherForecast weatherForecast);

        [ConsulPath("/WeatherForecast", HttpMethodKind.Put)]
        bool Update([ToQuery] int id, [ToBody] WeatherForecast weatherForecast);
    }
}