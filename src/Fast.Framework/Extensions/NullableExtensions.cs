using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Fast.Framework
{

    /// <summary>
    /// 可空扩展类
    /// </summary>
    public static class NullableExtensions
    {

        /// <summary>
        /// 到Nullable
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo> toNullableCache;

        /// <summary>
        /// 构造方法
        /// </summary>
        static NullableExtensions()
        {
            toNullableCache = new Dictionary<Type, MethodInfo>()
            {
                { typeof(short),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(short) })},
                { typeof(ushort),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(ushort) })},
                { typeof(int),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(int) })},
                { typeof(uint),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(uint) })},
                { typeof(long),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(long) })},
                { typeof(ulong),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(ulong) })},
                { typeof(float),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(float) })},
                { typeof(double),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(double) })},
                { typeof(decimal),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(decimal) })},
                { typeof(char),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(char) })},
                { typeof(byte),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(byte) })},
                { typeof(sbyte),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(sbyte) })},
                { typeof(bool),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(bool) })},
                { typeof(string),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(string) })},
                { typeof(DateTime),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(DateTime) })},
                { typeof(DateTimeOffset),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(DateTimeOffset) })},
                { typeof(TimeSpan),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(TimeSpan) })},
                { typeof(Guid),typeof(NullableExtensions).GetMethod(nameof(ToNullable), new Type[] { typeof(Guid) })},
            };
        }

        /// <summary>
        /// 获取ToNullable方法信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static MethodInfo GetToNullableMethodInfo(this Type type)
        {
            if (!toNullableCache.ContainsKey(type))
            {
                throw new NotSupportedException($"类型:{type.Name}暂不支持转换");
            }
            return toNullableCache[type];
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static short? ToNullable(short value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static ushort? ToNullable(ushort value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static int? ToNullable(int value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static uint? ToNullable(uint value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static long? ToNullable(long value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static ulong? ToNullable(ulong value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static float? ToNullable(float value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static double? ToNullable(double value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static decimal? ToNullable(decimal value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static char? ToNullable(char value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte? ToNullable(byte value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static sbyte? ToNullable(sbyte value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool? ToNullable(bool value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static DateTime? ToNullable(DateTime value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static DateTimeOffset? ToNullable(DateTimeOffset value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static TimeSpan? ToNullable(TimeSpan value)
        {
            return value;
        }

        /// <summary>
        /// 到Nullable
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Guid? ToNullable(Guid value)
        {
            return value;
        }
    }


}

