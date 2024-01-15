using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fast.Framework.Test.Models
{

    /// <summary>
    /// 产品
    /// </summary>
    public class Product : IEntityCache
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        /// <summary>
        /// 类别ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ProductCode { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        [Logic]
        public bool DeleteMark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 自定义1
        /// </summary>
        public string Custom1 { get; set; }

        /// <summary>
        /// 自定义2
        /// </summary>
        public string Custom2 { get; set; }

        /// <summary>
        /// 自定义3
        /// </summary>
        public string Custom3 { get; set; }

        /// <summary>
        /// 自定义4
        /// </summary>
        public string Custom4 { get; set; }

        /// <summary>
        /// 自定义5
        /// </summary>
        public string Custom5 { get; set; }

        /// <summary>
        /// 自定义6
        /// </summary>
        public string Custom6 { get; set; }

        /// <summary>
        /// 自定义7
        /// </summary>
        public string Custom7 { get; set; }

        /// <summary>
        /// 自定义8
        /// </summary>
        public string Custom8 { get; set; }

        /// <summary>
        /// 自定义9
        /// </summary>
        public string Custom9 { get; set; }

        /// <summary>
        /// 自定义10
        /// </summary>
        public string Custom10 { get; set; }

        /// <summary>
        /// 自定义11
        /// </summary>
        public string Custom11 { get; set; }

        /// <summary>
        /// 自定义12
        /// </summary>
        [OptLock]
        public string Custom12 { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        [Navigate(MainName = "CategoryId", ChildName = "CategoryId")]
        public Category Category { get; set; }
    }
}

