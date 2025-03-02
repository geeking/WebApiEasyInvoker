using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using WebApiEasyInvoker.Attributes;
using WebApiEasyInvoker.Interfaces;
using WebApiEasyInvoker.Models;

namespace WebApiEasyInvoker.Utils
{
    public static class HttpRequestUtil
    {
        private static readonly ConcurrentDictionary<MethodInfo, UrlTemplate> _urlTemplates = new ConcurrentDictionary<MethodInfo, UrlTemplate>();
        private static readonly ConcurrentDictionary<MethodInfo, IRequestBodyFormatter> _requestBodyFormatterPool = new ConcurrentDictionary<MethodInfo, IRequestBodyFormatter>();

        public static UrlTemplate BuildUrlTemplate(MethodInfo methodInfo, IConfiguration configuration)
        {
            if (_urlTemplates.TryGetValue(methodInfo, out var urlTemplate))
            {
                return urlTemplate;
            }

            string host = null, url = null;
            HttpMethod httpMethod = HttpMethod.Get;

            var httpFullUrlAttribute = methodInfo.GetCustomAttribute<HttpFullUrlAttribute>();
            if (httpFullUrlAttribute != null)
            {
                url = httpFullUrlAttribute.Url;
                if (httpFullUrlAttribute.FromConfiguration)
                {
                    url = configuration.GetValue<string>(url);
                }

                httpMethod = httpFullUrlAttribute.HttpMethod;
            }
            else
            {
                var targetType = methodInfo.DeclaringType;
                var httpHostAttribute = targetType.GetCustomAttribute<HttpHostAttribute>();
                if (httpHostAttribute != null)
                {
                    var httpUrlAttribute = methodInfo.GetCustomAttribute<HttpUrlAttribute>();
                    if (httpUrlAttribute == null)
                    {
                        throw new Exception($"Can't build request url for {methodInfo.Name}");
                    }

                    host = httpHostAttribute.HttpHost;
                    if (httpHostAttribute.FromConfiguration)
                    {
                        host = configuration.GetValue<string>(host);
                    }

                    url = httpUrlAttribute.Url;
                    if (httpUrlAttribute.FromConfiguration)
                    {
                        url = configuration.GetValue<string>(url);
                    }

                    httpMethod = httpUrlAttribute.HttpMethod;
                }
                else
                {
                    throw new Exception($"Can't build request url for {targetType.Name}");
                }
            }

            var template = new UrlTemplate
            {
                Host = host,
                Url = url,
                HttpMethod = httpMethod
            };

            _urlTemplates.TryAdd(methodInfo, template);
            return template;
        }

        public static IRequestBodyFormatter GetCustomBodyFormatter(MethodInfo methodInfo, IServiceProvider serviceProvider)
        {
            if (_requestBodyFormatterPool.TryGetValue(methodInfo, out var requestBodyFormatter))
            {
                return requestBodyFormatter;
            }

            var formatterAttribute = methodInfo.GetCustomAttribute<RequestBodyFormatterAttribute>() 
                                     ?? methodInfo.DeclaringType?.GetCustomAttribute<RequestBodyFormatterAttribute>();

            if (formatterAttribute == null)
            {
                _requestBodyFormatterPool.TryAdd(methodInfo, null);
                return null;
            }
            else
            {
                var formatterType = formatterAttribute.FormatterType;
                var formatter = serviceProvider.GetRequiredService(formatterType) as IRequestBodyFormatter;
                _requestBodyFormatterPool.TryAdd(methodInfo, formatter);
                return formatter;
            }
        }
        
