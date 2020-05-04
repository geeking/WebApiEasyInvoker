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

            switch (httpMethodKind)
            {
                case HttpMethodKind.Get:
                    HttpMethod = HttpMethod.Get;
                    break;

                case HttpMethodKind.Post:
                    HttpMethod = HttpMethod.Post;
                    break;

                case HttpMethodKind.Put:
                    HttpMethod = HttpMethod.Put;
                    break;

                case HttpMethodKind.Delete:
                    HttpMethod = HttpMethod.Delete;
                    break;

                case HttpMethodKind.Patch:
                    HttpMethod = new HttpMethod("PATCH");
                    break;

                case HttpMethodKind.Head:
                    HttpMethod = new HttpMethod("HEAD");
                    break;

                case HttpMethodKind.Options:
                    HttpMethod = new HttpMethod("OPTIONS");
                    break;

                default:
                    HttpMethod = HttpMethod.Get;
                    break;
            }
        }

        public string Url { get; }
        public HttpMethod HttpMethod { get; private set; }
    }
}