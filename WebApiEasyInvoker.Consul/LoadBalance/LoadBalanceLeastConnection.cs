using System.Collections.Generic;
using System.Linq;

namespace WebApiEasyInvoker.Consul.LoadBalance
{
    internal class LoadBalanceLeastConnection : ILoadBalance
    {
        private static readonly object _lockObj = new object();

        public ServiceInfo ChoseOne(IEnumerable<ServiceInfo> services)
        {
            lock (_lockObj)
            {
                var sortedServices = services.OrderBy(x => x.AccessCount);
                var service = sortedServices.First();
                return service;
            }
        }
    }
}