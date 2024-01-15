using Fast.Framework.Test.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.UnitTest.MySql.ExpResolve
{

    /// <summary>
    /// 条件测试
    /// </summary>
    [TestClass]
    public class Where
    {

        /// <summary>
        /// 选项
        /// </summary>
        private readonly ResolveSqlOptions options = new ResolveSqlOptions() { DbType = DbType.MySQL, UseCustomParameter = false };


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
        /// In 测试
        /// </summary>
        [TestMethod]
        public void InTest()
        {
            IEnumerable<string> list1 = new List<string>() { "123", "456" };
            var list2 = new List<string>() { "123", "456" };
            var array1 = new string[2] { "123", "456" };

            Expression<Func<Product, bool>> ex = p => list1.Contains(p.Custom1)
            || list2.Contains(p.Custom2)
            || new List<string>() { "123", "456" }.Contains(p.Custom3)
            || array1.Contains(p.Custom4)
            || new string[2] { "123", "456" }.Contains(p.Custom5);

            var result = ex.ResolveSql(options);
            Console.WriteLine(result.SqlString);
            Assert.AreEqual(result.SqlString, "( ( ( ( `p`.`Custom1` IN ( @list1_0_1,@list1_1_2 ) OR `p`.`Custom2` IN ( @list2_0_3,@list2_1_4 ) ) OR `p`.`Custom3` IN ( '123','456' ) ) OR `p`.`Custom4` IN ( @array1_0_5,@array1_1_6 ) ) OR `p`.`Custom5` IN ( '123','456' ) )");
        }

        /// <summary>
        /// Not In 测试
        /// </summary>
        [TestMethod]
        public void NotInTest()
        {
            IEnumerable<string> list1 = new List<string>() { "123", "456" };
            var list2 = new List<string>() { "123", "456" };
            var array1 = new string[2] { "123", "456" };

            Expression<Func<Product, bool>> ex = p => !list1.Contains(p.Custom1)
            || !list2.Contains(p.Custom2)
            || !new List<string>() { "123", "456" }.Contains(p.Custom3)
            || !array1.Contains(p.Custom4)
            || !new string[2] { "123", "456" }.Contains(p.Custom5);

            var result = ex.ResolveSql(options);
            Console.WriteLine(result.SqlString);
            Assert.AreEqual(result.SqlString, "( ( ( ( `p`.`Custom1` NOT IN ( @list1_0_1,@list1_1_2 ) OR `p`.`Custom2` NOT IN ( @list2_0_3,@list2_1_4 ) ) OR `p`.`Custom3` NOT IN ( '123','456' ) ) OR `p`.`Custom4` NOT IN ( @array1_0_5,@array1_1_6 ) ) OR `p`.`Custom5` NOT IN ( '123','456' ) )");
        }

        /// <summary>
        /// CaseWhen
        /// </summary>
        [TestMethod]
        public void CaseWhen()
        {
            Expression<Func<Product, object>> ex = p => p.ProductId == 1 ? "测试1" : "测试2";
            var result = ex.ResolveSql(options);
            Console.WriteLine(result.SqlString);
            Assert.AreEqual(result.SqlString, "CASE WHEN ( `p`.`ProductId` = 1 ) THEN '测试1' ELSE '测试2' END");
        }
    }
}
