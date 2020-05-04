using System.Collections.Concurrent;
using System.Reflection;

namespace WebApiEasyInvoker
{
    public class WebApiExecutionGenerator<ITarget> where ITarget : IWebApiInvoker<ITarget>
    {
        private static readonly ConcurrentDictionary<string, IWebApiInvoker<ITarget>> _localCache = new ConcurrentDictionary<string, IWebApiInvoker<ITarget>>();

        /// <summary>
        /// build the executor for target interface
        /// </summary>
        /// <returns></returns>
        public static ITarget Create()
        {
            ITarget target;
            var key = typeof(ITarget).FullName;
            if (_localCache.ContainsKey(key))
            {
                if (_localCache.TryGetValue(key, out IWebApiInvoker<ITarget> value))
                {
                    target = (ITarget)value;
                    return target;
                }
            }
            target = DispatchProxyAsync.DispatchProxyAsync.Create<ITarget, WebApiExecutor<ITarget>>();
            _localCache.TryAdd(key, target);
            return target;
        }
    }
}