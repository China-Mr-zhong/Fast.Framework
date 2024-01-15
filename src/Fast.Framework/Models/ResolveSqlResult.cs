using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 解析Sql结果
    /// </summary>
    public class ResolveSqlResult
    {

        /// <summary>
        /// Sql字符串
        /// </summary>
        public string SqlString { get; set; }

        /// <summary>
        /// 数据库参数
        /// </summary>
        public List<FastParameter> DbParameters { get; set; }

        /// <summary>
        /// 成员名称
        /// </summary>
        public List<string> MemberNames { get; set; }

        /// <summary>
        /// 设置成员信息
        /// </summary>
        public List<SetMemberInfo> SetMemberInfos { get; set; }

        /// <summary>
        /// Lambda参数信息
        /// </summary>
        public List<LambdaParameterInfo> LambdaParameterInfos { get; set; }
    }
}
