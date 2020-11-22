using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiEasyInvoker.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class ToQueryAttribute : Attribute
    {
    }
}
