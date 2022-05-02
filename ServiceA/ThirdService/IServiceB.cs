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
        [HttpUrl]
        List<WeatherForecast> GetWeathers();

        [HttpUrl("/{id}")]
        WeatherForecast GetWeather(int id);

        [HttpUrl("", HttpMethodKind.Post)]
        bool Add([ToBody] WeatherForecast weatherForecast);

        [HttpUrl("", HttpMethodKind.Put)]
        bool Update([ToQuery] int id, [ToBody] WeatherForecast weatherForecast);
    }

    [HttpHost("http://localhost:5402/")]
    public interface IServiceB1 : IWebApiInvoker<IServiceB1>
    {
        [HttpUrl("WeatherForecast")]
        List<WeatherForecast> GetWeathers();

        [HttpUrl("WeatherForecast/{id}")]
        WeatherForecast GetWeather(int id);

        [HttpUrl("/WeatherForecast", HttpMethodKind.Post)]
        bool Add([ToBody] WeatherForecast weatherForecast);

        [HttpUrl("/WeatherForecast", HttpMethodKind.Put)]
        bool Update([ToQuery] int id, [ToBody] WeatherForecast weatherForecast);
    }

    [HttpHost("ServiceB:Host", FromConfiguration = true)]
    public interface IServiceB2 : IWebApiInvoker<IServiceB2>
    {
        [HttpUrl("/WeatherForecast")]
        List<WeatherForecast> GetWeathers();

        [HttpFullUrl("http://localhost:5402/WeatherForecast/{id}")]
        WeatherForecast GetWeather(int id);

        [HttpUrl("ServiceB:AddWeather", HttpMethodKind.Post, FromConfiguration = true)]
        bool Add([ToBody] WeatherForecast weatherForecast);

        [HttpUrl("ServiceB:UpdateWeather", HttpMethodKind.Put, FromConfiguration = true)]
        bool Update([ToQuery] int id, [ToBody] WeatherForecast weatherForecast);
    }
}