using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 动态条件表达式
    /// </summary>
    public static class DynamicWhereExp
    {
        #region 创建

        /// <summary>
        /// 合并表达式
        /// </summary>
        /// <param name="left">左</param>
        /// <param name="right">右</param>
        /// <param name="expressionType">表达式类型</param>
        /// <param name="Parameters">表达式参数</param>
        /// <returns></returns>
        public static Expression MergeExpression(this Expression left, Expression right, ExpressionType expressionType, List<ParameterExpression> Parameters)
        {
            if (left == null)
            {
                return right;
            }
            var expParameterReplace = new ExpParameterReplace(Parameters);
            if (expressionType == ExpressionType.Or || expressionType == ExpressionType.OrElse)
            {
                return Expression.OrElse(left, expParameterReplace.Visit(right));
            }
            else if (expressionType == ExpressionType.And || expressionType == ExpressionType.AndAlso)
            {
                return Expression.AndAlso(left, expParameterReplace.Visit(right));
            }
            throw new NotSupportedException($"{expressionType} 类型不支持合并.");
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DynamicWhereExp<T> Create<T>() where T : class, new()
        {
            return new DynamicWhereExp<T>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns></returns>
        public static DynamicWhereExp<T, T2> Create<T, T2>() where T : class, new() where T2 : class, new()
        {
            return new DynamicWhereExp<T, T2>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <returns></returns>
        public static DynamicWhereExp<T, T2, T3> Create<T, T2, T3>() where T : class, new() where T2 : class, new() where T3 : class, new()
        {
            return new DynamicWhereExp<T, T2, T3>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <returns></returns>
        public static DynamicWhereExp<T, T2, T3, T4> Create<T, T2, T3, T4>() where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new()
        {
            return new DynamicWhereExp<T, T2, T3, T4>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <returns></returns>
        public static DynamicWhereExp<T, T2, T3, T4, T5> Create<T, T2, T3, T4, T5>() where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new() where T5 : class, new()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <returns></returns>
        public static DynamicWhereExp<T, T2, T3, T4, T5, T6> Create<T, T2, T3, T4, T5, T6>() where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new() where T5 : class, new() where T6 : class, new()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5, T6>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <returns></returns>
        public static DynamicWhereExp<T, T2, T3, T4, T5, T6, T7> Create<T, T2, T3, T4, T5, T6, T7>() where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new() where T5 : class, new() where T6 : class, new() where T7 : class, new()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5, T6, T7>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <returns></returns>
        public static DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8> Create<T, T2, T3, T4, T5, T6, T7, T8>() where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new() where T5 : class, new() where T6 : class, new() where T7 : class, new() where T8 : class, new()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <returns></returns>
        public static DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9> Create<T, T2, T3, T4, T5, T6, T7, T8, T9>() where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new() where T5 : class, new() where T6 : class, new() where T7 : class, new() where T8 : class, new() where T9 : class, new()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <returns></returns>
        public static DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Create<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>() where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new() where T5 : class, new() where T6 : class, new() where T7 : class, new() where T8 : class, new() where T9 : class, new() where T10 : class, new()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <returns></returns>
        public static DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Create<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>() where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new() where T5 : class, new() where T6 : class, new() where T7 : class, new() where T8 : class, new() where T9 : class, new() where T10 : class, new() where T11 : class, new()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <returns></returns>
        public static DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Create<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>() where T : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new() where T5 : class, new() where T6 : class, new() where T7 : class, new() where T8 : class, new() where T9 : class, new() where T10 : class, new() where T11 : class, new() where T12 : class, new()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>();
        }
        #endregion
    }

    /// <summary>
    /// 表达式参数替换
    /// </summary>
    internal class ExpParameterReplace : ExpressionVisitor
    {

        public List<ParameterExpression> Parameters { get; }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var exp = Parameters.FirstOrDefault(f => f.Name == node.Name);
            return base.VisitParameter(exp ?? node);
        }

        internal ExpParameterReplace(List<ParameterExpression> Parameters)
        {
            this.Parameters = Parameters;
        }
    }

    #region T1
    /// <summary>
    /// 动态表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicWhereExp<T> where T : class, new()
    {

        /// <summary>
        /// 体表达式
        /// </summary>
        public Expression BodyExp { get; private set; }

        /// <summary>
        /// 参数表达式
        /// </summary>
        public List<ParameterExpression> Parameters { get; private set; }

        public DynamicWhereExp()
        {
        }

        public DynamicWhereExp(Expression expression, List<ParameterExpression> parameters)
        {
            this.BodyExp = expression;
            this.Parameters = parameters;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitParameter(LambdaExpression lambdaExp)
        {
            Parameters ??= lambdaExp.Parameters.ToList();
        }

        /// <summary>
        /// 和
        /// </summary>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T> And(Expression<Func<T, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            return this;
        }

        /// <summary>
        /// 和如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T> AndIF(bool where, Expression<Func<T, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 或
        /// </summary>
        /// <param name="lambdaExp"></param>
        /// <returns></returns>
        public DynamicWhereExp<T> Or(Expression<Func<T, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            return this;
        }

        /// <summary>
        /// 或如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T> OrIF(bool where, Expression<Func<T, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public DynamicWhereExp<T> Clone()
        {
            return new DynamicWhereExp<T>(BodyExp, Parameters);
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, bool>> Build()
        {
            if (BodyExp == null)
            {
                BodyExp = Expression.Constant(true);
                Parameters = new List<ParameterExpression>()
                {
                    Expression.Parameter(typeof(T),"a")
                };
            }
            var lambdaExp = (Expression<Func<T, bool>>)Expression.Lambda(BodyExp, Parameters);
            return lambdaExp;
        }
    }
    #endregion

    #region T2
    /// <summary>
    /// 动态表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class DynamicWhereExp<T, T2> where T : class, new()
    {
        /// <summary>
        /// 体表达式
        /// </summary>
        public Expression BodyExp { get; private set; }

        /// <summary>
        /// 参数表达式
        /// </summary>
        public List<ParameterExpression> Parameters { get; private set; }

        public DynamicWhereExp()
        {
        }

        public DynamicWhereExp(Expression expression, List<ParameterExpression> parameters)
        {
            this.BodyExp = expression;
            this.Parameters = parameters;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitParameter(LambdaExpression lambdaExp)
        {
            Parameters ??= lambdaExp.Parameters.ToList();
        }

        /// <summary>
        /// 和
        /// </summary>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2> And(Expression<Func<T, T2, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            return this;
        }

        /// <summary>
        /// 和如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2> AndIF(bool where, Expression<Func<T, T2, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 或
        /// </summary>
        /// <param name="lambdaExp"></param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2> Or(Expression<Func<T, T2, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            return this;
        }

        /// <summary>
        /// 或如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2> OrIF(bool where, Expression<Func<T, T2, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public DynamicWhereExp<T, T2> Clone()
        {
            return new DynamicWhereExp<T, T2>(BodyExp, Parameters);
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, T2, bool>> Build()
        {
            if (BodyExp == null)
            {
                Parameters = new List<ParameterExpression>()
                {
                    Expression.Parameter(typeof(T),"a"),
                    Expression.Parameter(typeof(T2),"b")
                };
                BodyExp = Expression.Constant(true);
            }
            var lambdaExp = (Expression<Func<T, T2, bool>>)Expression.Lambda(BodyExp, Parameters);
            return lambdaExp;
        }
    }
    #endregion

    #region T3
    /// <summary>
    /// 动态表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class DynamicWhereExp<T, T2, T3> where T : class, new()
    {
        /// <summary>
        /// 体表达式
        /// </summary>
        public Expression BodyExp { get; private set; }

        /// <summary>
        /// 参数表达式
        /// </summary>
        public List<ParameterExpression> Parameters { get; private set; }

        public DynamicWhereExp()
        {
        }

        public DynamicWhereExp(Expression expression, List<ParameterExpression> parameters)
        {
            this.BodyExp = expression;
            this.Parameters = parameters;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitParameter(LambdaExpression lambdaExp)
        {
            Parameters ??= lambdaExp.Parameters.ToList();
        }

        /// <summary>
        /// 和
        /// </summary>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3> And(Expression<Func<T, T2, T3, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            return this;
        }

        /// <summary>
        /// 和如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3> AndIF(bool where, Expression<Func<T, T2, T3, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 或
        /// </summary>
        /// <param name="lambdaExp"></param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3> Or(Expression<Func<T, T2, T3, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            return this;
        }

        /// <summary>
        /// 或如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3> OrIF(bool where, Expression<Func<T, T2, T3, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3> Clone()
        {
            return new DynamicWhereExp<T, T2, T3>(BodyExp, Parameters);
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, T2, T3, bool>> Build()
        {
            if (BodyExp == null)
            {
                Parameters = new List<ParameterExpression>()
                {
                    Expression.Parameter(typeof(T),"a"),
                    Expression.Parameter(typeof(T2),"b"),
                    Expression.Parameter(typeof(T3),"c")
                };
                BodyExp = Expression.Constant(true);
            }
            var lambdaExp = (Expression<Func<T, T2, T3, bool>>)Expression.Lambda(BodyExp, Parameters);
            return lambdaExp;
        }
    }
    #endregion

    #region T4
    /// <summary>
    /// 动态表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public class DynamicWhereExp<T, T2, T3, T4> where T : class, new()
    {
        /// <summary>
        /// 体表达式
        /// </summary>
        public Expression BodyExp { get; private set; }

        /// <summary>
        /// 参数表达式
        /// </summary>
        public List<ParameterExpression> Parameters { get; private set; }

        public DynamicWhereExp()
        {
        }

        public DynamicWhereExp(Expression expression, List<ParameterExpression> parameters)
        {
            this.BodyExp = expression;
            this.Parameters = parameters;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitParameter(LambdaExpression lambdaExp)
        {
            Parameters ??= lambdaExp.Parameters.ToList();
        }

        /// <summary>
        /// 和
        /// </summary>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4> And(Expression<Func<T, T2, T3, T4, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            return this;
        }

        /// <summary>
        /// 和如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4> AndIF(bool where, Expression<Func<T, T2, T3, T4, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 或
        /// </summary>
        /// <param name="lambdaExp"></param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4> Or(Expression<Func<T, T2, T3, T4, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            return this;
        }

        /// <summary>
        /// 或如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4> OrIF(bool where, Expression<Func<T, T2, T3, T4, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4> Clone()
        {
            return new DynamicWhereExp<T, T2, T3, T4>(BodyExp, Parameters);
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, T2, T3, T4, bool>> Build()
        {
            if (BodyExp == null)
            {
                Parameters = new List<ParameterExpression>()
                {
                    Expression.Parameter(typeof(T),"a"),
                    Expression.Parameter(typeof(T2),"b"),
                    Expression.Parameter(typeof(T3),"c"),
                    Expression.Parameter(typeof(T4),"d")
                };
                BodyExp = Expression.Constant(true);
            }
            var lambdaExp = (Expression<Func<T, T2, T3, T4, bool>>)Expression.Lambda(BodyExp, Parameters);
            return lambdaExp;
        }
    }
    #endregion

    #region T5
    /// <summary>
    /// 动态表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    public class DynamicWhereExp<T, T2, T3, T4, T5> where T : class, new()
    {
        /// <summary>
        /// 体表达式
        /// </summary>
        public Expression BodyExp { get; private set; }

        /// <summary>
        /// 参数表达式
        /// </summary>
        public List<ParameterExpression> Parameters { get; private set; }

        public DynamicWhereExp()
        {
        }

        public DynamicWhereExp(Expression expression, List<ParameterExpression> parameters)
        {
            this.BodyExp = expression;
            this.Parameters = parameters;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitParameter(LambdaExpression lambdaExp)
        {
            Parameters ??= lambdaExp.Parameters.ToList();
        }

        /// <summary>
        /// 和
        /// </summary>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5> And(Expression<Func<T, T2, T3, T4, T5, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            return this;
        }

        /// <summary>
        /// 和如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5> AndIF(bool where, Expression<Func<T, T2, T3, T4, T5, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 或
        /// </summary>
        /// <param name="lambdaExp"></param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5> Or(Expression<Func<T, T2, T3, T4, T5, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            return this;
        }

        /// <summary>
        /// 或如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5> OrIF(bool where, Expression<Func<T, T2, T3, T4, T5, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5> Clone()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5>(BodyExp, Parameters);
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, T2, T3, T4, T5, bool>> Build()
        {
            if (BodyExp == null)
            {
                Parameters = new List<ParameterExpression>()
                {
                    Expression.Parameter(typeof(T),"a"),
                    Expression.Parameter(typeof(T2),"b"),
                    Expression.Parameter(typeof(T3),"c"),
                    Expression.Parameter(typeof(T4),"d"),
                    Expression.Parameter(typeof(T5),"e")
                };
                BodyExp = Expression.Constant(true);
            }
            var lambdaExp = (Expression<Func<T, T2, T3, T4, T5, bool>>)Expression.Lambda(BodyExp, Parameters);
            return lambdaExp;
        }
    }
    #endregion

    #region T6
    /// <summary>
    /// 动态表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    public class DynamicWhereExp<T, T2, T3, T4, T5, T6> where T : class, new()
    {
        /// <summary>
        /// 体表达式
        /// </summary>
        public Expression BodyExp { get; private set; }

        /// <summary>
        /// 参数表达式
        /// </summary>
        public List<ParameterExpression> Parameters { get; private set; }

        public DynamicWhereExp()
        {
        }

        public DynamicWhereExp(Expression expression, List<ParameterExpression> parameters)
        {
            this.BodyExp = expression;
            this.Parameters = parameters;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitParameter(LambdaExpression lambdaExp)
        {
            Parameters ??= lambdaExp.Parameters.ToList();
        }

        /// <summary>
        /// 和
        /// </summary>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6> And(Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            return this;
        }

        /// <summary>
        /// 和如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6> AndIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 或
        /// </summary>
        /// <param name="lambdaExp"></param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6> Or(Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            return this;
        }

        /// <summary>
        /// 或如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6> OrIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6> Clone()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5, T6>(BodyExp, Parameters);
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, T2, T3, T4, T5, T6, bool>> Build()
        {
            if (BodyExp == null)
            {
                Parameters = new List<ParameterExpression>()
                {
                    Expression.Parameter(typeof(T),"a"),
                    Expression.Parameter(typeof(T2),"b"),
                    Expression.Parameter(typeof(T3),"c"),
                    Expression.Parameter(typeof(T4),"d"),
                    Expression.Parameter(typeof(T5),"e"),
                    Expression.Parameter(typeof(T6),"f")
                };
                BodyExp = Expression.Constant(true);
            }
            var lambdaExp = (Expression<Func<T, T2, T3, T4, T5, T6, bool>>)Expression.Lambda(BodyExp, Parameters);
            return lambdaExp;
        }
    }
    #endregion

    #region T7
    /// <summary>
    /// 动态表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    public class DynamicWhereExp<T, T2, T3, T4, T5, T6, T7> where T : class, new()
    {
        /// <summary>
        /// 体表达式
        /// </summary>
        public Expression BodyExp { get; private set; }

        /// <summary>
        /// 参数表达式
        /// </summary>
        public List<ParameterExpression> Parameters { get; private set; }

        public DynamicWhereExp()
        {
        }

        public DynamicWhereExp(Expression expression, List<ParameterExpression> parameters)
        {
            this.BodyExp = expression;
            this.Parameters = parameters;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitParameter(LambdaExpression lambdaExp)
        {
            Parameters ??= lambdaExp.Parameters.ToList();
        }

        /// <summary>
        /// 和
        /// </summary>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7> And(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            return this;
        }

        /// <summary>
        /// 和如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7> AndIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 或
        /// </summary>
        /// <param name="lambdaExp"></param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7> Or(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            return this;
        }

        /// <summary>
        /// 或如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7> OrIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7> Clone()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5, T6, T7>(BodyExp, Parameters);
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> Build()
        {
            if (BodyExp == null)
            {
                Parameters = new List<ParameterExpression>()
                {
                    Expression.Parameter(typeof(T),"a"),
                    Expression.Parameter(typeof(T2),"b"),
                    Expression.Parameter(typeof(T3),"c"),
                    Expression.Parameter(typeof(T4),"d"),
                    Expression.Parameter(typeof(T5),"e"),
                    Expression.Parameter(typeof(T6),"f"),
                    Expression.Parameter(typeof(T7),"g")
                };
                BodyExp = Expression.Constant(true);
            }
            var lambdaExp = (Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>>)Expression.Lambda(BodyExp, Parameters);
            return lambdaExp;
        }
    }
    #endregion

    #region T8
    /// <summary>
    /// 动态表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    public class DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8> where T : class, new()
    {
        /// <summary>
        /// 体表达式
        /// </summary>
        public Expression BodyExp { get; private set; }

        /// <summary>
        /// 参数表达式
        /// </summary>
        public List<ParameterExpression> Parameters { get; private set; }

        public DynamicWhereExp()
        {
        }

        public DynamicWhereExp(Expression expression, List<ParameterExpression> parameters)
        {
            this.BodyExp = expression;
            this.Parameters = parameters;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitParameter(LambdaExpression lambdaExp)
        {
            Parameters ??= lambdaExp.Parameters.ToList();
        }

        /// <summary>
        /// 和
        /// </summary>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8> And(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            return this;
        }

        /// <summary>
        /// 和如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8> AndIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 或
        /// </summary>
        /// <param name="lambdaExp"></param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8> Or(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            return this;
        }

        /// <summary>
        /// 或如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8> OrIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8> Clone()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8>(BodyExp, Parameters);
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> Build()
        {
            if (BodyExp == null)
            {
                Parameters = new List<ParameterExpression>()
                {
                    Expression.Parameter(typeof(T),"a"),
                    Expression.Parameter(typeof(T2),"b"),
                    Expression.Parameter(typeof(T3),"c"),
                    Expression.Parameter(typeof(T4),"d"),
                    Expression.Parameter(typeof(T5),"e"),
                    Expression.Parameter(typeof(T6),"f"),
                    Expression.Parameter(typeof(T7),"g"),
                    Expression.Parameter(typeof(T8),"h")
                };
                BodyExp = Expression.Constant(true);
            }
            var lambdaExp = (Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>>)Expression.Lambda(BodyExp, Parameters);
            return lambdaExp;
        }
    }
    #endregion

    #region T9
    /// <summary>
    /// 动态表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    public class DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9> where T : class, new()
    {
        /// <summary>
        /// 体表达式
        /// </summary>
        public Expression BodyExp { get; private set; }

        /// <summary>
        /// 参数表达式
        /// </summary>
        public List<ParameterExpression> Parameters { get; private set; }

        public DynamicWhereExp()
        {
        }

        public DynamicWhereExp(Expression expression, List<ParameterExpression> parameters)
        {
            this.BodyExp = expression;
            this.Parameters = parameters;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitParameter(LambdaExpression lambdaExp)
        {
            Parameters ??= lambdaExp.Parameters.ToList();
        }

        /// <summary>
        /// 和
        /// </summary>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9> And(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            return this;
        }

        /// <summary>
        /// 和如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9> AndIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 或
        /// </summary>
        /// <param name="lambdaExp"></param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9> Or(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            return this;
        }

        /// <summary>
        /// 或如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9> OrIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9> Clone()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9>(BodyExp, Parameters);
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> Build()
        {
            if (BodyExp == null)
            {
                Parameters = new List<ParameterExpression>()
                {
                    Expression.Parameter(typeof(T),"a"),
                    Expression.Parameter(typeof(T2),"b"),
                    Expression.Parameter(typeof(T3),"c"),
                    Expression.Parameter(typeof(T4),"d"),
                    Expression.Parameter(typeof(T5),"e"),
                    Expression.Parameter(typeof(T6),"f"),
                    Expression.Parameter(typeof(T7),"g"),
                    Expression.Parameter(typeof(T8),"h"),
                    Expression.Parameter(typeof(T9),"i")
                };
                BodyExp = Expression.Constant(true);
            }
            var lambdaExp = (Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>>)Expression.Lambda(BodyExp, Parameters);
            return lambdaExp;
        }
    }
    #endregion

    #region T10
    /// <summary>
    /// 动态表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    public class DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> where T : class, new()
    {
        /// <summary>
        /// 体表达式
        /// </summary>
        public Expression BodyExp { get; private set; }

        /// <summary>
        /// 参数表达式
        /// </summary>
        public List<ParameterExpression> Parameters { get; private set; }

        public DynamicWhereExp()
        {
        }

        public DynamicWhereExp(Expression expression, List<ParameterExpression> parameters)
        {
            this.BodyExp = expression;
            this.Parameters = parameters;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitParameter(LambdaExpression lambdaExp)
        {
            Parameters ??= lambdaExp.Parameters.ToList();
        }

        /// <summary>
        /// 和
        /// </summary>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> And(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            return this;
        }

        /// <summary>
        /// 和如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AndIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 或
        /// </summary>
        /// <param name="lambdaExp"></param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Or(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            return this;
        }

        /// <summary>
        /// 或如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> Build()
        {
            if (BodyExp == null)
            {
                Parameters = new List<ParameterExpression>()
                {
                    Expression.Parameter(typeof(T),"a"),
                    Expression.Parameter(typeof(T2),"b"),
                    Expression.Parameter(typeof(T3),"c"),
                    Expression.Parameter(typeof(T4),"d"),
                    Expression.Parameter(typeof(T5),"e"),
                    Expression.Parameter(typeof(T6),"f"),
                    Expression.Parameter(typeof(T7),"g"),
                    Expression.Parameter(typeof(T8),"h"),
                    Expression.Parameter(typeof(T9),"i"),
                    Expression.Parameter(typeof(T10),"j")
                };
                BodyExp = Expression.Constant(true);
            }
            var lambdaExp = (Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>>)Expression.Lambda(BodyExp, Parameters);
            return lambdaExp;
        }
    }
    #endregion

    #region T11
    /// <summary>
    /// 动态表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    /// <typeparam name="T11"></typeparam>
    public class DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> where T : class, new()
    {
        /// <summary>
        /// 体表达式
        /// </summary>
        public Expression BodyExp { get; private set; }

        /// <summary>
        /// 参数表达式
        /// </summary>
        public List<ParameterExpression> Parameters { get; private set; }

        public DynamicWhereExp()
        {
        }

        public DynamicWhereExp(Expression expression, List<ParameterExpression> parameters)
        {
            this.BodyExp = expression;
            this.Parameters = parameters;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitParameter(LambdaExpression lambdaExp)
        {
            Parameters ??= lambdaExp.Parameters.ToList();
        }

        /// <summary>
        /// 和
        /// </summary>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> And(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            return this;
        }

        /// <summary>
        /// 和如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AndIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 或
        /// </summary>
        /// <param name="lambdaExp"></param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Or(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            return this;
        }

        /// <summary>
        /// 或如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Clone()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(BodyExp, Parameters);
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> Build()
        {
            if (BodyExp == null)
            {
                Parameters = new List<ParameterExpression>()
                {
                    Expression.Parameter(typeof(T),"a"),
                    Expression.Parameter(typeof(T2),"b"),
                    Expression.Parameter(typeof(T3),"c"),
                    Expression.Parameter(typeof(T4),"d"),
                    Expression.Parameter(typeof(T5),"e"),
                    Expression.Parameter(typeof(T6),"f"),
                    Expression.Parameter(typeof(T7),"g"),
                    Expression.Parameter(typeof(T8),"h"),
                    Expression.Parameter(typeof(T9),"i"),
                    Expression.Parameter(typeof(T10),"j"),
                    Expression.Parameter(typeof(T11),"k")
                };
                BodyExp = Expression.Constant(true);
            }
            var lambdaExp = (Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>>)Expression.Lambda(BodyExp, Parameters);
            return lambdaExp;
        }
    }
    #endregion

    #region T12
    /// <summary>
    /// 动态表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    /// <typeparam name="T9"></typeparam>
    /// <typeparam name="T10"></typeparam>
    /// <typeparam name="T11"></typeparam>
    /// <typeparam name="T12"></typeparam>
    public class DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> where T : class, new()
    {
        /// <summary>
        /// 体表达式
        /// </summary>
        public Expression BodyExp { get; private set; }

        /// <summary>
        /// 参数表达式
        /// </summary>
        public List<ParameterExpression> Parameters { get; private set; }

        public DynamicWhereExp()
        {
        }

        public DynamicWhereExp(Expression expression, List<ParameterExpression> parameters)
        {
            this.BodyExp = expression;
            this.Parameters = parameters;
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitParameter(LambdaExpression lambdaExp)
        {
            Parameters ??= lambdaExp.Parameters.ToList();
        }

        /// <summary>
        /// 和
        /// </summary>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> And(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            return this;
        }

        /// <summary>
        /// 和如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AndIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.AndAlso, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 或
        /// </summary>
        /// <param name="lambdaExp"></param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Or(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> lambdaExp)
        {
            InitParameter(lambdaExp);
            BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            return this;
        }

        /// <summary>
        /// 或如果
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="lambdaExp">表达式</param>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrIF(bool where, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> lambdaExp)
        {
            if (where)
            {
                InitParameter(lambdaExp);
                BodyExp = BodyExp.MergeExpression(lambdaExp.Body, ExpressionType.OrElse, Parameters);
            }
            return this;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Clone()
        {
            return new DynamicWhereExp<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(BodyExp, Parameters);
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> Build()
        {
            if (BodyExp == null)
            {
                Parameters = new List<ParameterExpression>()
                {
                    Expression.Parameter(typeof(T),"a"),
                    Expression.Parameter(typeof(T2),"b"),
                    Expression.Parameter(typeof(T3),"c"),
                    Expression.Parameter(typeof(T4),"d"),
                    Expression.Parameter(typeof(T5),"e"),
                    Expression.Parameter(typeof(T6),"f"),
                    Expression.Parameter(typeof(T7),"g"),
                    Expression.Parameter(typeof(T8),"h"),
                    Expression.Parameter(typeof(T9),"i"),
                    Expression.Parameter(typeof(T10),"j"),
                    Expression.Parameter(typeof(T11),"k"),
                    Expression.Parameter(typeof(T12),"l")
                };
                BodyExp = Expression.Constant(true);
            }
            var lambdaExp = (Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>>)Expression.Lambda(BodyExp, Parameters);
            return lambdaExp;
        }
    }
    #endregion
}
