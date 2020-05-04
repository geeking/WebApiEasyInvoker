using System;

namespace DispatchProxyAsync
{
    internal class PackedArgs
    {
        internal static readonly Type[] PackedTypes = new Type[] {
                typeof(object),
                typeof(Type),
                typeof(int),
                typeof(object[]),
                typeof(Type[]),
                typeof(object)
            };

        private readonly object[] _args;

        internal PackedArgs() : this(new object[PackedTypes.Length])
        {
        }

        internal PackedArgs(object[] args)
        {
            _args = args;
        }

        internal DispatchProxyAsync DispatchProxy { get { return (DispatchProxyAsync)_args[GlobalConst.DISPATCH_PROXY_POSITION]; } }
        internal Type DeclaringType { get { return (Type)_args[GlobalConst.DECLARING_TYPE_POSITION]; } }
        internal int MethodToken { get { return (int)_args[GlobalConst.METHOD_TOKEN_POSITION]; } }
        internal object[] Args { get { return (object[])_args[GlobalConst.ARGS_POSITION]; } }
        internal Type[] GenericTypes { get { return (Type[])_args[GlobalConst.GENERIC_TYPES_POSITION]; } }
        internal object ReturnValue { /*get { return args[ReturnValuePosition]; }*/ set { _args[GlobalConst.RETURN_VALUE_POSITION] = value; } }
    }
}