using System.Reflection;
using WebApiEasyInvoker.Models;
using WebApiEasyInvoker.Utils;

namespace WebApiEasyInvoker.Interfaces.Impl
{
    internal class UrlBuilderDefault : IUrlBuilder
    {
        public UrlTemplate GetUrlTemplate(MethodInfo methodInfo)
            => HttpRequestUtil.BuildUrlTemplate(methodInfo);
    }
}