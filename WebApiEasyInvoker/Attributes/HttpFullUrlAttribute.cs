using System;

namespace WebApiEasyInvoker.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HttpFullUrlAttribute : HttpUrlAttribute
    {
        public HttpFullUrlAttribute(string url, HttpMethodKind httpMethodKind = HttpMethodKind.Get) : base(url, httpMethodKind)
        {
        }
    }
}