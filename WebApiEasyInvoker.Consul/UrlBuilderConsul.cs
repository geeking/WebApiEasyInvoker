﻿using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using WebApiEasyInvoker.Consul.Attributes;
using WebApiEasyInvoker.Consul.LoadBalance;
using WebApiEasyInvoker.Consul.Services;
using WebApiEasyInvoker.Interfaces;
using WebApiEasyInvoker.Models;
using WebApiEasyInvoker.Utils;

namespace WebApiEasyInvoker.Consul
{
    internal class UrlBuilderConsul : IUrlBuilder
    {
        private readonly IConsulServiceInfoProvider _consulInfoProvider;
        private readonly IConfiguration _configuration;

        public UrlBuilderConsul(IConsulServiceInfoProvider consulInfoProvider,
            IConfiguration configuration)
        {
            _consulInfoProvider = consulInfoProvider;
            _configuration = configuration;
        }

        public UrlTemplate GetUrlTemplate(MethodInfo methodInfo)
        {
            var serviceAttribute = methodInfo.DeclaringType.GetCustomAttribute<ConsulServiceAttribute>();
            if (serviceAttribute != null)
            {
                var serviceName = serviceAttribute.ServiceName;
                var pathAttribute = methodInfo.GetCustomAttribute<ConsulPathAttribute>();
                if (pathAttribute != null)
                {
                    var urlPath = pathAttribute.Path;
                    var httpMethod = pathAttribute.HttpMethod;
                    var loadBalance = LoadBalanceRepository.GetLoadBalance(methodInfo.DeclaringType.FullName);
                    var serviceInfo = _consulInfoProvider.GetServiceInfo(serviceName, loadBalance);
                    if (serviceInfo == null)
                    {
                        throw new Exception($"Can't find {serviceName} in Consul");
                    }
                    var host = serviceInfo.Address.StartsWith("http") ? serviceInfo.Address : $"http://{serviceInfo.Address}";
                    return new UrlTemplate
                    {
                        Host = $"{host}:{serviceInfo.ServicePort}",
                        HttpMethod = httpMethod,
                        Url = urlPath
                    };
                }
            }

            return HttpRequestUtil.BuildUrlTemplate(methodInfo, _configuration);
        }
    }
}