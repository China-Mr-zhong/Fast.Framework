using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Models
{

    /// <summary>
    /// 列信息
    /// </summary>
    public class ColumnInfo
    {
        /// <summary>
        /// 分组ID
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// 属性信息
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// 数据库生成选项
        /// </summary>
        public DatabaseGeneratedOption DatabaseGeneratedOption { get; set; }

        /// <summary>
        /// 计算值
        /// </summary>
        public object ComputedValue { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否不映射
        /// </summary>
        public bool IsNotMapped { get; set; }

        /// <summary>
        /// 是否可空类型
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// 是否条件
        /// </summary>
        public bool IsWhere { get; set; }

        /// <summary>
        /// 是否版本
        /// </summary>
        public bool IsVersion { get; set; }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public ColumnInfo Clone()
        {
            return new ColumnInfo()
            {
                GroupId = this.GroupId,
                PropertyInfo = this.PropertyInfo,
                ColumnName = this.ColumnName,
                ParameterName = this.ParameterName,
                DatabaseGeneratedOption = this.DatabaseGeneratedOption,
                ComputedValue = this.ComputedValue,
                IsPrimaryKey = this.IsPrimaryKey,
                IsNotMapped = this.IsNotMapped,
                IsNullable = this.IsNullable,
                IsWhere = this.IsWhere,
                IsVersion = this.IsVersion
            };
        }
    }
}
