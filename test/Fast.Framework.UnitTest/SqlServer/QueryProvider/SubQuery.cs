using Fast.Framework.UnitTest.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.UnitTest.SqlServer.QueryProvider
{

    /// <summary>
    /// 子查询
    /// </summary>
    [TestClass]
    public class SubQuery : TestBase
    {
        public SubQuery()
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
        /// From子查询
        /// </summary>
        [TestMethod]
        public void FromSubQuery()
        {
            var subQuery = db.Query<Product>().Select(s => new
            {
                s.ProductId,
                s.CategoryId,
                s.ProductCode,
                s.ProductName,
                s.DeleteMark
            });
            var query = db.Query(subQuery);
            var sql = query.ToSqlString();
            Assert.AreEqual(sql, "SELECT * FROM ( SELECT [ProductId] AS [ProductId],[CategoryId] AS [CategoryId],[ProductCode] AS [ProductCode],[ProductName] AS [ProductName],[DeleteMark] AS [DeleteMark] FROM [Product] ) [p1]");
            var data = query.First();
        }

        /// <summary>
        /// 联表子查询
        /// </summary>
        [TestMethod]
        public void JoinSubQuery()
        {
            var subQuery = db.Query<Product>().Select(s => new
            {
                s.ProductId,
                s.CategoryId,
                s.ProductCode,
                s.ProductName,
                s.DeleteMark
            });
            var query = db.Query<Category>().InnerJoin(subQuery, (a, b) => a.CategoryId == b.CategoryId);
            var sql = query.ToSqlString();
            Assert.AreEqual(sql, "SELECT p1.[CategoryId],p1.[CategoryName] FROM [Category] [p1]\r\nINNER JOIN ( SELECT [ProductId] AS [ProductId],[CategoryId] AS [CategoryId],[ProductCode] AS [ProductCode],[ProductName] AS [ProductName],[DeleteMark] AS [DeleteMark] FROM [Product] ) [p2] ON ( [p1].[CategoryId] = [p2].[CategoryId] )");
            var data = query.First();
        }

        /// <summary>
        /// Select子查询
        /// </summary>
        [TestMethod]
        public void SelectSubQuery()
        {
            var query = db.Query<Product>().Select(s => new
            {
                CategoryName = db.Query<Category>().Where(w => w.CategoryId == 1).Select(s => s.CategoryName).First()
            });

            var sql = query.ToSqlString();
            Assert.AreEqual(sql, "SELECT ( SELECT TOP 1 [p2].[CategoryName] FROM [Category] [p2] \r\nWHERE ( [p2].[CategoryId] = 1 ) ) AS [CategoryName] FROM [Product] [p1]");
            var data = query.First();
        }

        /// <summary>
        /// Where子查询
        /// </summary>
        [TestMethod]
        public void WhereSubQuery()
        {
            var query = db.Query<Category>().Where(w => w.CategoryId == 1 && db.Query<Product>().Where(w => w.CategoryId == 1).Select(s => 1).Any());//Any支持取反

            var sql = query.ToSqlString();
            Assert.AreEqual(sql, "SELECT p1.[CategoryId],p1.[CategoryName] FROM [Category] [p1] \r\nWHERE ( ( [p1].[CategoryId] = 1 ) AND EXISTS ( SELECT 1 FROM [Product] [p2] \r\nWHERE ( [p2].[CategoryId] = 1 ) ) )");
            var data = query.First();
        }
    }
}
