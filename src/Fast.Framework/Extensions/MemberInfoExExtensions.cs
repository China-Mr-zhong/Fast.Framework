using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Fast.Framework.Models;
using Fast.Framework.Utils;

namespace Fast.Framework.Extensions
{

    /// <summary>
    /// 成员信息扩展类
    /// </summary>
    public static class MemberInfoExExtensions
    {

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="memberInfos">成员信息</param>
        /// <param name="compilerVar">编译器变量值</param>
        /// <param name="memberName">成员名称</param>
        /// <returns></returns>
        public static object GetValue(this Stack<MemberInfoEx> memberInfos, object compilerVar, out string memberName)
        {
            var names = new List<string>();
            foreach (var item in memberInfos)
            {
                if (!item.Member.Name.StartsWith("CS$<>8__locals"))
                {
                    names.Add(item.Member.Name);
                }
                if (item.ArrayIndex.Count > 0)
                {
                    names.Add(string.Join("_", item.ArrayIndex.ToList()));
                }
                if (item.Member.MemberType == MemberTypes.Field)
                {
                    var fieldInfo = (FieldInfo)item.Member;
                    compilerVar = fieldInfo.GetValue(compilerVar);
                }
                else if (item.Member.MemberType == MemberTypes.Property)
                {
                    var propertyInfo = (PropertyInfo)item.Member;
                    compilerVar = propertyInfo.GetValue(compilerVar);
                }
                else
                {
                    throw new NotSupportedException($"不支持获取 {item.Member.MemberType} 类型值.");
                }
                if (item.ArrayIndex != null && item.ArrayIndex.Count > 0)
                {
                    foreach (var index in item.ArrayIndex)
                    {
                        compilerVar = (compilerVar as Array).GetValue(index);
                    }
                }
            }
            memberName = string.Join("_", names);
            return compilerVar;
        }
    }
}

