using System;
using System.Collections.Generic;
using System.Linq;

namespace Fast.Framework.Models
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
        /// 是否匿名类型
        /// </summary>
        public bool IsAnonymousType { get; set; }

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
        public List<ColumnInfo> ColumnsInfos { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public EntityInfo()
        {
            this.EntityName = "";
            this.TableName = "";
            this.Alias = "";
            this.ColumnsInfos = new List<ColumnInfo>();
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
            entityInfo.IsAnonymousType = this.IsAnonymousType;
            entityInfo.TableName = this.TableName;
            entityInfo.Alias = this.Alias;
            entityInfo.ColumnsInfos.AddRange(ColumnsInfos.Select(s => s.Clone()));
            return entityInfo;
        }
    }
}

