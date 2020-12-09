using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiEasyInvoker.Consul.LoadBalance
{
    class LoadBalanceFirst : ILoadBalance
    {
        public ServiceInfo ChoseOne(IEnumerable<ServiceInfo> services)
        {
            return services.First();
        }
    }
}
