using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using WebApiEasyInvoker.Consul.LoadBalance;

namespace WebApiEasyInvoker.Consul.Services
{
    class ConsulServiceInfoProvider : IConsulServiceInfoProvider
    {
        //private readonly IMemoryCache _memoryCache;
        private readonly IConsulServiceRefresh _consulServiceRefresh;

        public ConsulServiceInfoProvider(IConsulServiceRefresh consulServiceRefresh)
        {
            _consulServiceRefresh = consulServiceRefresh;
        }
        public ServiceInfo GetServiceInfo(string serviceName, ILoadBalance loadBalance)
        {
            var infos = _consulServiceRefresh.GetServiceInfos(serviceName);
            if (infos == null || infos.Count() < 1)
            {
                return null;
            }
            ServiceInfo result = loadBalance.ChoseOne(infos);
            return result;
        }
    }
}
