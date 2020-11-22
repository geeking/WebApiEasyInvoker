using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiEasyInvoker.Consul
{
    static class GlobalValue
    {
        private static EasyInvokerConsulConfig _easyInvokerConsulConfig;


        public static void SetConsulConfig(EasyInvokerConsulConfig consulConfig)
        {
            _easyInvokerConsulConfig = consulConfig;
        }

        public static EasyInvokerConsulConfig GetConsulConfig() => _easyInvokerConsulConfig;
    }
}
