using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiEasyInvoker.Consul
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
