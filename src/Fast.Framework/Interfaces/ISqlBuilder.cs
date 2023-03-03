using Fast.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Interfaces
{

    /// <summary>
    /// Sql建造者接口类
    /// </summary>
    public interface ISqlBuilder
    {
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
