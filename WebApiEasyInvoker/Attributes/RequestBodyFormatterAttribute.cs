using System;
using WebApiEasyInvoker.Interfaces;

namespace WebApiEasyInvoker.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class RequestBodyFormatterAttribute : Attribute
    {
        public RequestBodyFormatterAttribute(Type formatterType)
        {
            if (!formatterType.IsAssignableFrom(typeof(IRequestBodyFormatter)))
            {
                throw new ArgumentException($"the formatterType({formatterType.Name}) must implement {nameof(IRequestBodyFormatter)} interface");
            }
            FormatterType = formatterType;
        }

        public Type FormatterType { get; }
    }
}