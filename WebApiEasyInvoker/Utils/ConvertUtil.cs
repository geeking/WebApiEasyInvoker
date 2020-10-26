using Newtonsoft.Json;
using System;

namespace WebApiEasyInvoker.Utils
{
    internal static class ConvertUtil
    {
        /// <summary>
        /// get object from input. Notice:if T is a class type,then input should be a json string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string input)
        {
            return (T)DeserializeObject(typeof(T), input);
        }

        /// <summary>
        /// get object from input. Notice:if destType is Object,then input should be a json string
        /// </summary>
        /// <param name="destType"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static object DeserializeObject(Type destType, string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var typeCode = Type.GetTypeCode(destType);
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return bool.Parse(input);

                case TypeCode.Byte:
                    return byte.Parse(input);

                case TypeCode.Char:
                    return char.Parse(input);

                case TypeCode.DateTime:
                    return DateTime.Parse(input);

                case TypeCode.DBNull:
                    break;

                case TypeCode.Decimal:
                    return decimal.Parse(input);

                case TypeCode.Double:
                    return double.Parse(input);

                case TypeCode.Empty:
                    return null;

                case TypeCode.Int16:
                    return short.Parse(input);

                case TypeCode.Int32:
                    return int.Parse(input);

                case TypeCode.Int64:
                    return long.Parse(input);

                case TypeCode.Object:
                    return JsonConvert.DeserializeObject(input, destType);

                case TypeCode.SByte:
                    return sbyte.Parse(input);

                case TypeCode.Single:
                    return float.Parse(input);

                case TypeCode.String:
                    return input;

                case TypeCode.UInt16:
                    return ushort.Parse(input);

                case TypeCode.UInt32:
                    return uint.Parse(input);

                case TypeCode.UInt64:
                    return ulong.Parse(input);

                default:
                    break;
            }
            return input;
        }

        /// <summary>
        /// serialize the object to json string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeObjectJson(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}