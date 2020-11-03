using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApiEasyInvoker.Models;

namespace WebApiEasyInvoker
{
    public static class EasyInvokerServiceCollectionExtensions
    {
        public static void AddWebApiEasyInvoker(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembie in assemblies)
            {
                assembie.GetTypes().ToList().ForEach(t =>
                {
                    if (t.IsInterface && t.GetInterface("IWebApiInvoker`1", true) != null)
                    {
                        var baseType = typeof(WebApiExecutor<>).MakeGenericType(t);
                        services.AddScoped(t, p => WebApiExecutionGenerator.Create(baseType, t));
                    }
                });
            }
        }
    }
}
