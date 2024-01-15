using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// Sql建造者接口类
    /// </summary>
    public interface ISqlBuilder
    {
        /// <summary>
        /// Lambda表达式
        /// </summary>
        ILambdaExp LambdaExp { get; }

        /// <summary>
        /// 实体信息
        /// </summary>
        EntityInfo EntityInfo { get; set; }

        /// <summary>
        /// 参数信息
        /// </summary>
        List<FastParameter> DbParameters { get; set; }

        /// <summary>
        /// 到Sql字符串
        /// </summary>
        /// <returns></returns>
        string ToSqlString();
    }
}
