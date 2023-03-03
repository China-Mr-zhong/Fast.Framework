using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Diagnostics;
using Fast.Framework.Extensions;
using Fast.Framework.Implements;
using Fast.Framework.Interfaces;
using Fast.Framework.Models;
using Fast.Framework.Utils;
using System.Runtime.ExceptionServices;
using System.IO;
using System.Text;
using System.Net.Http;
using Fast.Framework;
using Fast.Framework.Test.Models;
using System.Net.WebSockets;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
using Fast.Framework.Enum;
using System.Collections.Concurrent;
using Fast.Framework.Snowflake;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Fast.Framework.Test
{
    public class Program
    {

        static void Main(string[] args)
        {
            try
            {
                IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();

                var dbOptions = configuration.GetSection("DbOptions").Get<List<DbOptions>>();
                var db = new DbContext(dbOptions);

                #region 插入模拟数据
                //{
                //    var random = new Random();
                //    var dataTotal = 1000000;
                //    var list = new List<Product>();
                //    for (int j = 1; j <= dataTotal; j++)
                //    {
                //        list.Add(new Product()
                //        {
                //            CategoryId = random.Next(1, 10),
                //            ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //            ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //            CreateTime = DateTime.Now,
                //            Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //            Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //            Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //            Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //            Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //            Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //            Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //            Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //            Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //            Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //            Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //            Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //        });
                //    }
                //    Console.WriteLine("模拟数据构造完成.");
                //    db.BulkCopy().Copy(list);
                //    Console.WriteLine("批量复制数据完成.");
                //}
                #endregion

                //db.Aop.DbLog = (sql, dp) =>
                //{
                //    Console.WriteLine(sql);
                //    Console.WriteLine();
                //    if (dp != null)
                //    {
                //        foreach (var item in dp)
                //        {
                //            Console.WriteLine($"参数名称:{item.ParameterName} 参数值:{item.Value}");
                //        }
                //    }
                //};

                //db.ChangeDb("3");

                //var list = new List<Product>();
                //for (int j = 1; j <= 10; j++)
                //{
                //    list.Add(new Product()
                //    {
                //        ProductId = j,
                //        CategoryId = 1,
                //        ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //        ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //        CreateTime = DateTime.Now,
                //        Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //        Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //        Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //        Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //        Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //        Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //        Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //        Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //        Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //        Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //        Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //        Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{j}",
                //    });
                //}

                //var result = db.Update(list).Exceute();
                //Console.WriteLine($"受影响行:{result}");

                //var obj = new Product()
                //{
                //    CategoryId = 1,
                //    ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}_{6}",
                //    ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}_{6}",
                //    CreateTime = DateTime.Now,
                //    Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}_{6}",
                //    Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}_{6}",
                //    Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}_{6}",
                //    Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}_{6}",
                //    Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}_{6}",
                //    Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}_{6}",
                //    Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}_{6}",
                //    Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}_{6}",
                //    Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{6}",
                //    Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{6}",
                //    Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{6}",
                //    Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{6}",
                //};

                //var stopwatch = new Stopwatch();
                //stopwatch.Start();
                //var result = db.Insert(obj).Exceute();
                //stopwatch.Stop();
                //Console.WriteLine($"插入实体 受影响行数:{result} 执行耗时:{stopwatch.ElapsedMilliseconds}ms {stopwatch.ElapsedMilliseconds / 1000.00}s");


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        }
    }
}