using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace DispatchProxyAsync
{
    /// <summary>
    ///     Helper class to handle the IL EMIT for the generation of proxies.
    /// Much of this code was taken directly from the Silverlight proxy generation.
    /// Differences between this and the Silverlight version are:
    ///   1. This version is based on DispatchProxy from NET Native and CoreCLR, not RealProxy in Silverlight ServiceModel.
    ///
    ///     There are several notable differences between them.
    ///   2. Both DispatchProxy and RealProxy permit the caller to ask for a proxy specifying a pair of types:
    ///
    ///     the interface type to implement, and a base type.But they behave slightly differently:
    ///        - RealProxy generates a proxy type that derives from Object and * implements" all the base type's
    ///          interfaces plus all the interface type's interfaces.
    ///        - DispatchProxy generates a proxy type that * derives* from the base type and implements all
    ///          the interface type's interfaces.  This is true for both the CLR version in NET Native and this
    ///          version for CoreCLR.
    ///   3. DispatchProxy and RealProxy use different type hierarchies for the generated proxies:
    ///        - RealProxy type hierarchy is:
    ///              proxyType : proxyBaseType : object
    ///          Presumably the 'proxyBaseType' in the middle is to allow it to implement the base type's interfaces
    ///          explicitly, preventing collision for same name methods on the base and interface types.
    ///        - DispatchProxy hierarchy is:
    ///              proxyType : baseType(where baseType : DispatchProxy)
    ///          The generated DispatchProxy proxy type does not need to generate implementation methods
    ///          for the base type's interfaces, because the base type already must have implemented them.
    ///   4. RealProxy required a proxy instance to hold a backpointer to the RealProxy instance to mirror
    ///      the.Net Remoting design that required the proxy and RealProxy to be separate instances.
    ///      But the DispatchProxy design encourages the proxy type to* be* an DispatchProxy.  Therefore,
    ///      the proxy's 'this' becomes the equivalent of RealProxy's backpointer to RealProxy, so we were
    ///      able to remove an extraneous field and ctor arg from the DispatchProxy proxies.
    /// </summary>
    internal static class AsyncDispatchProxyGenerator
    {
        // Proxies are requested for a pair of types: base type and interface type.
        // The generated proxy will subclass the given base type and implement the interface type.
        // We maintain a cache keyed by 'base type' containing a dictionary keyed by interface type,
        // containing the generated proxy type for that pair.   There are likely to be few (maybe only 1)
        // base type in use for many interface types.
        // Note: this differs from Silverlight's RealProxy implementation which keys strictly off the
        // interface type.  But this does not allow the same interface type to be used with more than a
        // single base type.  The implementation here permits multiple interface types to be used with
        // multiple base types, and the generated proxy types will be unique.
        // This cache of generated types grows unbounded, one element per unique T/ProxyT pair.
        // This approach is used to prevent regenerating identical proxy types for identical T/Proxy pairs,
        // which would ultimately be a more expensive leak.
        // Proxy instances are not cached.  Their lifetime is entirely owned by the caller of DispatchProxy.Create.
        private static readonly Dictionary<Type, Dictionary<Type, Type>> s_baseTypeAndInterfaceToGeneratedProxyType =
            new Dictionary<Type, Dictionary<Type, Type>>();

        private static readonly ProxyAssembly s_proxyAssembly = new ProxyAssembly();
        private static readonly MethodInfo s_dispatchProxyInvokeMethod = typeof(DispatchProxyAsync).GetTypeInfo().GetDeclaredMethod("Invoke");
        private static readonly MethodInfo s_dispatchProxyInvokeAsyncMethod = typeof(DispatchProxyAsync).GetTypeInfo().GetDeclaredMethod("InvokeAsync");
        private static readonly MethodInfo s_dispatchProxyInvokeAsyncTMethod = typeof(DispatchProxyAsync).GetTypeInfo().GetDeclaredMethod("InvokeAsyncT");

        // Returns a new instance of a proxy the derives from 'baseType' and implements 'interfaceType'
        internal static object CreateProxyInstance(Type baseType, Type interfaceType)
        {
            Debug.Assert(baseType != null);
            Debug.Assert(interfaceType != null);

            Type proxiedType = GetProxyType(baseType, interfaceType);
            return Activator.CreateInstance(proxiedType, new DispatchProxyHandler());
        }

        private static Type GetProxyType(Type baseType, Type interfaceType)
        {
            lock (s_baseTypeAndInterfaceToGeneratedProxyType)
            {
                Dictionary<Type, Type> interfaceToProxy = null;
                if (!s_baseTypeAndInterfaceToGeneratedProxyType.TryGetValue(baseType, out interfaceToProxy))
                {
                    interfaceToProxy = new Dictionary<Type, Type>();
                    s_baseTypeAndInterfaceToGeneratedProxyType[baseType] = interfaceToProxy;
                }

                Type generatedProxy = null;
                if (!interfaceToProxy.TryGetValue(interfaceType, out generatedProxy))
                {
                    generatedProxy = GenerateProxyType(baseType, interfaceType);
                    interfaceToProxy[interfaceType] = generatedProxy;
                }

                return generatedProxy;
            }
        }

        /// <summary>
        /// Unconditionally generates a new proxy type derived from 'baseType' and implements 'interfaceType'
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        private static Type GenerateProxyType(Type baseType, Type interfaceType)
        {
            // Parameter validation is deferred until the point we need to create the proxy.
            // This prevents unnecessary overhead revalidating cached proxy types.
            TypeInfo baseTypeInfo = baseType.GetTypeInfo();

            // The interface type must be an interface, not a class
            if (!interfaceType.GetTypeInfo().IsInterface)
            {
                // "T" is the generic parameter seen via the public contract
                throw new ArgumentException($"InterfaceType_Must_Be_Interface, {interfaceType.FullName}", "T");
            }

            // The base type cannot be sealed because the proxy needs to subclass it.
            if (baseTypeInfo.IsSealed)
            {
                // "TProxy" is the generic parameter seen via the public contract
                throw new ArgumentException($"BaseType_Cannot_Be_Sealed, {baseTypeInfo.FullName}", "TProxy");
            }

            // The base type cannot be abstract
            if (baseTypeInfo.IsAbstract)
            {
                throw new ArgumentException($"BaseType_Cannot_Be_Abstract {baseType.FullName}", "TProxy");
            }

            // The base type must have a public default ctor
            if (!baseTypeInfo.DeclaredConstructors.Any(c => c.IsPublic && c.GetParameters().Length == 0))
            {
                throw new ArgumentException($"BaseType_Must_Have_Default_Ctor {baseType.FullName}", "TProxy");
            }

            // Create a type that derives from 'baseType' provided by caller
            ProxyBuilder pb = s_proxyAssembly.CreateProxy("generatedProxy", baseType);

            foreach (Type t in interfaceType.GetTypeInfo().ImplementedInterfaces)
                pb.AddInterfaceImpl(t);

            pb.AddInterfaceImpl(interfaceType);

            Type generatedProxyType = pb.CreateType();
            return generatedProxyType;
        }

        private static ProxyMethodResolverContext Resolve(object[] args)
        {
            PackedArgs packed = new PackedArgs(args);
            MethodBase method = s_proxyAssembly.ResolveMethodToken(packed.DeclaringType, packed.MethodToken);
            if (method.IsGenericMethodDefinition)
                method = ((MethodInfo)method).MakeGenericMethod(packed.GenericTypes);

            return new ProxyMethodResolverContext(packed, method);
        }

        public static object Invoke(object[] args)
        {
            var context = Resolve(args);

            // Call (protected method) DispatchProxyAsync.Invoke()
            object returnValue = null;
            try
            {
                Debug.Assert(s_dispatchProxyInvokeMethod != null);
                returnValue = s_dispatchProxyInvokeMethod.Invoke(context.Packed.DispatchProxy,
                                                                       new object[] { context.Method, context.Packed.Args });
                context.Packed.ReturnValue = returnValue;
            }
            catch (TargetInvocationException tie)
            {
                ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
            }

            return returnValue;
        }

        public static async Task InvokeAsync(object[] args)
        {
            var context = Resolve(args);

            // Call (protected Task method) NetCoreStackDispatchProxy.InvokeAsync()
            try
            {
                Debug.Assert(s_dispatchProxyInvokeAsyncMethod != null);
                await (Task)s_dispatchProxyInvokeAsyncMethod.Invoke(context.Packed.DispatchProxy,
                                                                       new object[] { context.Method, context.Packed.Args });
            }
            catch (TargetInvocationException tie)
            {
                ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
            }
        }

        public static async Task<T> InvokeAsync<T>(object[] args)
        {
            var context = Resolve(args);

            // Call (protected Task<T> method) NetCoreStackDispatchProxy.InvokeAsync<T>()
            T returnValue = default(T);
            try
            {
                Debug.Assert(s_dispatchProxyInvokeAsyncTMethod != null);
                var genericmethod = s_dispatchProxyInvokeAsyncTMethod.MakeGenericMethod(typeof(T));
                returnValue = await (Task<T>)genericmethod.Invoke(context.Packed.DispatchProxy,
                                                                       new object[] { context.Method, context.Packed.Args });
                context.Packed.ReturnValue = returnValue;
            }
            catch (TargetInvocationException tie)
            {
                ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
            }
            return returnValue;
        }
    }
}