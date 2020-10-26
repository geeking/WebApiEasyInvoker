using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using WebApiEasyInvoker.Config;
using WebApiEasyInvoker.Models;
using WebApiEasyInvoker.Utils;

namespace WebApiEasyInvoker
{
    public class WebApiExecutor<ITarget> : DispatchProxyAsync.DispatchProxyAsync where ITarget : IWebApiInvoker<ITarget>
    {
        private readonly Type _targetType;
        private readonly HttpClient _httpClient;
        private HttpConfig _httpConfig;

        public WebApiExecutor()
        {
            _targetType = typeof(ITarget);
            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
            _httpClient = serviceProvider.GetService<IHttpClientFactory>().CreateClient();
        }

        private bool IsConfigMethod(MethodInfo methodInfo, object[] args)
        {
            if (methodInfo.Name == nameof(IWebApiInvoker<ITarget>.Config)
                && args != null
                && args.Length == 1
                && args[0] is HttpConfig)
            {
                return true;
            }
            return false;
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
            var requestMessage = HttpRequestUtil.BuildHttpRequestMessage(_httpConfig?.RequestHeaders, targetMethod, argInfos);
            _httpConfig?.Logger?.LogDebug("Request url:{0} {1}", requestMessage.Method, requestMessage.RequestUri.AbsoluteUri);

            var response = _httpClient.SendAsync(requestMessage).ConfigureAwait(false).GetAwaiter().GetResult();
            var content = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return ConvertUtil.DeserializeObject(targetMethod.ReturnType, content);
        }

        public override async Task InvokeAsync(MethodInfo targetMethod, object[] args)
        {
            //var requestMessage = BuildHttpRequest(targetMethod, args);
            var argInfos = InitArgumentInfos(targetMethod.GetParameters(), args);
            var requestMessage = HttpRequestUtil.BuildHttpRequestMessage(_httpConfig?.RequestHeaders, targetMethod, argInfos);
            _httpConfig?.Logger?.LogDebug("Request url:{0} {1}", requestMessage.Method, requestMessage.RequestUri.AbsoluteUri);

            await _httpClient.SendAsync(requestMessage);
        }

        public override async Task<T> InvokeAsyncT<T>(MethodInfo targetMethod, object[] args)
        {
            //var requestMessage = BuildHttpRequest(targetMethod, args);
            var argInfos = InitArgumentInfos(targetMethod.GetParameters(), args);
            var requestMessage = HttpRequestUtil.BuildHttpRequestMessage(_httpConfig?.RequestHeaders, targetMethod, argInfos);
            _httpConfig?.Logger?.LogDebug("Request url:{0} {1}", requestMessage.Method, requestMessage.RequestUri.AbsoluteUri);

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return ConvertUtil.DeserializeObject<T>(content);
        }
    }
}