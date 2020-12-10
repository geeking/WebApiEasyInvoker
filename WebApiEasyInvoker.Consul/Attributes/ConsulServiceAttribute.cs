using System;
using System.Collections.Generic;
using System.Text;
using WebApiEasyInvoker.Consul.LoadBalance;

namespace WebApiEasyInvoker.Consul.Attributes
{
    /// <summary>
    /// set service name for witch register to consul
    /// so can auto get service host from consul
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class ConsulServiceAttribute : Attribute
    {
        public ConsulServiceAttribute(string serviceName, BalanceType balanceType = BalanceType.First)
        {
            ServiceName = serviceName;
            BalanceType = balanceType;
        }

        public string ServiceName { get; }
        public BalanceType BalanceType { get; }
    }
}
