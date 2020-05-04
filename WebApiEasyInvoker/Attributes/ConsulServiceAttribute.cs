using System;

namespace WebApiEasyInvoker.Attributes
{
    /// <summary>
    /// set service name for witch register to consul
    /// so can auto get service host from consul
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class ConsulServiceAttribute : Attribute
    {
        public ConsulServiceAttribute(string serviceName)
        {
            ServiceName = serviceName;
        }

        public string ServiceName { get; }
    }
}