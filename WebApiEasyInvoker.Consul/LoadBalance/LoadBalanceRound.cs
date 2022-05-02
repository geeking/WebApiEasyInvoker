using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WebApiEasyInvoker.Consul.LoadBalance
{
    class LoadBalanceRound : ILoadBalance
    {
        private int _count = 0;
        public ServiceInfo ChoseOne(IEnumerable<ServiceInfo> services)
        {
            Interlocked.CompareExchange(ref _count, 0, int.MaxValue);
            var count = Interlocked.Increment(ref _count);
            var index = count % services.Count();
            return services.ElementAt(index);
        }
    }
}
