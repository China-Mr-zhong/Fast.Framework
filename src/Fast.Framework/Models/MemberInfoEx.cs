using System;
using System.Collections.Generic;
using System.Reflection;

namespace Fast.Framework
{

    /// <summary>
    /// 成员信息扩展
    /// </summary>
    public class MemberInfoEx
    {
        /// <summary>
        /// 数组索引
        /// </summary>
        public Stack<int> ArrayIndex { get; set; }

        /// <summary>
        /// 成员信息
        /// </summary>
        public MemberInfo MemberInfo { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public MemberInfoEx()
        {
            ArrayIndex = new Stack<int>();
        }
    }
}

