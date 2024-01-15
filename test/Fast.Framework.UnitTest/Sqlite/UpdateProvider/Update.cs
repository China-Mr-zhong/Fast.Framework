using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Test.Models;
using Fast.Framework.UnitTest.Base;
using Fast.Framework.UnitTest.Data;

namespace Fast.Framework.UnitTest.Sqlite.UpdateProvider
{
    /// <summary>
    /// 更新
    /// </summary>
    [TestClass]
    public class Update : TestBase
    {

        /// <summary>
        /// 构造方法
        /// </summary>
        public Update()
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
        /// 实体更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void EntityUpdate()
        {
            var result = db.Update(UpdateDataSource.GetProduct()).Exceute();
            Console.WriteLine($"对象更新 受影响行数 {result}");
        }

        /// <summary>
        /// 实体列表更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void EntityListUpdate()
        {
            var result = db.Update(UpdateDataSource.GetProductList()).Exceute();
            Console.WriteLine($"对象列表更新 受影响行数 {result}");
        }

        /// <summary>
        /// 匿名对象更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void AnonymousObj()
        {
            var result = db.Update(UpdateDataSource.GetAnonymousObj()).As("Product").WhereColumns("ProductId").Exceute();
            Console.WriteLine($"匿名对象更新 受影响行数 {result}");
        }

        /// <summary>
        /// 匿名对象列表更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void AnonymousObjList()
        {
            var result = db.Update(UpdateDataSource.GetAnonymousObjList()).As("Product").WhereColumns("ProductId").Exceute();
            Console.WriteLine($"匿名对象列表更新 受影响行数 {result}");
        }

        /// <summary>
        /// 字典更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DictionaryUpdate()
        {
            var result = db.Update(UpdateDataSource.GetKeyValues()).As("Product").WhereColumns("ProductId").Exceute();
            Console.WriteLine($"字典更新 受影响行数 {result}");
        }

        /// <summary>
        /// 字典列表更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DictionaryListUpdate()
        {
            var result = db.Update(UpdateDataSource.GetKeyValuesList()).As("Product").WhereColumns("ProductId").Exceute();
            Console.WriteLine($"字典列表更新 受影响行数 {result}");
        }

        /// <summary>
        /// 条件更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void WhereUpdate()
        {
            var result = db.Update<Product>().SetColumns(c => new Product()
            {
                ProductCode = "1001",
                ProductName = "测试修改"
            }).Where(p => p.ProductId == 1).Exceute();
            Console.WriteLine($"条件更新 受影响行数 {result}");
        }

    }
}
