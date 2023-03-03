using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Fast.Framework.Extensions
{

    /// <summary>
    /// Sql函数
    /// </summary>
    public static class SqlFunc
    {

        #region 字符串函数
        /// <summary>
        /// 长度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int Len<T>(T t)
        {
            return default;
        }

        /// <summary>
        /// 长度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int Length<T>(T t)
        {
            return default;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="strings">字符串</param>
        /// <returns></returns>
        public static string Concat(params string[] strings)
        {
            return default;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="strings">字符串</param>
        /// <returns></returns>
        public static string Concat(List<string> strings)
        {
            return default;
        }

        /// <summary>
        /// 运算
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str1"></param>
        /// <param name="op">运算符</param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool Operation(string str1, string op, string str2)
        {
            return default;
        }

        #endregion

        #region 聚合函数

        /// <summary>
        /// 最大值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T Max<T>(T t)
        {
            return default;
        }

        /// <summary>
        /// 最小值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T Min<T>(T t)
        {
            return default;
        }

        /// <summary>
        /// 计数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int Count<T>(T t)
        {
            return default;
        }

        /// <summary>
        /// 合计
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static decimal Sum<T>(T t)
        {
            return default;
        }

        /// <summary>
        /// 平均
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Avg<T>(T t)
        {
            return default;
        }
        #endregion

        #region 数学函数
        /// <summary>
        /// 绝对值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static decimal Abs<T>(T t)
        {
            return default;
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="decimals">小数点</param>
        /// <returns></returns>
        public static decimal Round<T>(T t, int decimals)
        {
            return default;
        }
        #endregion

        #region 日期函数

        /// <summary>
        /// SqlServer 独有日期格式化函数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dateTime"></param>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public static T Convert<T>(string type, T dateTime, int formatType)
        {
            return default;
        }

        /// <summary>
        /// 日期差异
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="datepart">日期部分</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        public static int DateDiff(string datepart, DateTime startDate, DateTime endDate)
        {
            return default;
        }

        /// <summary>
        /// 日期差异
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="datepart">日期部分</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        public static int TimestampDiff(string datepart, DateTime startDate, DateTime endDate)
        {
            return default;
        }

        /// <summary>
        /// 年
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int Year<T>(T t)
        {
            return default;
        }

        /// <summary>
        /// 月
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int Month<T>(T t)
        {
            return default;
        }

        /// <summary>
        /// 日
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int Day<T>(T t)
        {
            return default;
        }
        #endregion

        #region 查询函数

        /// <summary>
        /// In查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public static bool In<T>(T t, params T[] list)
        {
            return default;
        }

        /// <summary>
        /// In查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public static bool In<T>(T t, List<T> list)
        {
            return default;
        }

        /// <summary>
        /// Not In查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public static bool NotIn<T>(T t, params T[] list)
        {
            return default;
        }

        /// <summary>
        /// Not In查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public static bool NotIn<T>(T t, List<T> list)
        {
            return default;
        }
        #endregion

        #region 其它函数

        /// <summary>
        /// 是否空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static T IsNull<T>(T t, T value)
        {
            return default;
        }

        /// <summary>
        /// 如果空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static T IfNull<T>(T t, T value)
        {
            return default;
        }

        /// <summary>
        /// 空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static T Nvl<T>(T t, T value)
        {
            return default;
        }

        /// <summary>
        /// Case
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Case<T> Case<T>(T t)
        {
            return default;
        }

        /// <summary>
        /// CaseWhen
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Case<T> CaseWhen<T>(bool value)
        {
            return default;
        }

        /// <summary>
        /// When
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Case<T> When<T>(this Case<T> source, T value)
        {
            return default;
        }

        /// <summary>
        /// When
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Case<T> When<T>(this Case<T> source, bool value)
        {
            return default;
        }

        /// <summary>
        /// Then
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Case<T> Then<T>(this Case<T> source, T value)
        {
            return default;
        }

        /// <summary>
        /// Else
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static Case<T> Else<T>(this Case<T> source, T value)
        {
            return default;
        }

        /// <summary>
        /// End
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源</param>
        /// <returns></returns>
        public static T End<T>(this Case<T> source)
        {
            return default;
        }

        #endregion
    }

    /// <summary>
    /// Case包装类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Case<T>
    {
    }
}

