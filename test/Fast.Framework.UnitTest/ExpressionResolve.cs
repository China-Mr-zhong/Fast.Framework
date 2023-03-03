using Fast.Framework.Test.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.UnitTest
{

    /// <summary>
    /// 表达式解析
    /// </summary>
    [TestClass]
    public class ExpressionResolve
    {

        /// <summary>
        /// 选项
        /// </summary>
        private readonly ResolveSqlOptions options = new ResolveSqlOptions() { DbType = Enum.DbType.MySQL };


        /// <summary>
        /// 布尔测试1
        /// </summary>
        [TestMethod]
        public void BoolTest1()
        {
            Expression<Func<Product, bool>> ex = p => true;
            var result = ex.ResolveSql(options);
            Console.WriteLine(result.SqlString);
            Assert.AreEqual(result.SqlString, "1 = 1");
        }

        /// <summary>
        /// 布尔测试2
        /// </summary>
        [TestMethod]
        public void BoolTest2()
        {
            Expression<Func<Product, bool>> ex = p => true || p.DeleteMark || !p.DeleteMark || p.DeleteMark == true || p.DeleteMark == false || false;
            var result = ex.ResolveSql(options);
            Console.WriteLine(result.SqlString);
            Assert.AreEqual(result.SqlString, "( ( ( ( ( 1 = 1 OR `p`.`DeleteMark` = 1 ) OR `p`.`DeleteMark` = 0 ) OR ( `p`.`DeleteMark` = 1 ) ) OR ( `p`.`DeleteMark` = 0 ) ) OR 0 = 1 )");
        }

        /// <summary>
        /// 布尔测试3
        /// </summary>
        [TestMethod]
        public void BoolTest3()
        {
            Expression<Func<Product, bool>> ex = p => p.DeleteMark;
            var result = ex.ResolveSql(options);
            Console.WriteLine(result.SqlString);
            Assert.AreEqual(result.SqlString, "`p`.`DeleteMark` = 1");
        }

        /// <summary>
        /// 布尔测试4
        /// </summary>
        [TestMethod]
        public void BoolTest4()
        {
            Expression<Func<Product, bool>> ex = p => !p.DeleteMark;
            var result = ex.ResolveSql(options);
            Console.WriteLine(result.SqlString);
            Assert.AreEqual(result.SqlString, "`p`.`DeleteMark` = 0");
        }

        /// <summary>
        /// 条件测试
        /// </summary>
        [TestMethod]
        public void Conditional1()
        {
            Expression<Func<Product, object>> ex = p => p.ProductId == 1 ? "测试1" : "测试2";
            var result = ex.ResolveSql(options);
            Console.WriteLine(result.SqlString);
            Assert.AreEqual(result.SqlString, "CASE WHEN ( `p`.`ProductId` = 1 ) THEN '测试1' ELSE '测试2' END");
        }
    }
}
