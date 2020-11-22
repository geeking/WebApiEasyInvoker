using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using WebApiEasyInvoker.Consul.Attributes;
using WebApiEasyInvoker.Consul.Services;
using WebApiEasyInvoker.Interfaces;
using WebApiEasyInvoker.Models;
using WebApiEasyInvoker.Utils;

namespace WebApiEasyInvoker.Consul
{
    class UrlBuilderConsul : IUrlBuilder
    {
        private readonly IConsulServiceInfoProvider _consulInfoProvider;

        public UrlBuilderConsul(IConsulServiceInfoProvider consulInfoProvider)
        {
            _consulInfoProvider = consulInfoProvider;
        }


        public UrlTemplate GetUrlTemplate(MethodInfo methodInfo)
        {
            var serviceAttribute = methodInfo.DeclaringType.GetCustomAttribute<ConsulServiceAttribute>();
            if (serviceAttribute != null)
            {
                var serviceName = serviceAttribute.ServiceName;
                var balanceType = serviceAttribute.BalanceType;
                var pathAttribute = methodInfo.GetCustomAttribute<ConsulPathAttribute>();
                if (pathAttribute != null)
                {
                    var urlPath = pathAttribute.Path;
                    var httpMethod = pathAttribute.HttpMethod;

                    var serviceInfo = _consulInfoProvider.GetServiceInfo(serviceName, balanceType);
                    if (serviceInfo == null)
                    {
                        throw new Exception($"Can't find {serviceName} in Consul");
                    }
                    //todo
                }
            }

            return HttpRequestUtil.BuildUrlTemplate(methodInfo);
        }
    }
}
