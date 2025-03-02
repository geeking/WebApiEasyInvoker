using System.Linq;
using WebApiEasyInvoker.Consul.LoadBalance;

namespace WebApiEasyInvoker.Consul.Services
{
    internal class ConsulServiceInfoProvider : IConsulServiceInfoProvider
    {
        private readonly IConsulServiceRefresh _consulServiceRefresh;

        public ConsulServiceInfoProvider(IConsulServiceRefresh consulServiceRefresh)
        {
            _consulServiceRefresh = consulServiceRefresh;
        }

        public ServiceInfo GetServiceInfo(string serviceName, ILoadBalance loadBalance)
        {
            var infos = _consulServiceRefresh.GetServiceInfos(serviceName);
            if (infos == null || infos.Count() < 1)
            {
                return null;
            }
            ServiceInfo result = loadBalance.ChoseOne(infos);
            result.AccessCount++;
            return result;
        }
    }
}