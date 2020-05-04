using System;

namespace WebApiEasyInvoker.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ConsulPathAttribute : Attribute
    {
        public ConsulPathAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}