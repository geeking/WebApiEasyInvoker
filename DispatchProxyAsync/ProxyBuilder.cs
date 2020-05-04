using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace DispatchProxyAsync
{
    internal partial class ProxyBuilder
    {
        private static readonly MethodInfo s_delegateInvoke = typeof(DispatchProxyHandler).GetMethod("InvokeHandle");
        private static readonly MethodInfo s_delegateInvokeAsync = typeof(DispatchProxyHandler).GetMethod("InvokeAsyncHandle");
        private static readonly MethodInfo s_delegateinvokeAsyncT = typeof(DispatchProxyHandler).GetMethod("InvokeAsyncHandleT");

        private ProxyAssembly _assembly;
        private TypeBuilder _tb;
        private Type _proxyBaseType;
        private List<FieldBuilder> _fields;

        internal ProxyBuilder(ProxyAssembly assembly, TypeBuilder tb, Type proxyBaseType)
        {
            _assembly = assembly;
            _tb = tb;
            _proxyBaseType = proxyBaseType;

            _fields = new List<FieldBuilder>();
            _fields.Add(tb.DefineField("_handler", typeof(DispatchProxyHandler), FieldAttributes.Private));
        }

        private static bool IsGenericTask(Type type)
        {
            var current = type;
            while (current != null)
            {
                if (current.GetTypeInfo().IsGenericType && current.GetGenericTypeDefinition() == typeof(Task<>))
                    return true;
                current = current.GetTypeInfo().BaseType;
            }
            return false;
        }

        private void Complete()
        {
            Type[] args = new Type[_fields.Count];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = _fields[i].FieldType;
            }

            ConstructorBuilder cb = _tb.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, args);
            ILGenerator il = cb.GetILGenerator();

            // chained ctor call
            ConstructorInfo baseCtor = _proxyBaseType.GetTypeInfo().DeclaredConstructors.SingleOrDefault(c => c.IsPublic && c.GetParameters().Length == 0);
            Debug.Assert(baseCtor != null);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, baseCtor);

            // store all the fields
            for (int i = 0; i < args.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg, i + 1);
                il.Emit(OpCodes.Stfld, _fields[i]);
            }

            il.Emit(OpCodes.Ret);
        }

        internal Type CreateType()
        {
            this.Complete();
            return _tb.CreateTypeInfo().AsType();
        }

        internal void AddInterfaceImpl(Type iface)
        {
            // If necessary, generate an attribute to permit visibility
            // to internal types.
            _assembly.EnsureTypeIsVisible(iface);

            _tb.AddInterfaceImplementation(iface);

            // AccessorMethods -> Metadata mappings.
            var propertyMap = new Dictionary<MethodInfo, PropertyAccessorInfo>(MethodInfoEqualityComparer.Instance);
            foreach (PropertyInfo pi in iface.GetRuntimeProperties())
            {
                var ai = new PropertyAccessorInfo(pi.GetMethod, pi.SetMethod);
                if (pi.GetMethod != null)
                    propertyMap[pi.GetMethod] = ai;
                if (pi.SetMethod != null)
                    propertyMap[pi.SetMethod] = ai;
            }

            var eventMap = new Dictionary<MethodInfo, EventAccessorInfo>(MethodInfoEqualityComparer.Instance);
            foreach (EventInfo ei in iface.GetRuntimeEvents())
            {
                var ai = new EventAccessorInfo(ei.AddMethod, ei.RemoveMethod, ei.RaiseMethod);
                if (ei.AddMethod != null)
                    eventMap[ei.AddMethod] = ai;
                if (ei.RemoveMethod != null)
                    eventMap[ei.RemoveMethod] = ai;
                if (ei.RaiseMethod != null)
                    eventMap[ei.RaiseMethod] = ai;
            }

            foreach (MethodInfo mi in iface.GetRuntimeMethods())
            {
                MethodBuilder mdb = AddMethodImpl(mi);
                PropertyAccessorInfo associatedProperty;
                if (propertyMap.TryGetValue(mi, out associatedProperty))
                {
                    if (MethodInfoEqualityComparer.Instance.Equals(associatedProperty.InterfaceGetMethod, mi))
                        associatedProperty.GetMethodBuilder = mdb;
                    else
                        associatedProperty.SetMethodBuilder = mdb;
                }

                EventAccessorInfo associatedEvent;
                if (eventMap.TryGetValue(mi, out associatedEvent))
                {
                    if (MethodInfoEqualityComparer.Instance.Equals(associatedEvent.InterfaceAddMethod, mi))
                        associatedEvent.AddMethodBuilder = mdb;
                    else if (MethodInfoEqualityComparer.Instance.Equals(associatedEvent.InterfaceRemoveMethod, mi))
                        associatedEvent.RemoveMethodBuilder = mdb;
                    else
                        associatedEvent.RaiseMethodBuilder = mdb;
                }
            }

            foreach (PropertyInfo pi in iface.GetRuntimeProperties())
            {
                PropertyAccessorInfo ai = propertyMap[pi.GetMethod ?? pi.SetMethod];
                PropertyBuilder pb = _tb.DefineProperty(pi.Name, pi.Attributes, pi.PropertyType, pi.GetIndexParameters().Select(p => p.ParameterType).ToArray());
                if (ai.GetMethodBuilder != null)
                    pb.SetGetMethod(ai.GetMethodBuilder);
                if (ai.SetMethodBuilder != null)
                    pb.SetSetMethod(ai.SetMethodBuilder);
            }

            foreach (EventInfo ei in iface.GetRuntimeEvents())
            {
                EventAccessorInfo ai = eventMap[ei.AddMethod ?? ei.RemoveMethod];
                EventBuilder eb = _tb.DefineEvent(ei.Name, ei.Attributes, ei.EventHandlerType);
                if (ai.AddMethodBuilder != null)
                    eb.SetAddOnMethod(ai.AddMethodBuilder);
                if (ai.RemoveMethodBuilder != null)
                    eb.SetRemoveOnMethod(ai.RemoveMethodBuilder);
                if (ai.RaiseMethodBuilder != null)
                    eb.SetRaiseMethod(ai.RaiseMethodBuilder);
            }
        }

        private MethodBuilder AddMethodImpl(MethodInfo mi)
        {
            ParameterInfo[] parameters = mi.GetParameters();
            Type[] paramTypes = ParamTypes(parameters, false);

            MethodBuilder mdb = _tb.DefineMethod(mi.Name, MethodAttributes.Public | MethodAttributes.Virtual, mi.ReturnType, paramTypes);
            if (mi.ContainsGenericParameters)
            {
                Type[] ts = mi.GetGenericArguments();
                string[] ss = new string[ts.Length];
                for (int i = 0; i < ts.Length; i++)
                {
                    ss[i] = ts[i].Name;
                }
                GenericTypeParameterBuilder[] genericParameters = mdb.DefineGenericParameters(ss);
                for (int i = 0; i < genericParameters.Length; i++)
                {
                    genericParameters[i].SetGenericParameterAttributes(ts[i].GetTypeInfo().GenericParameterAttributes);
                }
            }
            ILGenerator il = mdb.GetILGenerator();

            ParametersArray args = new ParametersArray(il, paramTypes);

            // object[] args = new object[paramCount];
            il.Emit(OpCodes.Nop);
            GenericArray<object> argsArr = new GenericArray<object>(il, ParamTypes(parameters, true).Length);

            for (int i = 0; i < parameters.Length; i++)
            {
                // args[i] = argi;
                if (!parameters[i].IsOut)
                {
                    argsArr.BeginSet(i);
                    args.Get(i);
                    argsArr.EndSet(parameters[i].ParameterType);
                }
            }

            // object[] packed = new object[PackedArgs.PackedTypes.Length];
            GenericArray<object> packedArr = new GenericArray<object>(il, PackedArgs.PackedTypes.Length);

            // packed[PackedArgs.DispatchProxyPosition] = this;
            packedArr.BeginSet(GlobalConst.DISPATCH_PROXY_POSITION);
            il.Emit(OpCodes.Ldarg_0);
            packedArr.EndSet(typeof(DispatchProxyAsync));

            // packed[PackedArgs.DeclaringTypePosition] = typeof(iface);
            MethodInfo Type_GetTypeFromHandle = typeof(Type).GetRuntimeMethod("GetTypeFromHandle", new Type[] { typeof(RuntimeTypeHandle) });
            int methodToken;
            Type declaringType;
            _assembly.GetTokenForMethod(mi, out declaringType, out methodToken);
            packedArr.BeginSet(GlobalConst.DECLARING_TYPE_POSITION);
            il.Emit(OpCodes.Ldtoken, declaringType);
            il.Emit(OpCodes.Call, Type_GetTypeFromHandle);
            packedArr.EndSet(typeof(object));

            // packed[PackedArgs.MethodTokenPosition] = iface method token;
            packedArr.BeginSet(GlobalConst.METHOD_TOKEN_POSITION);
            il.Emit(OpCodes.Ldc_I4, methodToken);
            packedArr.EndSet(typeof(Int32));

            // packed[PackedArgs.ArgsPosition] = args;
            packedArr.BeginSet(GlobalConst.ARGS_POSITION);
            argsArr.Load();
            packedArr.EndSet(typeof(object[]));

            // packed[PackedArgs.GenericTypesPosition] = mi.GetGenericArguments();
            if (mi.ContainsGenericParameters)
            {
                packedArr.BeginSet(GlobalConst.GENERIC_TYPES_POSITION);
                Type[] genericTypes = mi.GetGenericArguments();
                GenericArray<Type> typeArr = new GenericArray<Type>(il, genericTypes.Length);
                for (int i = 0; i < genericTypes.Length; ++i)
                {
                    typeArr.BeginSet(i);
                    il.Emit(OpCodes.Ldtoken, genericTypes[i]);
                    il.Emit(OpCodes.Call, Type_GetTypeFromHandle);
                    typeArr.EndSet(typeof(Type));
                }
                typeArr.Load();
                packedArr.EndSet(typeof(Type[]));
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.IsByRef)
                {
                    args.BeginSet(i);
                    argsArr.Get(i);
                    args.EndSet(i, typeof(object));
                }
            }

            MethodInfo invokeMethod = s_delegateInvoke;
            if (mi.ReturnType == typeof(Task))
            {
                invokeMethod = s_delegateInvokeAsync;
            }
            if (IsGenericTask(mi.ReturnType))
            {
                var returnTypes = mi.ReturnType.GetGenericArguments();
                invokeMethod = s_delegateinvokeAsyncT.MakeGenericMethod(returnTypes);
            }

            // Call AsyncDispatchProxyGenerator.Invoke(object[]), InvokeAsync or InvokeAsyncT
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, _fields[GlobalConst.InvokeActionFieldAndCtorParameterIndex]);
            packedArr.Load();
            il.Emit(OpCodes.Callvirt, invokeMethod);
            if (mi.ReturnType != typeof(void))
            {
                BuildUtil.Convert(il, typeof(object), mi.ReturnType, false);
            }
            else
            {
                il.Emit(OpCodes.Pop);
            }

            il.Emit(OpCodes.Ret);

            _tb.DefineMethodOverride(mdb, mi);
            return mdb;
        }

        private static Type[] ParamTypes(ParameterInfo[] parms, bool noByRef)
        {
            Type[] types = new Type[parms.Length];
            for (int i = 0; i < parms.Length; i++)
            {
                types[i] = parms[i].ParameterType;
                if (noByRef && types[i].IsByRef)
                    types[i] = types[i].GetElementType();
            }
            return types;
        }
        
    }

}