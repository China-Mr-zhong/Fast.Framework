using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;


namespace Fast.Framework
{

    /// <summary>
    /// Object扩展类
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 改变类型
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static object ChangeType(this object value, Type conversionType)
        {
            if (value == null)
            {
                return default;
            }
            var type = value.GetType();
            if (type.Equals(typeof(Guid)) && conversionType.Equals(typeof(string)))
            {
                value = value.ToString();
            }
            if (conversionType.Equals(type))
            {
                return value;
            }
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                var nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 改变类型
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static T ChangeType<T>(this object value)
        {
            var conversionType = typeof(T);
            if (value == null)
            {
                return default;
            }
            var type = value.GetType();
            if (type.Equals(typeof(Guid)) && conversionType.Equals(typeof(string)))
            {
                value = value.ToString();
            }
            if (conversionType.Equals(type))
            {
                return (T)value;
            }
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                var nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            return (T)Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 异步等待
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static object AsyncWait(this object obj)
        {
            if (obj != null)
            {
                var type = obj.GetType();
                if (type.FullName.StartsWith("System.Threading.Tasks.Task") || type.FullName.StartsWith("System.Runtime.CompilerServices.AsyncTaskMethodBuilder"))
                {
                    var property = type.GetProperty("Result");
                    if (property != null)
                    {
                        return property.GetValue(obj);
                    }
                }
            }
            return obj;
        }
    }
}
