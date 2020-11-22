using System;

namespace WebApiEasyInvoker.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class HttpHostAttribute : Attribute
    {
        public HttpHostAttribute(string httpHost)
        {
            HttpHost = httpHost;
        }

        public string HttpHost { get; }
    }
}