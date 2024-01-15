using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Fast.Framework
{

    /// <summary>
    /// 数据库参数
    /// </summary>
    public class FastParameter : DbParameter
    {

        /// <summary>
        /// 数据库类型
        /// </summary>
        public override System.Data.DbType DbType { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        public override ParameterDirection Direction { get; set; }

        /// <summary>
        /// 是否空
        /// </summary>
        public override bool IsNullable { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        public override string ParameterName { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        private int size;

        /// <summary>
        /// 大小
        /// </summary>
        public override int Size
        {
            get
            {
                if (Value != null)
                {
                    size = Value is byte[] byteArray ? byteArray.Length : Value is string stringValue ? stringValue.Length : 0;

                    if (size < 4000)
                    {
                        size = 4000;
                    }
                }
                if (size == 0)
                {
                    size = 4000;
                }
                return size;
            }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, message: null);
                }
                size = value;
            }
        }

        /// <summary>
        /// 源列
        /// </summary>
        public override string SourceColumn { get; set; }

        /// <summary>
        /// 源列空映射
        /// </summary>
        public override bool SourceColumnNullMapping { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public override object Value { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        public FastParameter(string name, object value)
        {
            this.ParameterName = name;
            this.Value = value;
            if (value != null)
            {
                this.DbType = value.GetType().GetDbType();
            }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="direction">参数方向</param>
        public FastParameter(string name, object value, ParameterDirection direction)
        {
            this.ParameterName = name;
            this.Value = value;
            this.Direction = direction;
            if (value != null)
            {
                this.DbType = value.GetType().GetDbType();
            }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="dbType">数据库类型</param>
        public FastParameter(string name, object value, System.Data.DbType dbType)
        {
            this.ParameterName = name;
            this.Value = value;
            this.DbType = dbType;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="direction">参数方向</param>
        public FastParameter(string name, object value, System.Data.DbType dbType, ParameterDirection direction)
        {
            this.ParameterName = name;
            this.Value = value;
            this.DbType = dbType;
            this.Direction = direction;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="direction">参数方向</param>
        /// <param name="size">大小</param>
        public FastParameter(string name, object value, System.Data.DbType dbType, ParameterDirection direction, int size)
        {
            this.ParameterName = name;
            this.Value = value;
            this.DbType = dbType;
            this.Direction = direction;
            this.Size = size;
        }

        /// <summary>
        /// 重置数据库类型
        /// </summary>
        public override void ResetDbType()
        {
            this.DbType = System.Data.DbType.String;
        }
    }
}

