using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace WebApiEasyInvoker.Models
{
    public class UrlTemplate
    {
        public string Host { get; set; }
        public string Url { get; set; }
        public HttpMethod HttpMethod { get; set; }
    }
}
