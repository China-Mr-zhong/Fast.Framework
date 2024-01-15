using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// Oracle日期时间解析
    /// </summary>
    public class OracleDateTimeResolve : ExpMemberResolve
    {

        /// <summary>
        /// 模板
        /// </summary>
        private readonly Dictionary<string, string> templates;

        /// <summary>
        /// 构造方法
        /// </summary>
        public OracleDateTimeResolve()
        {
            templates = new Dictionary<string, string>
            {
                { nameof(DateTime.Date), "TRUNC( {0} )" },
                { nameof(DateTime.Year),"EXTRACT( YEAR FROM {0} )" },
                { nameof(DateTime.Month),"EXTRACT( MONTH FROM {0} )"},
                { nameof(DateTime.Day),"EXTRACT( DAY FROM {0} )"},
                { nameof(DateTime.Hour),"EXTRACT( HOUR FROM CAST( {0} AS TIMESTAMP ) )"},
                { nameof(DateTime.Minute),"EXTRACT( MINUTE FROM CAST( {0} AS TIMESTAMP ) )"},
                { nameof(DateTime.Second),"EXTRACT( SECOND FROM CAST( {0} AS TIMESTAMP ) )"}
            };
        }

        /// <summary>
        /// 解析名称
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <returns></returns>
        public override string ResolveName(string memberName)
        {
            if (!templates.ContainsKey(memberName))
            {
                throw new NotSupportedException($"名称:{memberName}暂不支持解析.");
            }
            return templates[memberName];
        }
    }
}
