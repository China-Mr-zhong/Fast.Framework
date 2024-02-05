using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
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
        /// 是否字段
        /// </summary>
        public bool IsField { get; set; }

        /// <summary>
        /// 属性信息
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// 字段信息
        /// </summary>
        public FieldInfo FieldInfo { get; set; }

        /// <summary>
        /// 成员信息
        /// </summary>
        public MemberInfo MemberInfo { get; set; }

        /// <summary>
        /// 底层类型
        /// </summary>
        public Type UnderlyingType { get; set; }

        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 是否Json
        /// </summary>
        public bool IsJson { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// 数据库生成选项
        /// </summary>
        public DatabaseGeneratedOption DatabaseGeneratedOption { get; set; }

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
        /// 是否导航
        /// </summary>
        public bool IsNavigate { get; set; }

        /// <summary>
        /// 导航主名称
        /// </summary>
        public string NavMainName { get; set; }

        /// <summary>
        /// 导航子名称
        /// </summary>
        public string NavChildName { get; set; }

        /// <summary>
        /// 是否逻辑
        /// </summary>
        public bool IsLogic { get; set; }

        /// <summary>
        /// 数据库参数类型
        /// </summary>
        public System.Data.DbType? DbParameterType { get; set; }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public ColumnInfo Clone()
        {
            return new ColumnInfo()
            {
                GroupId = this.GroupId,
                IsField = this.IsField,
                PropertyInfo = this.PropertyInfo,
                FieldInfo = this.FieldInfo,
                MemberInfo = this.MemberInfo,
                UnderlyingType = this.UnderlyingType,
                ColumnName = this.ColumnName,
                TypeName = this.TypeName,
                IsJson = this.IsJson,
                Description = this.Description,
                ParameterName = this.ParameterName,
                DatabaseGeneratedOption = this.DatabaseGeneratedOption,
                IsPrimaryKey = this.IsPrimaryKey,
                IsNotMapped = this.IsNotMapped,
                IsNullable = this.IsNullable,
                IsWhere = this.IsWhere,
                IsVersion = this.IsVersion,
                IsNavigate = this.IsNavigate,
                IsLogic = this.IsLogic,
                DbParameterType = this.DbParameterType
            };
        }
    }
}
