using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DispatchProxyAsync
{
    public abstract class DispatchProxyAsync
    {
        public static T Create<T, TProxy>() where TProxy : DispatchProxyAsync
        {
            return (T)AsyncDispatchProxyGenerator.CreateProxyInstance(typeof(TProxy), typeof(T));
        }

        public static object Create(Type baseType, Type interfaceType) => AsyncDispatchProxyGenerator.CreateProxyInstance(baseType, interfaceType);

        public abstract object Invoke(MethodInfo targetMethod, object[] args);

        public abstract Task InvokeAsync(MethodInfo targetMethod, object[] args);

        public abstract Task<T> InvokeAsyncT<T>(MethodInfo targetMethod, object[] args);
    }
}