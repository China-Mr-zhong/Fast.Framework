using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Enum;
using Fast.Framework.Implements;

namespace Fast.Framework
{

    /// <summary>
    /// 删除
    /// </summary>
    [TestClass]
    public class Delete
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly IDbContext db;

        /// <summary>
        /// 数据库选项
        /// </summary>
        private readonly List<DbOptions> dbOptions;

        /// <summary>
        /// 构造方法
        /// </summary>
        public Delete()
        {
            dbOptions = JsonConfig.GetInstance().GetSection("DbOptions").Get<List<DbOptions>>();
            db = new DbContext(dbOptions);
        }

        /// <summary>
        /// 实体对象删除
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void EntityDelete()
        {
            var obj = new Product()
            {
                ProductId = 1,
                ProductCode = "1001",
                ProductName = "测试商品1"
            };
            var result = db.Delete(obj).Exceute();
            Console.WriteLine($"实体删除 受影响行数 {result}");
        }

        /// <summary>
        /// 实体对象删除异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task EntityDeleteAsync()
        {
            var obj = new Product()
            {
                ProductId = 1,
                ProductCode = "1001",
                ProductName = "测试商品1"
            };
            var result = await db.Delete(obj).ExceuteAsync();
            Console.WriteLine($"实体删除异步 受影响行数 {result}");
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
        /// 无条件删除异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task NotWhereDeleteAsync()
        {
            var result = await db.Delete<Product>().ExceuteAsync();
            Console.WriteLine($"无条件删除异步 受影响行数 {result}");
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
        /// 条件删除异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task WhereDeleteAsync()
        {
            var result = await db.Delete<Product>().Where(w => w.ProductId == 1).ExceuteAsync();
            Console.WriteLine($"条件删除异步 受影响行数 {result}");
        }

        /// <summary>
        /// Object删除
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void ObjDelete()
        {
            var result = db.Delete<object>().As("Product").WhereColumn("ProductId", 1001).Exceute();
            Console.WriteLine($"无实体删除 受影响行数 {result}");
        }

        /// <summary>
        /// Object删除异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ObjDeleteAsync()
        {
            var result = await db.Delete<object>().As("Product").WhereColumn("ProductId", 1001).ExceuteAsync();
            Console.WriteLine($"无实体删除异步 受影响行数 {result}");
        }

    }
}
