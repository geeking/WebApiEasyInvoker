using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WebApiEasyInvoker.Consul.Attributes;
using WebApiEasyInvoker.Consul.LoadBalance;
using WebApiEasyInvoker.Consul.Services;
using WebApiEasyInvoker.Interfaces;

namespace WebApiEasyInvoker.Consul
{
    public static class ConsulServiceCollectionExtensions
    {
        public static void AddWebApiEasyInvoker(this IServiceCollection services, EasyInvokerConsulConfig consulConfig, string httpClientName = "")
        {
            if (consulConfig == null)
            {
                throw new NullReferenceException("EasyInvokerConsulConfig is null");
            }

            services.TryAddScoped<IUrlBuilder, UrlBuilderConsul>();
            services.AddSingleton<IConsulServiceRefresh, ConsulServiceRefresh>();
            services.AddSingleton<IConsulServiceInfoProvider, ConsulServiceInfoProvider>();
            services.AddWebApiEasyInvoker(httpClientName);

            var serviceRefresh = services.BuildServiceProvider().GetService<IConsulServiceRefresh>();
            var serviceList = new List<string>();
            var balanceDic = new Dictionary<string, ILoadBalance>();
            foreach (var assemble in AppDomain.CurrentDomain.GetAssemblies())
            {
                assemble.GetTypes().ToList().ForEach(t =>
                {
                    if (t.IsInterface && t.GetInterface("IWebApiInvoker`1", true) != null)
                    {
                        var attribute = t.GetCustomAttribute<ConsulServiceAttribute>();
                        if (attribute != null)
                        {
                            var balanceType = attribute.BalanceType;
                            ILoadBalance loadBalance;
                            switch (balanceType)
                            {
                                case BalanceType.First:
                                    loadBalance = new LoadBalanceFirst();
                                    break;

                                case BalanceType.Last:
                                    loadBalance = new LoadBalanceLast();
                                    break;

                                case BalanceType.Round:
                                    loadBalance = new LoadBalanceRound();
                                    break;

                                case BalanceType.Random:
                                    loadBalance = new LoadBalanceRandom();
                                    break;

                                case BalanceType.LeastConnection:
                                    loadBalance = new LoadBalanceLeastConnection();
                                    break;

                                default:
                                    loadBalance = new LoadBalanceFirst();
                                    break;
                            }

                            balanceDic.Add(t.FullName, loadBalance);
                            if (!serviceList.Contains(attribute.ServiceName))
                            {
                                serviceList.Add(attribute.ServiceName);
                            }
                        }
                    }
                });
            }

            LoadBalanceRepository.LoadData(balanceDic);
            serviceRefresh.LoadServices(serviceList);
            serviceRefresh.Start(consulConfig);
        }
    }
}