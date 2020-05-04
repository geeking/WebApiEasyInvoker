using System;

namespace WebApiEasyInvoker.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ToBodyAttribute : Attribute
    {
        public ToBodyAttribute(FormatType formatType = FormatType.Json)
        {
            FormatType = formatType;
        }

        public FormatType FormatType { get; }
    }

    public enum FormatType
    {
        Json = 0,
        Form
    }
}