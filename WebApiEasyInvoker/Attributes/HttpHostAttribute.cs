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

        /// <summary>
        /// is the HttpHost read from configuartion.
        /// if set to true,the value of HttpHost should be the node path of config.
        /// default is false
        /// </summary>
        public bool FromConfiguration { get; set; } = false;
    }
}