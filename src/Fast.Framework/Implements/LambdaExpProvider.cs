using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 表达式提供者
    /// </summary>
    public class LambdaExpProvider : ILambdaExp
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
        public LambdaExpProvider()
        {
            ExpressionInfos = new List<ExpressionInfo>();
        }
    }
}
