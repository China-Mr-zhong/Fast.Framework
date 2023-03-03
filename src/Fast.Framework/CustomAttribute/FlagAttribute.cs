using System;


namespace Fast.Framework.CustomAttribute
{

    /// <summary>
    /// 标记属性
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class FlagAttribute : Attribute
    {
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="value">值</param>
        public FlagAttribute(object value)
        {
            this.Value = value;
        }
    }
}

