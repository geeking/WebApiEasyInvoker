using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApiEasyInvoker.Interfaces;
using WebApiEasyInvoker.Interfaces.Impl;
using WebApiEasyInvoker.Models;

namespace WebApiEasyInvoker
{
    public static class EasyInvokerServiceCollectionExtensions
    {
        public static void AddWebApiEasyInvoker(this IServiceCollection services, string httpClientName = "")
        {
            //try add httpClient if not
            services.AddHttpClient(httpClientName);
            services.TryAddScoped<IUrlBuilder, UrlBuilderDefault>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assemble in assemblies)
            {
                assemble.GetTypes().ToList().ForEach(t =>
                {
                    if (t.IsInterface && t.GetInterface("IWebApiInvoker`1", true) != null)
                    {
                        services.AddScoped(t, p => WebApiExecutionGenerator.Create(t, new object[] { p, httpClientName }));
                    }
                });
            }
        }
    }
}