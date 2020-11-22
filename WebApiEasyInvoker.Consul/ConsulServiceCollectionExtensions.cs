using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using WebApiEasyInvoker.Consul.Services;
using WebApiEasyInvoker.Interfaces;

namespace WebApiEasyInvoker.Consul
{
    public static class ConsulServiceCollectionExtensions
    {
        public static void AddWebApiEasyInvoker(this IServiceCollection services, EasyInvokerConsulConfig consulConfig)
        {
            GlobalValue.SetConsulConfig(consulConfig);
            services.TryAddScoped<IUrlBuilder, UrlBuilderConsul>();
            services.AddSingleton<IConsulServiceRefresh, ConsulServiceRefresh>();
            services.AddSingleton<IConsulServiceInfoProvider, ConsulServiceInfoProvider>();
            services.AddWebApiEasyInvoker();
        }
    }
}
