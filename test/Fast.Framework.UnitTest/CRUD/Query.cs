using Fast.Framework.Enum;
using Fast.Framework.Extensions;
using Fast.Framework.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 删除
    /// </summary>
    [TestClass]
    public class Query
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
        public Query()
        {
            dbOptions = JsonConfig.GetInstance().GetSection("DbOptions").Get<List<DbOptions>>();
            db = new DbContext(dbOptions);
            db.Aop.DbLog = (sql, dp) =>
            {
                Console.WriteLine(sql);
                Console.WriteLine();
            };
        }

        /// <summary>
        /// 第一
        /// </summary>
        [TestMethod]
        public void First()
        {
            var data = db.Query<Product>().First();
            Console.WriteLine("第一个结果");
        }

        /// <summary>
        /// 第一异步
        /// </summary>
        [TestMethod]
        public async Task FirstAsync()
        {
            var data = await db.Query<Product>().FirstAsync();
            Console.WriteLine("第一个结果异步");
        }

        /// <summary>
        /// 数组
        /// </summary>
        [TestMethod]
        public void ToArray()
        {
            var data = db.Query<Product>().ToArray();
            Console.WriteLine("数组");
        }

        /// <summary>
        /// 数组异步
        /// </summary>
        [TestMethod]
        public async Task ToArrayAsync()
        {
            var data = await db.Query<Product>().ToArrayAsync();
            Console.WriteLine("数组异步");
        }

        /// <summary>
        /// 列表
        /// </summary>
        [TestMethod]
        public void ToList()
        {
            var data = db.Query<Product>().ToList();
            Console.WriteLine("列表");
        }

        /// <summary>
        /// 列表异步
        /// </summary>
        [TestMethod]
        public async Task ToListAsync()
        {
            var data = await db.Query<Product>().ToListAsync();
            Console.WriteLine("列表异步");
        }

        /// <summary>
        /// 字典
        /// </summary>
        [TestMethod]
        public void ToDictionary()
        {
            var data = db.Query<Product>().ToDictionary();
            Console.WriteLine("字典");
        }

        /// <summary>
        /// 字典异步
        /// </summary>
        [TestMethod]
        public async Task ToDictionaryAsync()
        {
            var data = await db.Query<Product>().ToDictionaryAsync();
            Console.WriteLine("字典异步");
        }

        /// <summary>
        /// 字典列表
        /// </summary>
        [TestMethod]
        public void ToDictionaryList()
        {
            var data = db.Query<Product>().ToDictionaryList();
            Console.WriteLine("字典列表");
        }

        /// <summary>
        /// 字典列表异步
        /// </summary>
        [TestMethod]
        public async Task ToDictionaryListAsync()
        {
            var data = await db.Query<Product>().ToDictionaryListAsync();
            Console.WriteLine("字典列表异步");
        }

        /// <summary>
        /// 分页列表
        /// </summary>
        [TestMethod]
        public void ToPageList()
        {
            var data = db.Query<Product>().ToPageList(1, 100);
            Console.WriteLine("分页列表");
        }

        /// <summary>
        /// 分页列表异步
        /// </summary>
        [TestMethod]
        public async Task ToPageListAsync()
        {
            var data = await db.Query<Product>().ToPageListAsync(1, 100);
            Console.WriteLine("分页列表异步");
        }

        /// <summary>
        /// 对象Json
        /// </summary>
        [TestMethod]
        public void ObjToJson()
        {
            var data = db.Query<Product>().ObjToJson();
            Console.WriteLine("对象Json");
        }

        /// <summary>
        /// 对象Json异步
        /// </summary>
        [TestMethod]
        public async Task ObjToJsonAsync()
        {
            var data = await db.Query<Product>().ObjToJsonAsync();
            Console.WriteLine("对象Json异步");
        }

        /// <summary>
        /// 对象列表Json
        /// </summary>
        [TestMethod]
        public void ObjListToJson()
        {
            var data = db.Query<Product>().ObjListToJson();
            Console.WriteLine("对象列表Json");
        }

        /// <summary>
        /// 对象列表Json异步
        /// </summary>
        [TestMethod]
        public async Task ObjListToJsonAsync()
        {
            var data = await db.Query<Product>().ObjListToJsonAsync();
            Console.WriteLine("对象列表Json异步");
        }

        /// <summary>
        /// 数据表格
        /// </summary>
        [TestMethod]
        public void ToDataTable()
        {
            var data = db.Query<Product>().ToDataTable();
            Console.WriteLine("数据表格");
        }

        /// <summary>
        /// 数据表格异步
        /// </summary>
        [TestMethod]
        public async Task ToDataTableAsync()
        {
            var data = await db.Query<Product>().ToDataTableAsync();
            Console.WriteLine("数据表格异步");
        }
    }
}
