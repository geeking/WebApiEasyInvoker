using System.Net.Http;

namespace WebApiEasyInvoker.Attributes
{
    public enum HttpMethodKind
    {
        Get,
        Post,
        Put,
        Delete,
        Patch,
        Head,
        Options
    }

    public static class HttpMethodKindExtension
    {
        public static HttpMethod ToHttpMethod(this HttpMethodKind httpMethodKind)
        {
            switch (httpMethodKind)
            {
                case HttpMethodKind.Get:
                    return HttpMethod.Get;

                case HttpMethodKind.Post:
                    return HttpMethod.Post;

                case HttpMethodKind.Put:
                    return HttpMethod.Put;

                case HttpMethodKind.Delete:
                    return HttpMethod.Delete;

                case HttpMethodKind.Patch:
                    return new HttpMethod("PATCH");

                case HttpMethodKind.Head:
                    return new HttpMethod("HEAD");

                case HttpMethodKind.Options:
                    return new HttpMethod("OPTIONS");

                default:
                    return HttpMethod.Get;
            }
        }
    }
}