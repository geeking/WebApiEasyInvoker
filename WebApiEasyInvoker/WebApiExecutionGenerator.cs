using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using WebApiEasyInvoker.Interfaces;
using WebApiEasyInvoker.Interfaces.Impl;

namespace WebApiEasyInvoker
{
    public class WebApiExecutionGenerator
    {
        /// <summary>
        /// build the executor for target interface
        /// </summary>
        /// <returns></returns>
        public static ITarget Create<ITarget>(string httpClientName = "") where ITarget : IWebApiInvoker<ITarget>
        {
            ServiceCollection serviceCollections = new ServiceCollection();
            serviceCollections.AddScoped<IUrlBuilder, UrlBuilderDefault>();
            serviceCollections.AddHttpClient(httpClientName);
            var serviceProvider = serviceCollections.BuildServiceProvider();
            return DispatchProxyAsync.DispatchProxyAsync.Create<ITarget, WebApiExecutor<ITarget>>(new object[] {serviceProvider});
        }

        /// <summary>
        /// build the executor for target interface
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="ctorInitArgs"></param>
        /// <returns></returns>
        internal static object Create(Type interfaceType, object[] ctorInitArgs = null)
        {
            var baseType = typeof(WebApiExecutor<>).MakeGenericType(interfaceType);
            return DispatchProxyAsync.DispatchProxyAsync.Create(baseType, interfaceType, ctorInitArgs);
        }
    }
}