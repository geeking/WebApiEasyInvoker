using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiEasyInvoker.Consul
{
    public class ServiceInfo
    {
        public string ID { get; set; }
        public string ServiceName { get; set; }
        public string Address { get; set; }
        public int ServicePort { get; set; }
        public string Node { get; set; }
        public ulong AccessCount { get; set; }
    }
}
