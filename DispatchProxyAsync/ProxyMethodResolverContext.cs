using System.Reflection;

namespace DispatchProxyAsync
{
    internal class ProxyMethodResolverContext
    {
        public PackedArgs Packed { get; }
        public MethodBase Method { get; }

        public ProxyMethodResolverContext(PackedArgs packed, MethodBase method)
        {
            Packed = packed;
            Method = method;
        }
    }
}