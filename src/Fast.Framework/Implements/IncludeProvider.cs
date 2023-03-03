using Fast.Framework.Abstract;
using Fast.Framework.Factory;
using Fast.Framework.Interfaces;
using Fast.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Implements
{

    /// <summary>
    /// 包括提供者实现类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IncludeProvider<T, TProperty> : QueryProvider<T>, IInclude<T, TProperty> where TProperty : class
    {
        /// <summary>
        /// Ado
        /// </summary>
        public IAdo Ado { get; }

        /// <summary>
        /// 包括信息
        /// </summary>
        public IncludeInfo IncludeInfo { get; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="queryBuilder">查询构建</param>
        /// <param name="IncludeInfo">包括信息</param>
        public IncludeProvider(IAdo ado, QueryBuilder queryBuilder, IncludeInfo IncludeInfo) : base(ado, queryBuilder)
        {
            this.Ado = ado;
            this.IncludeInfo = IncludeInfo;
        }
    }
}
