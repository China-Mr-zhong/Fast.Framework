using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 日期时间解析
    /// </summary>
    public class MysqlDateTimeResolve : ExpMemberResolve
    {

        /// <summary>
        /// 模板
        /// </summary>
        private readonly Dictionary<string, string> templates;

        /// <summary>
        /// 构造方法
        /// </summary>
        public MysqlDateTimeResolve()
        {
            templates = new Dictionary<string, string>
            {
                { nameof(DateTime.Date), "DATE_FORMAT( {0},'%Y-%m-%d' )" },
                { nameof(DateTime.Year),"YEAR( {0} )" },
                { nameof(DateTime.Month),"MONTH( {0} )"},
                { nameof(DateTime.Day),"DAY( {0} )"},
                { nameof(DateTime.Hour),"HOUR( {0} )"},
                { nameof(DateTime.Minute),"MINUTE( {0} )"},
                { nameof(DateTime.Second),"SECOND( {0} )"},
                { nameof(DateTime.DayOfYear),"DAYOFYEAR( {0} )"},
                { nameof(DateTime.DayOfWeek),"DAYOFWEEK( {0} )"}
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
