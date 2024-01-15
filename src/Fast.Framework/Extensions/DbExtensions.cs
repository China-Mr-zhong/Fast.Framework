using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 数据库扩展
    /// </summary>
    public static class DbExtensions
    {

        /// <summary>
        /// 检查Sql注入
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        public static string CheckSqlInject(this string input)
        {
            var pattern = "create |insert |delete |update |select |;|--|exec |execute |drop |alert ";
            var matches = Regex.Matches(input, pattern, RegexOptions.IgnoreCase);
            if (matches.Count > 0)
            {
                throw new Exception($"检查到非法字符串:{string.Join(" ", matches.Select(s => s.Value))}");
            }
            return input;
        }
    }
}
