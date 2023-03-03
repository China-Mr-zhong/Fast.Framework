using Fast.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Interfaces
{

    /// <summary>
    /// 包括接口类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public interface IInclude<T, TProperty> : IQuery<T> where TProperty : class
    {
        /// <summary>
        /// Ado
        /// </summary>
        IAdo Ado { get; }

        /// <summary>
        /// 包括信息
        /// </summary>
        IncludeInfo IncludeInfo { get; }
    }
}
