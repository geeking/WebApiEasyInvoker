using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WebApiEasyInvoker.Consul.LoadBalance
{
    static class LoadBalanceRepository
    {
        private static Dictionary<string, ILoadBalance> _loadBalanceDic = new Dictionary<string, ILoadBalance>();

        public static void LoadData(Dictionary<string,ILoadBalance> data)
        {
            _loadBalanceDic = data;
        }

        public static ILoadBalance GetLoadBalance(string ticket)
        {
            if (_loadBalanceDic.ContainsKey(ticket))
            {
                return _loadBalanceDic[ticket];
            }
            else
            {
                return null;
            }
            
        }
    }
}
