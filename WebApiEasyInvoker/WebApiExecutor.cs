﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using WebApiEasyInvoker.Config;
using WebApiEasyInvoker.Interfaces;
using WebApiEasyInvoker.Models;
using WebApiEasyInvoker.Utils;

namespace WebApiEasyInvoker
{
    public class WebApiExecutor<ITarget> : DispatchProxyAsync.DispatchProxyAsync where ITarget : IWebApiInvoker<ITarget>
    {
        private HttpClient _httpClient;
        private HttpConfig _httpConfig;
        private IUrlBuilder _urlBuilder;
        private ILogger<ITarget> _logger;
        public WebApiExecutor()
        {
        }

        private bool IsConfigMethod(MethodInfo methodInfo, object[] args)
        {
            return (methodInfo.Name == nameof(IWebApiInvoker<ITarget>.Config)
                && args != null
                && args.Length == 1
                && args[0] is HttpConfig);
        }

        private List<MethodArgumentInfo> InitArgumentInfos(ParameterInfo[] paramInfos, object[] values)
        {
            var argumentInfos = new List<MethodArgumentInfo>();

            if (paramInfos == null || values == null || paramInfos.Length != values.Length)
            {
                return argumentInfos;
            }
            for (int i = 0; i < paramInfos.Length; i++)
            {
                argumentInfos.Add(new MethodArgumentInfo
                {
                    Name = paramInfos[i].Name,
                    Value = values[i],
                    Attributes = paramInfos[i].GetCustomAttributes(),
                    Type = paramInfos[i].ParameterType,
                    Used = false
                });
            }
            return argumentInfos;
        }


        public override void CtorInit(object[] args)
        {
            if (args != null)
            {
                var provider = args[0] as ServiceProvider;
                var clientFactory = provider.GetService<IHttpClientFactory>();
                Debug.Assert(clientFactory != null);
                _httpClient = clientFactory.CreateClient();
                _logger = provider.GetService<ILogger<ITarget>>();
                _urlBuilder = provider.GetService<IUrlBuilder>();
            }
        }

        public override object Invoke(MethodInfo targetMethod, object[] args)
        {
            //if the method is 'Config',then save the config data and return this instance for the next invoke
            if (IsConfigMethod(targetMethod, args))
            {
                _httpConfig = args[0] as HttpConfig;
                _httpClient.Timeout = _httpConfig.Timeout;
                return this;
            }
            var argInfos = InitArgumentInfos(targetMethod.GetParameters(), args);
            var urlTemplate = _urlBuilder.GetUrlTemplate(targetMethod);

            var requestMessage = HttpRequestUtil.BuildHttpRequestMessage(_httpConfig?.RequestHeaders, argInfos, urlTemplate);
            _logger?.LogDebug("Request url:{0} {1}", requestMessage.Method, requestMessage.RequestUri.AbsoluteUri);

            var response = _httpClient.SendAsync(requestMessage).ConfigureAwait(false).GetAwaiter().GetResult();
            var content = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return ConvertUtil.DeserializeObject(targetMethod.ReturnType, content);
        }

        public override async Task InvokeAsync(MethodInfo targetMethod, object[] args)
        {
            var argInfos = InitArgumentInfos(targetMethod.GetParameters(), args);
            var urlTemplate = _urlBuilder.GetUrlTemplate(targetMethod);

            var requestMessage = HttpRequestUtil.BuildHttpRequestMessage(_httpConfig?.RequestHeaders, argInfos, urlTemplate);
            _logger?.LogDebug("Request url:{0} {1}", requestMessage.Method, requestMessage.RequestUri.AbsoluteUri);

            await _httpClient.SendAsync(requestMessage);
        }

        public override async Task<T> InvokeAsyncT<T>(MethodInfo targetMethod, object[] args)
        {
            var argInfos = InitArgumentInfos(targetMethod.GetParameters(), args);
            var urlTemplate = _urlBuilder.GetUrlTemplate(targetMethod);

            var requestMessage = HttpRequestUtil.BuildHttpRequestMessage(_httpConfig?.RequestHeaders, argInfos, urlTemplate);
            _logger?.LogDebug("Request url:{0} {1}", requestMessage.Method, requestMessage.RequestUri.AbsoluteUri);

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return ConvertUtil.DeserializeObject<T>(content);
        }
    }
}