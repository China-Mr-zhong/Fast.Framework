using Fast.Framework.UnitTest.Base;
using Fast.Framework.UnitTest.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fast.Framework.UnitTest.MySql.InsertProvider
{

    /// <summary>
    /// 插入
    /// </summary>
    [TestClass]
    public class Insert : TestBase
    {

        /// <summary>
        /// 构造方法
        /// </summary>
        public Insert()
        {
            db.ChangeDb("2");
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
        /// 实体对象
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void Entity()
        {
            var result = db.Insert(InsertDataSource.GetProduct()).Exceute();
            Console.WriteLine($"实体对象插入 受影响行数 {result}");
            Assert.IsTrue(result == 1);
        }

        /// <summary>
        /// 实体对象列表
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void EntityList()
        {
            var result = db.Insert(InsertDataSource.GetProductList()).Exceute();
            Console.WriteLine($"实体对象列表插入 受影响行数  {result}");
            Assert.IsTrue(result == 100);
        }

        /// <summary>
        /// 返回自增ID
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void ReturnIdentity()
        {
            var result = db.Insert(InsertDataSource.GetProduct()).ExceuteReturnIdentity();
            Console.WriteLine($"实体对象插入 返回自增ID {result}");
            Assert.IsTrue(result > 0);
        }

        /// <summary>
        /// 返回计算ID
        /// </summary>
        [TestMethod]
        public void ReturnComputedId()
        {
            var result = db.Insert(InsertDataSource.GetProduct()).ExceuteReturnComputedId();
            Console.WriteLine($"实体对象插入 返回计算ID {result}");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(Convert.ToString(result)));
        }

        /// <summary>
        /// 返回计算ID列表
        /// </summary>
        [TestMethod]
        public void ReturnComputedIdList()
        {
            var result = db.Insert(InsertDataSource.GetProductList()).ExceuteReturnComputedIds();
            Console.WriteLine($"实体对象插入 返回计算ID总数 {result.Count}");
            Assert.IsTrue(result.Count == 100);
        }

        /// <summary>
        /// 匿名对象
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void AnonymousObj()
        {
            //注意:需要使用As方法显示指定表名称
            var result = db.Insert(InsertDataSource.GetAnonymousObj()).IgnoreColumns("ProductId").As("Product").Exceute();
            Console.WriteLine($"匿名对象插入 受影响行数 {result}");
            Assert.IsTrue(result == 1);
        }

        /// <summary>
        /// 匿名对象列表
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void AnonymousObjList()
        {
            //注意:需要使用As方法显示指定表名称
            var result = db.Insert(InsertDataSource.GetAnonymousObjList()).IgnoreColumns("ProductId").As("Product").Exceute();
            Console.WriteLine($"匿名对象列表插入 受影响行数 {result}");
            Assert.IsTrue(result == 100);
        }

        /// <summary>
        /// 字典
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void Dictionary()
        {
            //注意:需要使用As方法显示指定表名称
            var result = db.Insert(InsertDataSource.GetKeyValues()).IgnoreColumns("ProductId").As("Product").Exceute();
            Console.WriteLine($"字典插入 受影响行数 {result}");
            Assert.IsTrue(result == 1);
        }

        /// <summary>
        /// 字典列表
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DictionaryList()
        {
            //注意:需要使用As方法显示指定表名称
            var result = db.Insert(InsertDataSource.GetKeyValuesList()).IgnoreColumns("ProductId").As("Product").Exceute();
            Console.WriteLine($"字典列表插入 受影响行数 {result}");
            Assert.IsTrue(result == 100);
        }

        /// <summary>
        /// 批量复制
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void BulkCopy()
        {
            var result = db.Fast<Product>().BulkCopy(InsertDataSource.GetProductList(5000));
            Console.WriteLine($"批量复制 受影响行数 {result}");
            Assert.IsTrue(result == 5000);
        }
    }
}