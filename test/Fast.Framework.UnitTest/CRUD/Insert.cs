

using Fast.Framework.Enum;
using Fast.Framework.Implements;
using Fast.Framework.Test.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// 插入
    /// </summary>
    [TestClass]
    public class Insert
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
        public Insert()
        {
            dbOptions = JsonConfig.GetInstance().GetSection("DbOptions").Get<List<DbOptions>>();
            db = new DbContext(dbOptions);
        }

        /// <summary>
        /// 实体对象
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void Entity()
        {
            var obj = new Product()
            {
                CategoryId = 1,
                ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{6}",
                ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{6}",
                CreateTime = DateTime.Now,
                Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{6}",
            };
            var result = db.Insert(obj).Exceute();
            Console.WriteLine($"实体对象插入 受影响行数 {result}");
            Assert.IsTrue(result == 1);
        }

        /// <summary>
        /// 实体对象异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task EntityAsync()
        {
            var obj = new Product()
            {
                CategoryId = 1,
                ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{6}",
                ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{6}",
                CreateTime = DateTime.Now,
                Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{6}",
            };
            var result = await db.Insert(obj).ExceuteAsync();
            Console.WriteLine($"实体对象插入异步 受影响行数 {result}");
            Assert.IsTrue(result == 1);
        }

        /// <summary>
        /// 返回自增ID
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void ReturnIdentity()
        {
            var obj = new Product()
            {
                CategoryId = 1,
                ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{6}",
                ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{6}",
                CreateTime = DateTime.Now,
                Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{6}",
            };
            var result = db.Insert(obj).ExceuteReturnIdentity();
            Console.WriteLine($"实体对象插入 返回自增ID {result}");
            Assert.IsTrue(result >= 0);
        }

        /// <summary>
        /// 返回自增ID异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ReturnIdentityAsync()
        {
            var obj = new Product()
            {
                CategoryId = 1,
                ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{6}",
                ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{6}",
                CreateTime = DateTime.Now,
                Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{6}",
            };
            var result = await db.Insert(obj).ExceuteReturnIdentityAsync();
            Console.WriteLine($"实体对象插入异步 返回自增ID {result}");
            Assert.IsTrue(result >= 0);
        }

        /// <summary>
        /// 实体对象列表
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void EntityList()
        {
            var list = new List<Product>();
            for (int i = 1; i <= 2100; i++)
            {
                list.Add(new Product()
                {
                    CategoryId = 1,
                    ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    CreateTime = DateTime.Now,
                    Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{i}",
                });
            }
            var result = db.Insert(list).Exceute();
            Console.WriteLine($"实体对象列表插入 受影响行数  {result}");
            Assert.IsTrue(result == 2100);
        }

        /// <summary>
        /// 实体对象列表异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task EntityListAsync()
        {
            var list = new List<Product>();
            for (int i = 1; i <= 2100; i++)
            {
                list.Add(new Product()
                {
                    CategoryId = 1,
                    ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    CreateTime = DateTime.Now,
                    Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{i}",
                });
            }
            var result = await db.Insert(list).ExceuteAsync();
            Console.WriteLine($"实体对象列表插入异步 受影响行数  {result}");
            Assert.IsTrue(result == 2100);
        }

        /// <summary>
        /// 匿名对象
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void AnonymousObj()
        {
            var obj = new 
            {
                CategoryId = 1,
                ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{6}",
                ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{6}",
                CreateTime = DateTime.Now,
                Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{6}",
            };
            //注意:需要使用As方法显示指定表名称
            var result = db.Insert(obj).As("Product").Exceute();
            Console.WriteLine($"匿名对象插入 受影响行数 {result}");
            Assert.IsTrue(result == 1);
        }

        /// <summary>
        /// 匿名对象异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AnonymousObjAsync()
        {
            var obj = new 
            {
                CategoryId = 1,
                ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{6}",
                ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{6}",
                CreateTime = DateTime.Now,
                Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{6}",
                Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{6}",
            };
            //注意:需要使用As方法显示指定表名称
            var result = await db.Insert(obj).As("Product").ExceuteAsync();
            Console.WriteLine($"匿名对象插入异步 受影响行数 {result}");
            Assert.IsTrue(result == 1);
        }

        /// <summary>
        /// 匿名对象列表
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void AnonymousObjList()
        {
            var list = new List<object>();
            for (int i = 1; i <= 2100; i++)
            {
                list.Add(new
                {
                    CategoryId = 1,
                    ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    CreateTime = DateTime.Now,
                    Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{i}",
                });
            }
            //注意:需要使用As方法显示指定表名称
            var result = db.Insert<dynamic>(list).As("Product").Exceute();
            Console.WriteLine($"匿名对象列表插入 受影响行数 {result}");
            Assert.IsTrue(result == 2100);
        }

        /// <summary>
        /// 匿名对象列表异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AnonymousObjListAsync()
        {
            var list = new List<object>();
            for (int i = 1; i <= 2100; i++)
            {
                list.Add(new
                {
                    CategoryId = 1,
                    ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    CreateTime = DateTime.Now,
                    Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{i}",
                    Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{i}",
                });
            }
            //注意:需要使用As方法显示指定表名称
            var result = await db.Insert<dynamic>(list).As("Product").ExceuteAsync();
            Console.WriteLine($"匿名对象列表插入异步 受影响行数 {result}");
            Assert.IsTrue(result == 2100);
        }

        /// <summary>
        /// 字典
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void Dictionary()
        {
            var obj = new Dictionary<string, object>()
            {
                {"CategoryId",1 },
                {"ProductCode", $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"ProductName", $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{6}" },
                {"CreateTime", DateTime.Now },
                {"Custom1",$"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom2",$"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom3",$"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom4",$"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom5",$"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom6",$"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom7",$"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom8",$"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                { "Custom9",$"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom10",$"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom11",$"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom12",$"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{6}"},
            };
            //注意:需要显示指定泛型类型
            var result = db.Insert(obj).As("Product").Exceute();
            Console.WriteLine($"字典插入 受影响行数 {result}");
            Assert.IsTrue(result == 1);
        }

        /// <summary>
        /// 字典异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DictionaryAsync()
        {
            var obj = new Dictionary<string, object>()
            {
                {"CategoryId",1 },
                {"ProductCode", $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"ProductName", $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{6}" },
                {"CreateTime", DateTime.Now },
                {"Custom1",$"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom2",$"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom3",$"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom4",$"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom5",$"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom6",$"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom7",$"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom8",$"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                { "Custom9",$"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom10",$"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom11",$"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom12",$"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{6}"},
            };
            //注意:需要显示指定泛型类型
            var result = await db.Insert(obj).As("Product").ExceuteAsync();
            Console.WriteLine($"字典插入异步 受影响行数 {result}");
            Assert.IsTrue(result == 1);
        }

        /// <summary>
        /// 字典列表
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DictionaryList()
        {
            var list = new List<Dictionary<string, object>>();
            for (int i = 0; i < 2100; i++)
            {
                list.Add(new Dictionary<string, object>()
                {
                    {"CategoryId",1 },
                    {"ProductCode", $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"ProductName", $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{i}" },
                    {"CreateTime", DateTime.Now },
                    {"Custom1",$"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom2",$"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom3",$"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom4",$"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom5",$"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom6",$"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom7",$"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom8",$"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    { "Custom9",$"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom10",$"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom11",$"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom12",$"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                 });
            }
            //注意:需要显示指定泛型类型
            var result = db.Insert(list).As("Product").Exceute();
            Console.WriteLine($"字典列表插入 受影响行数 {result}");
            Assert.IsTrue(result == 2100);
        }

        /// <summary>
        /// 字典列表异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DictionaryListAsync()
        {
            var list = new List<Dictionary<string, object>>();
            for (int i = 0; i < 2100; i++)
            {
                list.Add(new Dictionary<string, object>()
                {
                    {"CategoryId",1 },
                    {"ProductCode", $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"ProductName", $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{i}" },
                    {"CreateTime", DateTime.Now },
                    {"Custom1",$"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom2",$"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom3",$"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom4",$"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom5",$"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom6",$"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom7",$"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom8",$"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    { "Custom9",$"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom10",$"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom11",$"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                    {"Custom12",$"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{i}"},
                 });
            }
            //注意:需要显示指定泛型类型
            var result = await db.Insert(list).As("Product").ExceuteAsync();
            Console.WriteLine($"字典列表插入异步 受影响行数 {result}");
            Assert.IsTrue(result == 2100);
        }


    }
}