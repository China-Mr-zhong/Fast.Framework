using System;
using System.Collections.Generic;
using System.Text;
using Fast.Framework.Interfaces;
using Fast.Framework.Extensions;
using Fast.Framework.Models;
using Fast.Framework.Enum;
using Fast.Framework.Implements;
using Fast.Framework.Factory;

namespace Fast.Framework.Abstract
{

    /// <summary>
    /// 删除建造者抽象类
    /// </summary>
    public abstract class DeleteBuilder : ISqlBuilder
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public virtual DbType DbType { get; private set; } = DbType.SQLServer;

        /// <summary>
        /// 表达式
        /// </summary>
        public IExpressions Expressions { get; }

        /// <summary>
        /// 实体信息
        /// </summary>
        public EntityInfo EntityInfo { get; set; }

        /// <summary>
        /// 数据库参数
        /// </summary>
        public List<FastParameter> DbParameters { get; set; }

        /// <summary>
        /// 条件
        /// </summary>
        public List<string> Where { get; }

        /// <summary>
        /// 删除模板
        /// </summary>
        public virtual string DeleteTemplate { get; set; } = "DELETE FROM {0}";

        /// <summary>
        /// 条件模板
        /// </summary>
        public virtual string WhereTemplate { get; set; } = "WHERE {0}";

        /// <summary>
        /// 构造方法
        /// </summary>
        public DeleteBuilder()
        {
            Expressions = new ExpressionProvider();
            DbParameters = new List<FastParameter>();
            Where = new List<string>();
        }

        /// <summary>
        /// 解析表达式
        /// </summary>
        public virtual void ResolveExpressions()
        {
            if (!this.Expressions.ResolveComplete)
            {
                foreach (var item in this.Expressions.ExpressionInfos)
                {
                    item.ResolveSqlOptions.IgnoreParameter = true;
                    var result = item.Expression.ResolveSql(item.ResolveSqlOptions);

                    if (item.IsFormat)
                    {
                        result.SqlString = string.Format(item.Template, result.SqlString);
                    }

                    if (item.ResolveSqlOptions.ResolveSqlType == ResolveSqlType.Where)
                    {
                        this.Where.Add(result.SqlString);
                    }

                    if (result.DbParameters.Count > 0)
                    {
                        this.DbParameters.AddRange(result.DbParameters);
                    }
                }
                this.Expressions.ResolveComplete = true;
            }
        }

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        public virtual string ToSqlString()
        {
            ResolveExpressions();
            var sb = new StringBuilder();

            var identifier = DbType.MappingIdentifier();
            sb.AppendFormat(DeleteTemplate, identifier.Insert(1, EntityInfo.TableName));
            if (Where.Count > 0)
            {
                sb.Append(' ');
                sb.AppendFormat(WhereTemplate, string.Join(" AND ", Where));
            }
            var sql = sb.ToString();
            return sql;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public virtual DeleteBuilder Clone()
        {
            this.ResolveExpressions();
            var deleteBuilder = BuilderFactory.CreateDeleteBuilder(DbType);
            deleteBuilder.EntityInfo = this.EntityInfo.Clone();
            deleteBuilder.DbParameters.AddRange(this.DbParameters);
            deleteBuilder.Where.AddRange(this.Where);
            return deleteBuilder;
        }

        /// <summary>
        /// 到字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToSqlString();
        }
    }
}

