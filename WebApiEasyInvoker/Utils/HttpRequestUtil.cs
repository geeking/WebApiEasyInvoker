using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using WebApiEasyInvoker.Attributes;
using WebApiEasyInvoker.Models;

namespace WebApiEasyInvoker.Utils
{
    public static class HttpRequestUtil
    {
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
                if (item.Attributes.Any())
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
            foreach (var item in argInfos)
            {
                if (item.Attributes.Where(a => a.GetType() == typeof(ToQueryAttribute)).Any())
                {
                    queryString += GetKeyValueString(item.Name, item.Type, item.Value, queryString);
                    item.Used = true;
                }
            }
            return queryString;
        }

        private static (HttpMethod HttpMethod, string FullUrl) GetHttpRequestMethodAndUrl(UrlTemplate urlTemplate, List<MethodArgumentInfo> argInfos)
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
                    fullUrl = fullUrl.TrimEnd(new char[] { '&' }) + "&" + queryString;
                }
                else
                {
                    fullUrl += "?" + queryString;
                }
            }
            return (urlTemplate.HttpMethod, fullUrl);
        }

        private static HttpContent GetHttpRequestContent(List<MethodArgumentInfo> argInfos)
        {
            if (!argInfos.Any())
            {
                return null;
            }
            var strContent = string.Empty;
            var httpContentType = "application/json";
            var argInfo = argInfos.FirstOrDefault(ai => !ai.Used && ai.Attributes.Any(a => a.GetType() == typeof(ToBodyAttribute)));
            if (argInfo == null)
            {
                //if no ToBodyAttribute,then select the first class type
                //and use json formatter
                argInfo = argInfos.FirstOrDefault(ai => !ai.Used && !ai.Type.IsValueOrStringType());
                strContent = ConvertUtil.SerializeObjectJson(argInfo.Value);
            }
            else
            {
                var tobodyAtr = argInfo.Attributes
                    .Where(a => a.GetType() == typeof(ToBodyAttribute))
                    .Select(a => a as ToBodyAttribute)
                    .First();
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

        public static UrlTemplate BuildUrlTemplate(MethodInfo methodInfo)
        {
            string host = null, url = null;
            HttpMethod httpMethod = HttpMethod.Get;

            var httpFullUrlAttribute = methodInfo.GetCustomAttribute<HttpFullUrlAttribute>();
            if (httpFullUrlAttribute != null)
            {
                url = httpFullUrlAttribute.Url;
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
                    url = httpUrlAttribute.Url;
                    httpMethod = httpUrlAttribute.HttpMethod;
                }
                else
                {
                    throw new Exception($"Can't build request url for {targetType.Name}");
                }
            }
            return new UrlTemplate
            {
                Host = host,
                Url = url,
                HttpMethod = httpMethod
            };
        }

        public static HttpRequestMessage BuildHttpRequestMessage(HttpRequestHeaders requestHeaders, List<MethodArgumentInfo> argInfos, UrlTemplate urlTemplate)
        {
            var httpRequestMessage = new HttpRequestMessage();
            if (requestHeaders != null)
            {
                foreach (var header in requestHeaders)
                {
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }
            var (httpMethod, fullUrl) = GetHttpRequestMethodAndUrl(urlTemplate, argInfos);
            httpRequestMessage.RequestUri = new Uri(fullUrl);
            httpRequestMessage.Method = httpMethod;
            httpRequestMessage.Content = GetHttpRequestContent(argInfos);

            return httpRequestMessage;
        }
    }
}