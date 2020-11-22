using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiEasyInvoker.Consul.Services
{
    interface IConsulServiceInfoProvider
    {
        ServiceInfo GetServiceInfo(string serviceName, BalanceType balanceType);
    }
}
