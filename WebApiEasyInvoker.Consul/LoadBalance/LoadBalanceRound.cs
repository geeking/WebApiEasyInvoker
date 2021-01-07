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
            Interlocked.Increment(ref _count);
            //At a very low probability, the index may be the same,but does't matter
            var index = _count % services.Count();
            return services.ElementAt(index);
        }
    }
}
