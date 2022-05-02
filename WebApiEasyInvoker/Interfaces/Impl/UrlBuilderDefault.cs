using Microsoft.Extensions.Configuration;
using System.Reflection;
using WebApiEasyInvoker.Models;
using WebApiEasyInvoker.Utils;

namespace WebApiEasyInvoker.Interfaces.Impl
{
    internal class UrlBuilderDefault : IUrlBuilder
    {
        private readonly IConfiguration _configuration;

        public UrlBuilderDefault(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public UrlTemplate GetUrlTemplate(MethodInfo methodInfo)
            => HttpRequestUtil.BuildUrlTemplate(methodInfo, _configuration);
    }
}