using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiEasyInvoker.Consul.LoadBalance
{
    class LoadBalanceRandom : ILoadBalance
    {
        private Random _random = new Random(DateTime.Now.Millisecond);

        public ServiceInfo ChoseOne(IEnumerable<ServiceInfo> services)
        {
            return services.ElementAt(_random.Next(0, services.Count()));
        }
    }
}
