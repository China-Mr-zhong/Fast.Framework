using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Fast.Framework
{

    /// <summary>
    /// 成员信息扩展类
    /// </summary>
    public static class MemberInfoExtensions
    {

        /// <summary>
        /// 获取成员类型
        /// </summary>
        /// <param name="memberInfo">成员信息</param>
        /// <returns></returns>
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            if (memberInfo.MemberType == MemberTypes.Property)
            {
                var propertyInfo = memberInfo as PropertyInfo;
                return propertyInfo.PropertyType;
            }
            else if (memberInfo.MemberType == MemberTypes.Field)
            {
                var fieldInfo = memberInfo as FieldInfo;
                return fieldInfo.FieldType;
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="memberInfo">成员信息</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static object GetValue(this MemberInfo memberInfo, object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (memberInfo.MemberType == MemberTypes.Property)
            {
                var propertyInfo = memberInfo as PropertyInfo;
                return propertyInfo.GetValue(obj);
            }
            else if (memberInfo.MemberType == MemberTypes.Field)
            {
                var fieldInfo = memberInfo as FieldInfo;
                return fieldInfo.GetValue(obj);
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="memberInfo">成员信息</param>
        /// <param name="obj">对象</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static void SetValue(this MemberInfo memberInfo, object obj, object value)
        {
            if (obj != null)
            {
                var entityInfo = obj.GetType().GetEntityInfo();
                if (entityInfo.IsAnonymousType)
                {
                    var fieldInfo = entityInfo.EntityType.GetField($"<{memberInfo.Name}>i__Field", BindingFlags.NonPublic | BindingFlags.Instance);
                    fieldInfo.SetValue(obj, value);
                }
                else if (memberInfo.MemberType == MemberTypes.Property)
                {
                    var propertyInfo = memberInfo as PropertyInfo;
                    propertyInfo.SetValue(obj, value);
                }
                else if (memberInfo.MemberType == MemberTypes.Field)
                {
                    var fieldInfo = memberInfo as FieldInfo;
                    fieldInfo.SetValue(obj, value);
                }
            }
        }

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
                if (!item.MemberInfo.Name.StartsWith("CS$<>8__locals"))
                {
                    names.Add(item.MemberInfo.Name);
                }
                if (item.ArrayIndex.Count > 0)
                {
                    names.Add(string.Join("_", item.ArrayIndex.ToList()));
                }
                compilerVar = item.MemberInfo.GetValue(compilerVar);
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

