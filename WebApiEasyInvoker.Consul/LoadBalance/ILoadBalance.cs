using System.Collections.Generic;
using System.Text;

namespace WebApiEasyInvoker.Consul.LoadBalance
{
    interface ILoadBalance
    {
        ServiceInfo ChoseOne(IEnumerable<ServiceInfo> services);
    }
}
