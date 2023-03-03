using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Framework.Enum;
using Fast.Framework.Implements;
using Fast.Framework.Test.Models;

namespace Fast.Framework
{
    /// <summary>
    /// 删除
    /// </summary>
    [TestClass]
    public class Update
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
        public Update()
        {
            dbOptions = JsonConfig.GetInstance().GetSection("DbOptions").Get<List<DbOptions>>();
            db = new DbContext(dbOptions);
        }

        /// <summary>
        /// 实体更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void EntityUpdate()
        {
            var obj = new Product()
            {
                ProductId = 1,
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
            var result = db.Update(obj).Exceute();
            Console.WriteLine($"对象更新 受影响行数 {result}");
        }

        /// <summary>
        /// 实体更新异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task EntityUpdateAsync()
        {
            var obj = new Product()
            {
                ProductId = 1,
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
                ModifyTime = DateTime.Now,
            };
            var result = await db.Update(obj).ExceuteAsync();
            Console.WriteLine($"对象更新异步 受影响行数 {result}");
        }

        /// <summary>
        /// 实体列表更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void EntityListUpdate()
        {
            var list = new List<Product>();
            for (int i = 0; i < 1000; i++)
            {
                list.Add(new Product()
                {
                    ProductId = 1,
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
                    ModifyTime = DateTime.Now,
                });
            }
            var result = db.Update(list).Exceute();
            Console.WriteLine($"对象列表更新 受影响行数 {result}");
        }

        /// <summary>
        /// 实体列表更新异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task EntityListUpdateAsync()
        {
            var list = new List<Product>();
            for (int i = 0; i < 1000; i++)
            {
                list.Add(new Product()
                {
                    ProductId = 1,
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
                    ModifyTime = DateTime.Now,
                });
            }
            var result = await db.Update(list).ExceuteAsync();
            Console.WriteLine($"对象列表更新异步 受影响行数 {result}");
        }

        /// <summary>
        /// 匿名对象更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void AnonymousObj()
        {
            var obj = new
            {
                ProductId = 1,
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
            var result = db.Update(obj).As("Product").WhereColumns("ProductId").Exceute();
            Console.WriteLine($"匿名对象更新 受影响行数 {result}");
        }

        /// <summary>
        /// 匿名对象更新异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AnonymousObjAsync()
        {
            var obj = new
            {
                ProductId = 1,
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
            var result = await db.Update(obj).As("Product").WhereColumns("ProductId").ExceuteAsync();
            Console.WriteLine($"匿名对象更新异步 受影响行数 {result}");
        }

        /// <summary>
        /// 匿名对象列表更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void AnonymousObjList()
        {
            var list = new List<object>();
            for (int i = 0; i < 100; i++)
            {
                list.Add(new
                {
                    ProductId = 1,
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
            var result = db.Update(list).As("Product").WhereColumns("ProductId").Exceute();
            Console.WriteLine($"匿名对象列表更新 受影响行数 {result}");
        }

        /// <summary>
        /// 匿名对象列表更新异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AnonymousObjListAsync()
        {
            var list = new List<object>();
            for (int i = 0; i < 1000; i++)
            {
                list.Add(new
                {
                    ProductId = 1,
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
            var result = await db.Update(list).As("Product").WhereColumns("ProductId").ExceuteAsync();
            Console.WriteLine($"匿名对象列表更新异步 受影响行数 {result}");
        }

        /// <summary>
        /// 字典更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DictionaryUpdate()
        {
            var obj = new Dictionary<string, object>()
            {
                {"ProductId",1 },
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
            var result = db.Update(obj).As("Product").WhereColumns("ProductId").Exceute();
            Console.WriteLine($"字典更新 受影响行数 {result}");
        }

        /// <summary>
        /// 字典更新异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DictionaryUpdateAsync()
        {
            var obj = new Dictionary<string, object>()
            {
                {"ProductId",1 },
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
            var result = await db.Update(obj).As("Product").WhereColumns("ProductId").ExceuteAsync();
            Console.WriteLine($"字典更新异步 受影响行数 {result}");
        }

        /// <summary>
        /// 字典列表更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DictionaryListUpdate()
        {
            var list = new List<Dictionary<string, object>>();
            for (int i = 0; i < 2022; i++)
            {
                list.Add(new Dictionary<string, object>()
                {
                    { "ProductId",i},
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
            var result = db.Update(list).As("Product").WhereColumns("ProductId").Exceute();
            Console.WriteLine($"字典列表更新 受影响行数 {result}");
        }

        /// <summary>
        /// 字典列表更新异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DictionaryListUpdateAsync()
        {
            var list = new List<Dictionary<string, object>>();
            for (int i = 0; i < 2022; i++)
            {
                list.Add(new Dictionary<string, object>()
                {
                    { "ProductId",i+1},
                    {"ProductCode",$"更新编号:{i+1}"},
                    { "ProductName",$"更新商品:{i + 1}"}
                });
            }
            var result = await db.Update(list).As("Product").WhereColumns("ProductId").ExceuteAsync();
            Console.WriteLine($"字典列表更新异步 受影响行数 {result}");
        }

        /// <summary>
        /// 表达式更新
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void LambdaUpdate()
        {
            var obj = new Product()
            {
                ProductId = 1,
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
            var result = db.Update(obj).Where(p => p.ProductId == 100).Exceute();
            Console.WriteLine($"表达式更新 受影响行数 {result}");
        }

        /// <summary>
        /// 表达式更新异步
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task LambdaUpdateAsync()
        {
            var obj = new Product()
            {
                ProductId = 1,
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
            var result = await db.Update(obj).Where(p => p.ProductId == 100).ExceuteAsync();
            Console.WriteLine($"表达式更新异步 受影响行数 {result}");
        }


    }
}
