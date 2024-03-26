using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Fast.Framework
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
        /// 运算
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="op">运算符</param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool Operation(object obj1, string op, object obj2)
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
        public static T Sum<T>(T t)
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
        public static T Abs<T>(T t)
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
        /// 日期差异
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        public static int DateDiff(DateTime startDate, DateTime endDate)
        {
            return default;
        }

        /// <summary>
        /// 日期差异
        /// </summary>
        /// <param name="dateType">日期类型</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        public static int DateDiff(DateType dateType, DateTime startDate, DateTime endDate)
        {
            return default;
        }

        /// <summary>
        /// 日期差异
        /// </summary>
        /// <param name="dateType">日期类型</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        public static int TimestampDiff(DateType dateType, DateTime startDate, DateTime endDate)
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

        #region 查询
        /// <summary>
        /// In
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="subQuery"></param>
        /// <returns></returns>
        public static bool In<T>(T t, IQuery subQuery)
        {
            return default;
        }

        /// <summary>
        /// NotIn
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="subQuery"></param>
        /// <returns></returns>
        public static bool NotIn<T>(T t, IQuery subQuery)
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
        /// 行数
        /// </summary>
        /// <param name="orderBy">排序字段</param>
        /// <returns></returns>
        public static int RowNumber<T>(T orderBy)
        {
            return default;
        }

        /// <summary>
        /// 行数
        /// </summary>
        /// <param name="orderBy">排序字段</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns></returns>
        public static int RowNumber<T>(T orderBy, OrderByType orderByType)
        {
            return default;
        }

        /// <summary>
        /// 行数
        /// </summary>
        /// <param name="partitionBy">分区</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns></returns>
        public static int RowNumber<T, T1>(T partitionBy, T1 orderBy)
        {
            return default;
        }

        /// <summary>
        /// 行数
        /// </summary>
        /// <param name="partitionBy">分区</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns></returns>
        public static int RowNumber<T, T1>(T partitionBy, T1 orderBy, OrderByType orderByType)
        {
            return default;
        }

        #region Case When
        /// <summary>
        /// Case
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static CaseWrapper<T> Case<T>(T t)
        {
            return default;
        }

        /// <summary>
        /// CaseWhen
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static CaseWrapper<T> CaseWhen<T>(bool value)
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
        public static CaseWrapper<T> When<T>(this CaseWrapper<T> source, T value)
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
        public static CaseWrapper<T> When<T>(this CaseWrapper<T> source, bool value)
        {
            return default;
        }

        /// <summary>
        /// Then
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source">源</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static CaseWrapper<T> Then<T, TValue>(this CaseWrapper<T> source, TValue value)
        {
            return default;
        }

        /// <summary>
        /// Else
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source">源</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static CaseWrapper<T> Else<T, TValue>(this CaseWrapper<T> source, TValue value)
        {
            return default;
        }

        /// <summary>
        /// End
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源</param>
        /// <returns></returns>
        public static T End<T>(this CaseWrapper<T> source)
        {
            return default;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Case包装类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CaseWrapper<T>
    {
    }
}

