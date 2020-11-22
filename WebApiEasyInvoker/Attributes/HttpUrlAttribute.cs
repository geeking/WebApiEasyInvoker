using System;
using System.Net.Http;

namespace WebApiEasyInvoker.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpUrlAttribute : Attribute
    {
        public HttpUrlAttribute(string url, HttpMethodKind httpMethodKind = HttpMethodKind.Get)
        {
            Url = url;

            HttpMethod = httpMethodKind.ToHttpMethod();
        }

        public string Url { get; }
        public HttpMethod HttpMethod { get; private set; }
    }
}