using System;
using WebApiEasyInvoker.Interfaces;

namespace WebApiEasyInvoker.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class BodyFormatterAttribute : Attribute
    {
        public BodyFormatterAttribute(Type formatterType)
        {
            if (!formatterType.IsAssignableFrom(typeof(IBodyFormatter)))
            {
                throw new ArgumentException($"the formatterType({formatterType.Name}) must implement {nameof(IBodyFormatter)} interface");
            }
            FormatterType = formatterType;
        }

        public Type FormatterType { get; }
    }
}