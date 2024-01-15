using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.UnitTest.Data
{

    /// <summary>
    /// 删除数据源
    /// </summary>
    public class DeleteDataSource
    {
        /// <summary>
        /// 获取匿名对象
        /// </summary>
        /// <returns></returns>
        public static object GetAnonymousObj()
        {
            var obj = new
            {
                ProductId = 1,
                CategoryId = 1,
                ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}",
                ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}",
                CreateTime = DateTime.Now,
                Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}",
                Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}",
                Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}",
                Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}",
                Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}",
                Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}",
                Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}",
                Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}",
                Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}",
                Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}",
                Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}",
                Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}",
            };

            return obj;
        }

        /// <summary>
        /// 获取匿名对象列表
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<object> GetAnonymousObjList(int count = 100)
        {
            var list = new List<object>();
            for (int i = 1; i <= count; i++)
            {
                list.Add(new
                {
                    ProductId = i,
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
            return list;
        }

        /// <summary>
        /// 获取产品
        /// </summary>
        /// <returns></returns>
        public static Product GetProduct()
        {
            var obj = new Product()
            {
                ProductId = 1,
                CategoryId = 1,
                ProductCode = $"测试编号_{Timestamp.CurrentTimestampSeconds()}",
                ProductName = $"测试名称_{Timestamp.CurrentTimestampSeconds()}",
                CreateTime = DateTime.Now,
                Custom1 = $"测试自定义1_{Timestamp.CurrentTimestampSeconds()}",
                Custom2 = $"测试自定义2_{Timestamp.CurrentTimestampSeconds()}",
                Custom3 = $"测试自定义3_{Timestamp.CurrentTimestampSeconds()}",
                Custom4 = $"测试自定义4_{Timestamp.CurrentTimestampSeconds()}",
                Custom5 = $"测试自定义5_{Timestamp.CurrentTimestampSeconds()}",
                Custom6 = $"测试自定义6_{Timestamp.CurrentTimestampSeconds()}",
                Custom7 = $"测试自定义7_{Timestamp.CurrentTimestampSeconds()}",
                Custom8 = $"测试自定义8_{Timestamp.CurrentTimestampSeconds()}",
                Custom9 = $"测试自定义9_{Timestamp.CurrentTimestampSeconds()}",
                Custom10 = $"测试自定义10_{Timestamp.CurrentTimestampSeconds()}",
                Custom11 = $"测试自定义11_{Timestamp.CurrentTimestampSeconds()}",
                Custom12 = $"测试自定义12_{Timestamp.CurrentTimestampSeconds()}",
            };

            return obj;
        }

        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<Product> GetProductList(int count = 100)
        {
            var list = new List<Product>();
            for (int i = 1; i <= count; i++)
            {
                list.Add(new Product()
                {
                    ProductId = i,
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
            return list;
        }

        /// <summary>
        /// 获取键值对
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object> GetKeyValues()
        {
            var keyValues = new Dictionary<string, object>()
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
                {"Custom9",$"测试自定义9_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom10",$"测试自定义10_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom11",$"测试自定义11_{Timestamp.CurrentTimestampSeconds()}_{6}"},
                {"Custom12",$"测试自定义12_{Timestamp.CurrentTimestampSeconds()}_{6}"},
            };
            return keyValues;
        }

        /// <summary>
        /// 获取键值对列表
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> GetKeyValuesList(int count = 100)
        {
            var list = new List<Dictionary<string, object>>();
            for (int i = 1; i <= count; i++)
            {
                list.Add(new Dictionary<string, object>()
                {
                    {"ProductId",i },
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
            return list;
        }
    }
}
