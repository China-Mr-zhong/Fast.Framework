using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Fast.Framework.Utils
{
    /// <summary>
    /// Unicode工具类
    /// </summary>
    public static class Unicode
    {
        /// <summary>
        /// 字符串转Unicdoe
        /// </summary>
        /// <param name="scrString">源字符串</param>
        /// <returns></returns>
        public static string StringToUnicode(string scrString)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(scrString);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Unicode转字符串
        /// </summary>
        /// <param name="unicode">Unicode编码</param>
        /// <returns></returns>
        public static string UnicodeToString(string unicode)
        {
            string str = "";
            string[] strList = unicode.Split("\\u");
            for (int i = 1; i < strList.Length; i++)
            {
                str += (char)int.Parse(strList[i], NumberStyles.HexNumber);
            }
            return str;
        }
    }
}
