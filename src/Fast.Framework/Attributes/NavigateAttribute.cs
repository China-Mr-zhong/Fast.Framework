using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 导航属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NavigateAttribute : Attribute
    {

        /// <summary>
        /// 主名称
        /// </summary>
        public string MainName { get; set; }

        /// <summary>
        /// 子名称
        /// </summary>
        public string ChildName { get; set; }

        public NavigateAttribute()
        {
        }
    }
}
