using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Fast.Framework
{

    /// <summary>
    /// 实体信息
    /// </summary>
    public class EntityInfo
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        public object TargetObj { get; set; }

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// 实体名称
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否匿名类型
        /// </summary>
        public bool IsAnonymousType { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 列信息
        /// </summary>
        public List<ColumnInfo> ColumnInfos { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public EntityInfo()
        {
            this.EntityName = "";
            this.TableName = "";
            this.Alias = "";
            this.ColumnInfos = new List<ColumnInfo>();
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public EntityInfo Clone()
        {
            var entityInfo = new EntityInfo();
            entityInfo.TargetObj = this.TargetObj;
            entityInfo.EntityType = this.EntityType;
            entityInfo.EntityName = this.EntityName;
            entityInfo.Description = this.Description;
            entityInfo.IsAnonymousType = this.IsAnonymousType;
            entityInfo.TenantId = this.TenantId;
            entityInfo.TableName = this.TableName;
            entityInfo.Alias = this.Alias;
            entityInfo.ColumnInfos.AddRange(ColumnInfos.Select(s => s.Clone()));
            return entityInfo;
        }
    }
}

