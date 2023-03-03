using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Fast.Framework.Interfaces;
using Fast.Framework.Extensions;
using Fast.Framework.Models;
using Fast.Framework.Abstract;
using Fast.Framework.Enum;
using Fast.Framework.Factory;
using System.Data.Common;

namespace Fast.Framework.Implements
{

    /// <summary>
    /// 删除实现类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DeleteProvider<T> : IDelete<T> where T : class
    {

        /// <summary>
        /// 删除建造者
        /// </summary>
        public DeleteBuilder DeleteBuilder { get; }

        /// <summary>
        /// Ado
        /// </summary>
        private readonly IAdo ado;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado">ado</param>
        /// <param name="deleteBuilder">删除建造者</param>
        public DeleteProvider(IAdo ado, DeleteBuilder deleteBuilder)
        {
            this.ado = ado;
            this.DeleteBuilder = deleteBuilder;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public IDelete<T> Clone()
        {
            var deleteProvider = new DeleteProvider<T>(ado.Clone(), DeleteBuilder.Clone());
            return deleteProvider;
        }

        /// <summary>
        /// 作为
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public IDelete<T> As(string tableName)
        {
            DeleteBuilder.EntityInfo.TableName = tableName;
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="columnName">列名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public IDelete<T> WhereColumn(string columnName, object value)
        {
            if (DeleteBuilder.DbParameters.Any(a => a.ParameterName == columnName))
            {
                throw new Exception($"列名称{columnName}已存在条件,不允许重复添加.");
            }
            var whereStr = $"{ado.DbOptions.DbType.MappingIdentifier().Insert(1, columnName)} = {ado.DbOptions.DbType.MappingParameterSymbol()}{columnName}";
            DeleteBuilder.Where.Add(whereStr);
            DeleteBuilder.DbParameters.Add(new FastParameter(columnName, value));
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="whereColumns">条件列</param>
        /// <returns></returns>
        public IDelete<T> WhereColumns(Dictionary<string, object> whereColumns)
        {
            var sqlParameter = DeleteBuilder.DbParameters.FirstOrDefault(f => whereColumns.ContainsKey(f.ParameterName));
            if (sqlParameter != null)
            {
                throw new Exception($"列名称{sqlParameter.ParameterName}已存在条件,不允许重复添加.");
            }
            var whereStr = whereColumns.Keys.Select(s => $"{ado.DbOptions.DbType.MappingIdentifier().Insert(1, s)} = {ado.DbOptions.DbType.MappingParameterSymbol()}{s}");
            DeleteBuilder.Where.Add(string.Join(" AND ", whereStr));
            DeleteBuilder.DbParameters.AddRange(whereColumns.Select(s => new FastParameter(s.Key, s.Value)));
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IDelete<T> Where(Expression<Func<T, bool>> expression)
        {
            DeleteBuilder.Expressions.ExpressionInfos.Add(new ExpressionInfo()
            {
                ResolveSqlOptions = new ResolveSqlOptions()
                {
                    DbType = ado.DbOptions.DbType,
                    ResolveSqlType = ResolveSqlType.Where
                },
                Expression = expression
            });
            return this;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public int Exceute()
        {
            return ado.ExecuteNonQuery(CommandType.Text, DeleteBuilder.ToSqlString(), ado.ConvertParameter(DeleteBuilder.DbParameters));
        }

        /// <summary>
        /// 执行异步
        /// </summary>
        /// <returns></returns>
        public Task<int> ExceuteAsync()
        {
            return ado.ExecuteNonQueryAsync(CommandType.Text, DeleteBuilder.ToSqlString(), ado.ConvertParameter(DeleteBuilder.DbParameters));
        }

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        public string ToSqlString()
        {
            return this.DeleteBuilder.ToSqlString();
        }

    }
}

