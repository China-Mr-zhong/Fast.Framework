using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.UnitTest.Base;
using Fast.Framework.UnitTest.Data;

namespace Fast.Framework.UnitTest.Sqlite.DeleteProvider
{

    /// <summary>
    /// 删除
    /// </summary>
    [TestClass]
    public class Delete : TestBase
    {

        /// <summary>
        /// 构造方法
        /// </summary>
        public Delete()
        {
            db.ChangeDb("5");
            db.Aop.DbLog = (sql, dp) =>
            {
                Console.WriteLine(sql);
                Console.WriteLine();
                if (dp != null)
                {
                    foreach (var item in dp)
                    {
                        Console.WriteLine($"参数名称:{item.ParameterName} 参数值:{item.Value} 参数类型:{item.DbType}");
                    }
                }
            };
        }

        /// <summary>
        /// 实体对象删除
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void EntityDelete()
        {
            var result = db.Delete(DeleteDataSource.GetProduct()).Exceute();
            Console.WriteLine($"实体删除 受影响行数 {result}");
        }

        /// <summary>
        /// 实体对象列表删除
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void EntityListDelete()
        {
            var result = db.Delete(DeleteDataSource.GetProductList()).Exceute();
            Console.WriteLine($"实体删除 受影响行数 {result}");
        }

        /// <summary>
        /// 无条件删除
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void NotWhereDelete()
        {
            var result = db.Delete<Product>().Exceute();
            Console.WriteLine($"无条件删除 受影响行数 {result}");
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void WhereDelete()
        {
            var result = db.Delete<Product>().Where(w => w.ProductId == 1).Exceute();
            Console.WriteLine($"条件删除 受影响行数 {result}");
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void LogicDelete()
        {
            var result = db.Delete<Product>().Where(w => w.ProductId == 1).IsLogic().SetColumns(c => new Product()
            {
                ModifyTime = DateTime.Now
            }).Exceute();
            Console.WriteLine($"逻辑删除 受影响行数 {result}");
        }

        /// <summary>
        /// 逻辑删除列表
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void LogicDeleteList()
        {
            var result = db.Delete(DeleteDataSource.GetProductList()).IsLogic().SetColumns(c => new Product()
            {
                ModifyTime = DateTime.Now
            }).Exceute();
            Console.WriteLine($"逻辑删除列表 受影响行数 {result}");
        }
    }
}
