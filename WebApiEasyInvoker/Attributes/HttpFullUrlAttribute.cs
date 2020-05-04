using System;

namespace WebApiEasyInvoker.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpFullUrlAttribute : HttpUrlAttribute
    {
        public HttpFullUrlAttribute(string url, HttpMethodKind httpMethodKind = HttpMethodKind.Get) : base(url, httpMethodKind)
        {
        }
    }
}