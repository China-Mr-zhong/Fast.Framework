using System;
using System.Reflection;

namespace Fast.Framework
{

    /// <summary>
    /// 设置成员信息
    /// </summary>
    public class SetMemberInfo
    {

        /// <summary>
        /// 成员信息
        /// </summary>
        public MemberInfo MemberInfo { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public Lazy<object> Value { get; set; }

        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }
    }
}

