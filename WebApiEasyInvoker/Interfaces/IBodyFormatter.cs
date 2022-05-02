using System.Collections.Generic;
using System.Net.Http;
using WebApiEasyInvoker.Models;

namespace WebApiEasyInvoker.Interfaces
{
    public interface IBodyFormatter
    {
        HttpContent Serialize(List<MethodArgumentInfo> argInfos);
    }
}