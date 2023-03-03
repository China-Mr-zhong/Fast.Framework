using Fast.Framework.CustomAttribute;
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
    [Table("Product")]
    public class ProductPlus
    {
        public int ProductId { get; set; }

        public string MyProperty { get; set; }
    }
}

