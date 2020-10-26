﻿using System;
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
    internal static class HttpRequestUtil
    {
        private static string CombinUrlAndReplacePlaceholder(List<MethodArgumentInfo> argumentInfos, string host, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }
            if (!string.IsNullOrWhiteSpace(host))
            {
                url = $" {host.TrimEnd('/')}/{url.TrimStart('/')}";
            }
            foreach (var item in argumentInfos)
            {
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

        private static (HttpMethod HttpMethod, string FullUrl) GetHttpRequestMethodAndUrl(MethodInfo methodInfo, List<MethodArgumentInfo> argInfos)
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
                        throw new NullReferenceException($"Can't find {nameof(HttpUrlAttribute)} for {methodInfo.Name}");
                    }
                    host = httpHostAttribute.HttpHost;
                    url = httpUrlAttribute.Url;
                    httpMethod = httpUrlAttribute.HttpMethod;
                }
                else
                {
                    throw new NullReferenceException($"Can't find {nameof(HttpHostAttribute)} for {targetType.Name}");

                }
            }
            var fullUrl = CombinUrlAndReplacePlaceholder(argInfos, host, url);
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
            return (httpMethod, fullUrl);
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

        public static HttpRequestMessage BuildHttpRequestMessage(HttpRequestHeaders requestHeaders, MethodInfo methodInfo, List<MethodArgumentInfo> argInfos)
        {
            var httpRequestMessage = new HttpRequestMessage();
            if (requestHeaders != null)
            {
                foreach (var header in requestHeaders)
                {
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            var tmp = GetHttpRequestMethodAndUrl(methodInfo, argInfos);
            httpRequestMessage.RequestUri = new Uri(tmp.FullUrl);
            httpRequestMessage.Method = tmp.HttpMethod;
            httpRequestMessage.Content = GetHttpRequestContent(argInfos);

            return httpRequestMessage;
        }
    }
}