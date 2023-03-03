using Fast.Framework.Enum;
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
    /// 表达式接口类
    /// </summary>
    public interface IExpressions
    {

        /// <summary>
        /// 解析完成
        /// </summary>
        bool ResolveComplete { get; set; }

        /// <summary>
        /// 表达式信息
        /// </summary>
        List<ExpressionInfo> ExpressionInfos { get; }
    }
}
