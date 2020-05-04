using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiEasyInvoker.Utils
{
    internal static class TypeExtension
    {
        public static bool IsValueOrStringType(this Type type)
        {
            return type.IsValueType || type.IsAssignableFrom(typeof(string));
        }
    }
}
