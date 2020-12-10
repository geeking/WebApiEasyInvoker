using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiEasyInvoker.Consul.LoadBalance
{
    public enum BalanceType
    {
        First,
        Last,
        Round,
        Random,
        LeastConnection,
    }
}
