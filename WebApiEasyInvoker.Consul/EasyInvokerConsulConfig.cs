using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiEasyInvoker.Consul
{
    public sealed class EasyInvokerConsulConfig
    {
        public string Address { get; set; }
        public string Token { get; set; }

        public int RefreshMilliseconds { get; set; } = 4000;
    }
}
