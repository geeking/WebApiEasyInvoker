using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace WebApiEasyInvoker.Consul.Services
{
    class ConsulServiceInfoProvider : IConsulServiceInfoProvider
    {
        private readonly IMemoryCache _memoryCache;
        //private readonly ConcurrentDictionary<string,ServiceInfo[]>
        public ConsulServiceInfoProvider()
        {
            _memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        }
        public ServiceInfo GetServiceInfo(string serviceName, BalanceType balanceType)
        {
            throw new System.NotImplementedException();
        }
    }
}
