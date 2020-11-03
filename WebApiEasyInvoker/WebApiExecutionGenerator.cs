using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace WebApiEasyInvoker
{
    public class WebApiExecutionGenerator
    {
        /// <summary>
        /// build the executor for target interface
        /// </summary>
        /// <returns></returns>
        public static ITarget Create<ITarget>() where ITarget : IWebApiInvoker<ITarget>
        {
            return DispatchProxyAsync.DispatchProxyAsync.Create<ITarget, WebApiExecutor<ITarget>>();
        }

        /// <summary>
        /// build the executor for target interface
        /// </summary>
        /// <returns></returns>
        public static object Create(Type baseType, Type interfaceType)
        {
            return DispatchProxyAsync.DispatchProxyAsync.Create(baseType, interfaceType);
        }
    }

}