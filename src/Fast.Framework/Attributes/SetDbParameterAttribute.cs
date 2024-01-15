using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 设置数据库参数属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SetDbParameterAttribute : Attribute
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public System.Data.DbType DbType { get; set; }
    }
}
