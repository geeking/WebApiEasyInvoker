using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using WebApiEasyInvoker.Models;

namespace WebApiEasyInvoker.Interfaces
{
    public interface IUrlBuilder
    {
        UrlTemplate GetUrlTemplate(MethodInfo methodInfo);
    }
}