        public static HttpRequestMessage BuildHttpRequestMessage(HttpRequestHeaders requestHeaders,
            List<MethodArgumentInfo> argInfos,
            UrlTemplate urlTemplate,
            IRequestBodyFormatter requestBodyFormatter)
        {
            var httpRequestMessage = new HttpRequestMessage();
            if (requestHeaders != null)
            {
                foreach (var header in requestHeaders)
                {
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            var fullUrl = GetHttpRequestUrl(urlTemplate, argInfos);
            httpRequestMessage.RequestUri = new Uri(fullUrl);
            httpRequestMessage.Method = urlTemplate.HttpMethod;
            if (requestBodyFormatter == null)
            {
                httpRequestMessage.Content = GetHttpRequestContent(argInfos);
            }
            else
            {
                httpRequestMessage.Content = requestBodyFormatter.Serialize(argInfos);
            }

            return httpRequestMessage;
        }

        
        private static string CombinUrlAndReplacePlaceholder(List<MethodArgumentInfo> argumentInfos, string host, string url)
        {
            if (string.IsNullOrEmpty(host) && string.IsNullOrEmpty(url))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(host))
            {
                if (string.IsNullOrEmpty(url))
                {
                    url = $"{host.TrimEnd('/')}";
                }
                else
                {
                    url = $"{host.TrimEnd('/')}/{url.TrimStart('/')}";
                }
            }

            foreach (var item in argumentInfos)
            {
                //has ToBody or ToQuery Attribute,then jump
                if (item.ToQueryAttribute != null || item.ToBodyAttribute != null)
                {
                    continue;
                }

                if (item.Type.IsValueOrStringType())
                {
                    url = url.Replace($"{{{item.Name}}}", $"{item.Value}");
                    item.Used = true;
                }
            }

            return url;
        }

        private static string GetKeyValueString(string argName, Type argType, object instance, string kvStr)
        {
            if (instance == null)
            {
                return kvStr;
            }

            if (argType.IsValueOrStringType())
            {
                if (!string.IsNullOrEmpty(instance.ToString()))
                {
                    if (string.IsNullOrEmpty(kvStr))
                    {
                        kvStr += $"{argName}={instance}";
                    }
                    else
                    {
                        kvStr += $"&{argName}={instance}";
                    }
                }
            }
            else
            {
                foreach (var property in argType.GetProperties())
                {
                    kvStr = GetKeyValueString(property.Name, property.PropertyType, property.GetValue(instance), kvStr);
                }
            }

            return kvStr;
        }

        private static string GetQueryString(List<MethodArgumentInfo> argInfos)
        {
            var queryString = string.Empty;
            foreach (var item in argInfos.Where(x => x.ToQueryAttribute != null))
            {
                queryString += GetKeyValueString(item.Name, item.Type, item.Value, queryString);
                item.Used = true;
            }

            return queryString;
        }

        private static string GetHttpRequestUrl(UrlTemplate urlTemplate, List<MethodArgumentInfo> argInfos)
        {
            var fullUrl = CombinUrlAndReplacePlaceholder(argInfos, urlTemplate.Host, urlTemplate.Url);
            if (string.IsNullOrEmpty(fullUrl))
            {
                throw new Exception("Request url is null or empty");
            }

            var queryString = GetQueryString(argInfos);
            if (!string.IsNullOrEmpty(queryString))
            {
                if (fullUrl.Contains("?"))
                {
                    fullUrl = fullUrl.TrimEnd(new char[] {'&'}) + "&" + queryString;
                }
                else
                {
                    fullUrl += "?" + queryString;
                }
            }

            return fullUrl;
        }

        private static HttpContent GetHttpRequestContent(List<MethodArgumentInfo> argInfos)
        {
            if (!argInfos.Any())
            {
                return null;
            }

            var strContent = string.Empty;
            var httpContentType = "application/json";
            var argInfo = argInfos.FirstOrDefault(ai => !ai.Used && ai.ToBodyAttribute != null);
            if (argInfo == null)
            {
                //if no ToBodyAttribute,then select the first class type
                //and use json formatter
                argInfo = argInfos.FirstOrDefault(ai => !ai.Used && !ai.Type.IsValueOrStringType());
                if (argInfo == null)
                {
                    return null;
                }

                strContent = ConvertUtil.SerializeObjectJson(argInfo.Value);
            }
            else
            {
                var tobodyAtr = argInfo.ToBodyAttribute;
                if (tobodyAtr.FormatType == FormatType.Form)
                {
                    strContent = GetKeyValueString(argInfo.Name, argInfo.Type, argInfo.Value, strContent);
                    httpContentType = "application/x-www-form-urlencoded";
                }
                else
                {
                    strContent = ConvertUtil.SerializeObjectJson(argInfo.Value);
                }
            }

            var contentBytes = Encoding.UTF8.GetBytes(strContent);
            HttpContent content = new StreamContent(new MemoryStream(contentBytes));
            content.Headers.Add("Content-Type", httpContentType);
            return content;
        }


        
    }
}