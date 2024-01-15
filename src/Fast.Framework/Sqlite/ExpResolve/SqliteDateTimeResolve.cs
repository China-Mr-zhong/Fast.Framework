using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// Sqlite日期时间解析
    /// </summary>
    public class SqliteDateTimeResolve : ExpMemberResolve
    {

        /// <summary>
        /// 模板
        /// </summary>
        private readonly Dictionary<string, string> templates;

        /// <summary>
        /// 构造方法
        /// </summary>
        public SqliteDateTimeResolve()
        {
            templates = new Dictionary<string, string>
            {
                { nameof(DateTime.Date), "STRFTIME( '%Y-%m-%d',{0} )" },
                { nameof(DateTime.Year),"STRFTIME( '%Y',{0} )" },
                { nameof(DateTime.Month),"STRFTIME( '%m',{0} )"},
                { nameof(DateTime.Day),"STRFTIME( '%j',{0} )"},
                { nameof(DateTime.Hour),"STRFTIME( '%H',{0} )"},
                { nameof(DateTime.Minute),"STRFTIME( '%M',{0} )"},
                { nameof(DateTime.Second),"STRFTIME( '%S',{0} )"},
                { nameof(DateTime.Millisecond),"STRFTIME( '%f',{0} )"},
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
