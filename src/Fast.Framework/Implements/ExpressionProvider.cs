using Fast.Framework.Enum;
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
    /// 表达式提供者
    /// </summary>
    public class ExpressionProvider : IExpressions
    {

        /// <summary>
        /// 解析完成
        /// </summary>
        public bool ResolveComplete { get; set; }

        /// <summary>
        /// 表达式信息
        /// </summary>
        public List<ExpressionInfo> ExpressionInfos { get; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public ExpressionProvider()
        {
            this.ExpressionInfos = new List<ExpressionInfo>();
        }
    }
}
