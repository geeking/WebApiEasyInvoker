using System;
using System.Collections.Generic;
using System.Text;
using WebApiEasyInvoker.Consul.LoadBalance;

namespace WebApiEasyInvoker.Consul.Services
{
    interface IConsulServiceInfoProvider
    {
        ServiceInfo GetServiceInfo(string serviceName, ILoadBalance loadBalance);
    }
}
