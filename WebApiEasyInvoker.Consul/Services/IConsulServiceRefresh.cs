using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiEasyInvoker.Consul.Services
{
    interface IConsulServiceRefresh
    {
        void LoadServices(IEnumerable<string> services);
        void Start(EasyInvokerConsulConfig consulConfig);
        IEnumerable<ServiceInfo> GetServiceInfos(string serviceName);
    }
}
