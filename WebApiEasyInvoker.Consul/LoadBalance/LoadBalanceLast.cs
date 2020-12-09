using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiEasyInvoker.Consul.LoadBalance
{
    class LoadBalanceLast : ILoadBalance
    {
        public ServiceInfo ChoseOne(IEnumerable<ServiceInfo> services)
        {
            return services.Last();
        }
    }
}
