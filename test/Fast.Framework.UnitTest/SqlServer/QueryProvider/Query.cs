using Fast.Framework.UnitTest.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.UnitTest.SqlServer.QueryProvider
{

    /// <summary>
    /// 查询
    /// </summary>
    [TestClass]
    public class Query : TestBase
    {

        /// <summary>
        /// 构造方法
        /// </summary>
        public Query()
        {
            db.ChangeDb("1");
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
        /// 第一
        /// </summary>
        [TestMethod]
        public void First()
        {
            var data = db.Query<Product>().First();
        }

        /// <summary>
        /// 数组
        /// </summary>
        [TestMethod]
        public void ToArray()
        {
            var data = db.Query<Product>().ToArray();
        }

        /// <summary>
        /// 列表
        /// </summary>
        [TestMethod]
        public void ToList()
        {
            var data = db.Query<Product>().ToList();
        }

        /// <summary>
        /// 字典
        /// </summary>
        [TestMethod]
        public void ToDictionary()
        {
            var data = db.Query<Product>().ToDictionary();
        }

        /// <summary>
        /// 字典列表
        /// </summary>
        [TestMethod]
        public void ToDictionaryList()
        {
            var data = db.Query<Product>().ToDictionaryList();
        }

        /// <summary>
        /// 分页列表
        /// </summary>
        [TestMethod]
        public void ToPageList()
        {
            var data = db.Query<Product>().ToPageList(1, 100);
        }

        /// <summary>
        /// 数据表格
        /// </summary>
        [TestMethod]
        public void ToDataTable()
        {
            var data = db.Query<Product>().ToDataTable();
        }
    }
}
