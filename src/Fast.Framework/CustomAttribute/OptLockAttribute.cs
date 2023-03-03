using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.CustomAttribute
{

    /// <summary>
    /// 乐观锁属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OptLockAttribute : Attribute
    {
    }
}
