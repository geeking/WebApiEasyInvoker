using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WebApiEasyInvoker.Models
{
    public class MethodArgumentInfo
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool Used { get; set; }
        public Type Type { get; set; }
        public IEnumerable<Attribute> Attributes { get; set; }
    }
}
