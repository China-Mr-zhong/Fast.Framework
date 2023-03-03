using System;


namespace Fast.Framework.CustomAttribute
{

    /// <summary>
    /// 租户
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TenantAttribute : Attribute
    {

        /// <summary>
        /// 数据库ID
        /// </summary>
        public string DbId { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dbId"></param>
        public TenantAttribute(string dbId)
        {
            DbId = dbId;
        }
    }
}

