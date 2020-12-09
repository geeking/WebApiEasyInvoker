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
            if (_count == int.MaxValue)
            {
                _count = 0;
            }
            Interlocked.Increment(ref _count);
            var index = _count % services.Count();
            return services.ElementAt(index);
        }
    }
}
