using System;


namespace Fast.Framework
{

    /// <summary>
    /// 租户属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TenantAttribute : Attribute
    {

        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        public TenantAttribute(string tenantId)
        {
            TenantId = tenantId;
        }
    }
}

