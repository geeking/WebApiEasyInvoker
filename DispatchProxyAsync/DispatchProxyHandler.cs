using System.Threading.Tasks;

namespace DispatchProxyAsync
{
    public class DispatchProxyHandler
    {
        public DispatchProxyHandler()
        {
        }

        public object InvokeHandle(object[] args)
        {
            return AsyncDispatchProxyGenerator.Invoke(args);
        }

        public Task InvokeAsyncHandle(object[] args)
        {
            return AsyncDispatchProxyGenerator.InvokeAsync(args);
        }

        public Task<T> InvokeAsyncHandleT<T>(object[] args)
        {
            return AsyncDispatchProxyGenerator.InvokeAsync<T>(args);
        }
    }
}