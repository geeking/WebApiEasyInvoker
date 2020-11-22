using Microsoft.Extensions.Logging;
using System;
using System.Net.Http.Headers;

namespace WebApiEasyInvoker.Config
{
    /// <summary>
    /// web request configration
    /// </summary>
    public class HttpConfig
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);
        public HttpRequestHeaders RequestHeaders { get; set; }
    }
}