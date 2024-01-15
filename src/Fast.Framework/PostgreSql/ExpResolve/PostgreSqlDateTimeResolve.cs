using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// PostgreSql日期时间解析
    /// </summary>
    public class PostgreSqlDateTimeResolve : ExpMemberResolve
    {

        /// <summary>
        /// 模板
        /// </summary>
        private readonly Dictionary<string, string> templates;

        /// <summary>
        /// 构造方法
        /// </summary>
        public PostgreSqlDateTimeResolve()
        {
            templates = new Dictionary<string, string>
            {
                { nameof(DateTime.Date), "{0}::DATE" },
                { nameof(DateTime.Year),"EXTRACT( YEAR FROM {0} )" },
                { nameof(DateTime.Month),"EXTRACT( MONTH FROM {0} )"},
                { nameof(DateTime.Day),"EXTRACT( DAY FROM {0} )"},
                { nameof(DateTime.Hour),"EXTRACT( HOUR FROM {0}::TIMESTAMP )"},
                { nameof(DateTime.Minute),"EXTRACT( MINUTE FROM {0}::TIMESTAMP )"},
                { nameof(DateTime.Second),"EXTRACT( SECOND FROM {0}::TIMESTAMP )"},
                { nameof(DateTime.DayOfYear),"EXTRACT( DOY FROM {0} )"},
                { nameof(DateTime.DayOfWeek),"EXTRACT( DOW FROM {0} )"}
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
