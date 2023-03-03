using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Test.Models
{


    /// <summary>
    /// 类别
    /// </summary>
    public class Category
    {
        /// <summary>
        /// 类别ID
        /// </summary>
        [Key]
        public int CategoryId { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 产品
        /// </summary>
        [NotMapped]
        public List<Product> Products { get; set; }

    }
}
