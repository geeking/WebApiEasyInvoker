using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DispatchProxyAsync
{
    public abstract class DispatchProxyAsync
    {
        public static T Create<T, TProxy>(object[] ctorInitArgs = null) where TProxy : DispatchProxyAsync
            => (T)Create(typeof(TProxy), typeof(T), ctorInitArgs);


        public static object Create(Type baseType, Type interfaceType, object[] ctorInitArgs = null)
            => AsyncDispatchProxyGenerator.CreateProxyInstance(baseType, interfaceType, ctorInitArgs);

        public virtual void CtorInit(object[] args) { }

        public abstract object Invoke(MethodInfo targetMethod, object[] args);

        public abstract Task InvokeAsync(MethodInfo targetMethod, object[] args);

        public abstract Task<T> InvokeAsyncT<T>(MethodInfo targetMethod, object[] args);
    }
}