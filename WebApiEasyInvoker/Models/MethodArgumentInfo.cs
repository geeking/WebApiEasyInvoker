using System;
using System.Collections.Generic;
using WebApiEasyInvoker.Attributes;

namespace WebApiEasyInvoker.Models
{
    public class MethodArgumentInfo
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool Used { get; set; }
        public Type Type { get; set; }

        public ToBodyAttribute ToBodyAttribute { get; set; }
        public ToQueryAttribute ToQueryAttribute { get; set; }
    }
}