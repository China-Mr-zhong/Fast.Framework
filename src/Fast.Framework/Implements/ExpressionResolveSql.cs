using Fast.Framework.CustomAttribute;
using Fast.Framework.Enum;
using Fast.Framework.Extensions;
using Fast.Framework.Interfaces;
using Fast.Framework.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Implements
{
    /// <summary>
    /// 表达式到Sql
    /// </summary>
    public class ExpressionResolveSql
    {

        /// <summary>
        /// 选项
        /// </summary>
        private readonly ResolveSqlOptions options;

        /// <summary>
        /// Sql构建
        /// </summary>
        public StringBuilder sqlBuilder;

        /// <summary>
        /// 数据库参数
        /// </summary>
        public List<FastParameter> dbParameters;

        /// <summary>
        /// 获取值
        /// </summary>
        public ExpressionResolveValue getValue;

        /// <summary>
        /// 成员信息
        /// </summary>
        private Stack<MemberInfoEx> memberInfos;

        /// <summary>
        /// 数组索引
        /// </summary>
        private Stack<int> arrayIndexs;

        /// <summary>
        /// 首次表达式
        /// </summary>
        private Expression bodyExpression;

        /// <summary>
        /// 构造方法
        /// </summary>
        public ExpressionResolveSql(ResolveSqlOptions options)
        {
            this.options = options;
            this.getValue = new ExpressionResolveValue();
            this.sqlBuilder = new StringBuilder();
            this.dbParameters = new List<FastParameter>();
            this.memberInfos = new Stack<MemberInfoEx>();
            this.arrayIndexs = new Stack<int>();
        }

        /// <summary>
        /// 访问
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        public Expression Visit(Expression node)
        {
            //Console.WriteLine($"当前访问 {node.NodeType} 类型表达式");
            switch (node)
            {
                case LambdaExpression:
                    {
                        return Visit(VisitLambda(node as LambdaExpression));
                    };
                case UnaryExpression:
                    {
                        return Visit(VisitUnary(node as UnaryExpression));
                    }
                case BinaryExpression:
                    {
                        return Visit(VisitBinary(node as BinaryExpression));
                    }
                case MethodCallExpression:
                    {
                        return Visit(VisitMethodCall(node as MethodCallExpression));
                    }
                case ConditionalExpression:
                    {
                        return Visit(VisitConditional(node as ConditionalExpression));
                    }
                case NewExpression:
                    {
                        return Visit(VisitNew(node as NewExpression));
                    }
                case MemberInitExpression:
                    {
                        return Visit(VisitMemberInit(node as MemberInitExpression));
                    }
                case NewArrayExpression:
                    {
                        return Visit(VisitNewArray(node as NewArrayExpression));
                    }
                case ListInitExpression:
                    {
                        return Visit(VisitListInit(node as ListInitExpression));
                    }
                case MemberExpression:
                    {
                        return Visit(VisitMember(node as MemberExpression));
                    }
                case ConstantExpression:
                    {
                        return VisitConstant(node as ConstantExpression);
                    };
                default: return null;
            }
        }

        /// <summary>
        /// 访问Lambda表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">节点</param>
        /// <returns></returns>
        private Expression VisitLambda(LambdaExpression lambdaExpression)
        {
            bodyExpression = lambdaExpression.Body;
            return bodyExpression;
        }

        /// <summary>
        /// 访问一元表达式
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        private Expression VisitUnary(UnaryExpression node)
        {
            Visit(node.Operand);
            if ((options.ResolveSqlType == ResolveSqlType.Where || options.ResolveSqlType == ResolveSqlType.Join || options.ResolveSqlType == ResolveSqlType.Having) && node.NodeType == ExpressionType.Not)
            {
                sqlBuilder.Append(options.DbType == DbType.PostgreSQL ? " = FALSE" : " = 0");
            }
            return null;
        }

        /// <summary>
        /// 访问二元表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Expression VisitBinary(BinaryExpression node)
        {
            #region 解析数组索引访问
            if (node.NodeType == ExpressionType.ArrayIndex)
            {
                var index = Convert.ToInt32((node.Right as ConstantExpression).Value);
                arrayIndexs.Push(index);
                return Visit(node.Left);
            }
            #endregion

            sqlBuilder.Append("( ");

            Visit(node.Left);

            #region 解析布尔类型特殊处理
            if ((options.ResolveSqlType == ResolveSqlType.Where || options.ResolveSqlType == ResolveSqlType.Join || options.ResolveSqlType == ResolveSqlType.Having) && node.Left is not BinaryExpression && node.Left.Type.Equals(typeof(bool))
                && node.NodeType != ExpressionType.Equal && node.NodeType != ExpressionType.NotEqual && (node.Left.NodeType == ExpressionType.MemberAccess || node.Left.NodeType == ExpressionType.Constant))
            {
                if (options.DbType == DbType.PostgreSQL)
                {
                    sqlBuilder.Append(" = TRUE");
                }
                else
                {
                    sqlBuilder.Append(" = 1");
                }
            }
            #endregion

            var op = node.NodeType.ExpressionTypeMapping();

            #region IS NULL 和 IS NOT NULL 特殊处理
            if ((options.ResolveSqlType == ResolveSqlType.Where || options.ResolveSqlType == ResolveSqlType.Join || options.ResolveSqlType == ResolveSqlType.Having) && (node.NodeType == ExpressionType.Equal || node.NodeType == ExpressionType.NotEqual) && node.Right.NodeType == ExpressionType.Constant)
            {
                var constantExpression = node.Right as ConstantExpression;
                if (constantExpression.Value == null)
                {
                    if (op == "=")
                    {
                        sqlBuilder.Append(" IS NULL )");
                    }
                    else if (op == "<>")
                    {
                        sqlBuilder.Append(" IS NOT NULL )");
                    }
                    return null;
                }
            }
            #endregion

            #region Sqlite字符串拼接特殊处理
            if (options.DbType == DbType.SQLite && node.NodeType == ExpressionType.Add)
            {
                if (node.Left.Type.Equals(typeof(string)) || node.Right.Type.Equals(typeof(string)))
                {
                    op = "||";
                }
            }
            #endregion

            sqlBuilder.Append($" {op} ");

            Visit(node.Right);

            #region 解析布尔类型特殊处理
            if ((options.ResolveSqlType == ResolveSqlType.Where || options.ResolveSqlType == ResolveSqlType.Join || options.ResolveSqlType == ResolveSqlType.Having) && node.Right is not BinaryExpression && node.Right.Type.Equals(typeof(bool))
                && node.NodeType != ExpressionType.Equal && node.NodeType != ExpressionType.NotEqual && (node.Right.NodeType == ExpressionType.MemberAccess || node.Right.NodeType == ExpressionType.Constant))
            {
                if (options.DbType == DbType.PostgreSQL)
                {
                    sqlBuilder.Append(" = TRUE");
                }
                else
                {
                    sqlBuilder.Append(" = 1");
                }
            }
            #endregion

            sqlBuilder.Append(" )");

            return null;
        }

        /// <summary>
        /// 访问方法表达式
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        private Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "SubQuery")
            {
                sqlBuilder.Append("( ");
                Visit(node.Arguments[0]);
                sqlBuilder.Append(" )");
            }
            else
            {
                if (options.DbType.MethodMapping().ContainsKey(node.Method.Name))
                {
                    options.DbType.MethodMapping()[node.Method.Name].Invoke(this, node, sqlBuilder);
                }
                else if (node.Object != null && node.Object.Type.IsVariableBoundArray)//多维数组处理
                {
                    var resolve = new ExpressionResolveSql(new ResolveSqlOptions());
                    var args = new List<int>();
                    foreach (var item in node.Arguments)
                    {
                        args.Add(Convert.ToInt32((resolve.Visit(item) as ConstantExpression).Value));
                    }
                    var obj = (resolve.Visit(node.Object) as ConstantExpression).Value;
                    var value = (obj as Array).GetValue(args.ToArray());
                    Visit(Expression.Constant(value));
                }
                else
                {
                    throw new NotImplementedException($"未实现 {node.Method.Name} 方法解析,可通过DbType枚举的扩展方法自定义添加.");
                }
            }
            return null;
        }

        /// <summary>
        /// 访问条件表达式树
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        private Expression VisitConditional(ConditionalExpression node)
        {
            sqlBuilder.Append("CASE WHEN ");
            Visit(node.Test);
            sqlBuilder.Append(" THEN ");
            Visit(node.IfTrue);
            sqlBuilder.Append(" ELSE ");
            Visit(node.IfFalse);
            sqlBuilder.Append(" END");
            return null;
        }

        /// <summary>
        /// 访问对象表达式
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        private Expression VisitNew(NewExpression node)
        {
            if (node.Type.Name.StartsWith("<>f__AnonymousType"))
            {
                var flagAttribute = typeof(ResolveSqlType).GetField(options.ResolveSqlType.ToString()).GetCustomAttribute<FlagAttribute>(false);
                for (int i = 0; i < node.Members.Count; i++)
                {
                    if (options.ResolveSqlType == ResolveSqlType.NewAs)
                    {
                        Visit(node.Arguments[i]);
                        sqlBuilder.Append($" {flagAttribute?.Value} ");
                        var name = node.Members[i].GetCustomAttribute<ColumnAttribute>(false)?.Name;
                        if (options.IgnoreIdentifier)
                        {
                            sqlBuilder.Append(name == null ? node.Members[i].Name : name);
                        }
                        else
                        {
                            sqlBuilder.Append($"{options.DbType.MappingIdentifier().Insert(1, name == null ? node.Members[i].Name : name)}");
                        }
                    }
                    else if (options.ResolveSqlType == ResolveSqlType.NewAssignment)
                    {
                        var name = node.Members[i].GetCustomAttribute<ColumnAttribute>(false)?.Name;
                        if (options.IgnoreIdentifier)
                        {
                            sqlBuilder.Append(name == null ? node.Members[i].Name : name);
                        }
                        else
                        {
                            sqlBuilder.Append($"{options.DbType.MappingIdentifier().Insert(1, name == null ? node.Members[i].Name : name)}");
                        }
                        sqlBuilder.Append($" {flagAttribute?.Value} ");
                        Visit(node.Arguments[i]);
                    }
                    else
                    {
                        Visit(node.Arguments[i]);
                    }
                    if (i + 1 < node.Members.Count)
                    {
                        sqlBuilder.Append(',');
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 访问成员初始化表达式
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        private Expression VisitMemberInit(MemberInitExpression node)
        {
            var flagAttribute = typeof(ResolveSqlType).GetField(options.ResolveSqlType.ToString()).GetCustomAttribute<FlagAttribute>(false);
            for (int i = 0; i < node.Bindings.Count; i++)
            {
                if (node.Bindings[i].BindingType == MemberBindingType.Assignment)
                {
                    var memberAssignment = node.Bindings[i] as MemberAssignment;
                    if (options.ResolveSqlType == ResolveSqlType.NewAs)
                    {
                        Visit(memberAssignment.Expression);
                        var name = memberAssignment.Member.GetCustomAttribute<ColumnAttribute>(false)?.Name;
                        sqlBuilder.Append($" {flagAttribute?.Value} ");
                        if (options.IgnoreIdentifier)
                        {
                            sqlBuilder.Append(name == null ? memberAssignment.Member.Name : name);
                        }
                        else
                        {
                            sqlBuilder.Append($"{options.DbType.MappingIdentifier().Insert(1, name == null ? memberAssignment.Member.Name : name)}");
                        }
                    }
                    else if (options.ResolveSqlType == ResolveSqlType.NewAssignment)
                    {
                        var name = memberAssignment.Member.GetCustomAttribute<ColumnAttribute>(false)?.Name;
                        if (options.IgnoreIdentifier)
                        {
                            sqlBuilder.Append(name == null ? memberAssignment.Member.Name : name);
                        }
                        else
                        {
                            sqlBuilder.Append($"{options.DbType.MappingIdentifier().Insert(1, name == null ? memberAssignment.Member.Name : name)}");
                        }
                        sqlBuilder.Append($" {flagAttribute?.Value} ");
                        Visit(memberAssignment.Expression);
                    }
                    else
                    {
                        sqlBuilder.Append(memberAssignment.Member.Name);
                    }
                    if (i + 1 < node.Bindings.Count)
                    {
                        sqlBuilder.Append(',');
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 访问对象数组表达式
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        private Expression VisitNewArray(NewArrayExpression node)
        {
            for (int i = 0; i < node.Expressions.Count; i++)
            {
                Visit(node.Expressions[i]);
                if (i + 1 < node.Expressions.Count)
                {
                    sqlBuilder.Append(',');
                }
            }
            return null;
        }

        /// <summary>
        /// 访问列表初始化表达式
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        private Expression VisitListInit(ListInitExpression node)
        {
            if (node.CanReduce)
            {
                var blockExpression = node.Reduce() as BlockExpression;
                var expressions = blockExpression.Expressions.Skip(1).SkipLast(1).ToList();
                for (int i = 0; i < expressions.Count; i++)
                {
                    var methodCallExpression = expressions[i] as MethodCallExpression;
                    foreach (var item in methodCallExpression.Arguments)
                    {
                        Visit(item);
                    }
                    if (i + 1 < expressions.Count)
                    {
                        sqlBuilder.Append(',');
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 访问成员表达式
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        private Expression VisitMember(MemberExpression node)
        {
            #region 多级访问限制
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter && memberInfos.Count > 0)
            {
                var parameterExpression = node.Expression as ParameterExpression;
                throw new Exception($"不支持{parameterExpression.Name}.{node.Member.Name}.{memberInfos.Pop().Member.Name}多级访问.");
            }
            #endregion

            #region Datetime特殊处理
            if (node.Type.Equals(typeof(DateTime)) && node.Expression == null)
            {
                memberInfos.Push(new MemberInfoEx()
                {
                    ArrayIndex = arrayIndexs,
                    Member = node.Member
                });
                return Expression.Constant(default(DateTime));
            }
            #endregion

            if (node.Expression != null)
            {
                if (node.Expression.NodeType == ExpressionType.Parameter)
                {
                    var parameterExpression = (ParameterExpression)node.Expression;
                    if (!options.IgnoreParameter)
                    {
                        if (options.IgnoreIdentifier)
                        {
                            sqlBuilder.Append($"{parameterExpression.Name}.");
                        }
                        else
                        {
                            sqlBuilder.Append($"{options.DbType.MappingIdentifier().Insert(1, parameterExpression.Name)}.");
                        }
                    }
                    var name = node.Member.Name;
                    if (!options.IgnoreColumnAttribute)
                    {
                        var columnAttribute = node.Member.GetCustomAttribute<ColumnAttribute>(false);
                        if (columnAttribute != null)
                        {
                            name = columnAttribute.Name;
                        }
                    }
                    if (options.IgnoreIdentifier)
                    {
                        sqlBuilder.Append(name);
                    }
                    else
                    {
                        sqlBuilder.Append(options.DbType.MappingIdentifier().Insert(1, name));
                    }

                    #region 解析布尔类型特殊处理
                    if ((options.ResolveSqlType == ResolveSqlType.Where || options.ResolveSqlType == ResolveSqlType.Join || options.ResolveSqlType == ResolveSqlType.Having) && node.Type.Equals(typeof(bool)) && bodyExpression.NodeType == ExpressionType.MemberAccess)
                    {
                        if (options.DbType == DbType.PostgreSQL)
                        {
                            sqlBuilder.Append(" = TRUE");
                        }
                        else
                        {
                            sqlBuilder.Append(" = 1");
                        }
                    }
                    #endregion

                }
                else if (node.Expression.NodeType == ExpressionType.MemberAccess || node.Expression.NodeType == ExpressionType.Constant)
                {
                    memberInfos.Push(new MemberInfoEx()
                    {
                        ArrayIndex = arrayIndexs,
                        Member = node.Member
                    });
                }
            }

            if (arrayIndexs.Count > 0)
            {
                arrayIndexs = new Stack<int>();
            }
            return node.Expression;
        }

        /// <summary>
        /// 访问常量表达式
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        private Expression VisitConstant(ConstantExpression node)
        {
            var value = node.Value;
            if (memberInfos.Count > 0)
            {
                value = memberInfos.GetValue(value, out var memberName);//获取成员变量值
                if (value is IQuery)
                {
                    var query = value as IQuery;
                    dbParameters.AddRange(query.QueryBuilder.DbParameters);
                    sqlBuilder.Append(query.QueryBuilder.ToSqlString());
                }
                else if (value is IList)
                {
                    if (value.GetType().IsVariableBoundArray)//多维数组判断
                    {
                        return Expression.Constant(value);
                    }
                    var list = value as IList;
                    var parNames = new List<string>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        var parameter = dbParameters.FirstOrDefault(f => f.ParameterName.StartsWith($"{memberName}_{i}_"));
                        if (parameter == null)
                        {
                            var newName = $"{memberName}_{i}_{options.ParameterIndex}";
                            parNames.Add(newName);

                            parameter = new FastParameter(newName, list[i]);

                            dbParameters.Add(parameter);

                            options.ParameterIndex++;
                        }
                        else
                        {
                            parNames.Add(parameter.ParameterName);
                        }
                    }
                    sqlBuilder.Append(string.Join(",", parNames.Select(s => $"{options.DbType.MappingParameterSymbol()}{s}")));
                }
                else//普通成员变量处理
                {
                    var parameter = dbParameters.FirstOrDefault(f => f.ParameterName.StartsWith($"{memberName}_"));
                    if (parameter == null)
                    {
                        var newName = $"{memberName}_{options.ParameterIndex}";
                        parameter = new FastParameter(newName, value);
                        dbParameters.Add(parameter);

                        sqlBuilder.Append($"{options.DbType.MappingParameterSymbol()}{newName}");
                        options.ParameterIndex++;
                    }
                    else
                    {
                        sqlBuilder.Append($"{options.DbType.MappingParameterSymbol()}{parameter.ParameterName}");
                    }
                }
                memberInfos.Clear();
            }
            else
            {
                if (node.Type.Equals(typeof(bool)))
                {
                    if (bodyExpression.NodeType == ExpressionType.Constant)
                    {
                        value = Convert.ToInt32(value);
                        value = $"1 = {value}";
                    }
                    else if (options.DbType == DbType.PostgreSQL)
                    {
                        //PostgreSQL 特殊处理 bool转换成大写
                        sqlBuilder.Append(Convert.ToString(value).ToUpper());
                        return node;
                    }
                    else
                    {
                        value = Convert.ToInt32(value);
                    }
                }
                value = AddQuotes(node.Type, value);
                sqlBuilder.Append(Convert.ToString(value));
            }
            return Expression.Constant(value);
        }

        /// <summary>
        /// 添加引号
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private static object AddQuotes(Type type, object value)
        {
            if (type.IsValueType && !type.Equals(typeof(DateTime)))
            {
                return value;
            }
            return $"'{value}'";
        }
    }
}
