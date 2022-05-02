using System;
using System.Net.Http;

namespace WebApiEasyInvoker.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpUrlAttribute : Attribute
    {
        public HttpUrlAttribute()
        {
            HttpMethod = HttpMethod.Get;
        }

        public HttpUrlAttribute(string url, HttpMethodKind httpMethodKind = HttpMethodKind.Get)
        {
            Url = url;

            HttpMethod = httpMethodKind.ToHttpMethod();
        }

        public string Url { get; }
        public HttpMethod HttpMethod { get; private set; }

        /// <summary>
        /// is the HttpHost read from configuartion.
        /// if set to true,the value of Url should be the node path of config.
        /// default is false
        /// </summary>
        public bool FromConfiguration { get; set; } = false;
    }
}