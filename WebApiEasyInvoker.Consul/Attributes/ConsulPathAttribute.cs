using System;
using System.Net.Http;
using WebApiEasyInvoker.Attributes;

namespace WebApiEasyInvoker.Consul.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]

    public sealed class ConsulPathAttribute : Attribute
    {
        public ConsulPathAttribute(string path, HttpMethodKind httpMethodKind = HttpMethodKind.Get)
        {
            Path = path;
            HttpMethod = httpMethodKind.ToHttpMethod();
        }

        public string Path { get; }
        public HttpMethod HttpMethod { get; private set; }

    }
}
