using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.UnitTest.Base
{

    /// <summary>
    /// 测试基类
    /// </summary>
    public class TestBase
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        public readonly IDbContext db;

        public TestBase()
        {
            var dbOptions = JsonConfig.GetInstance().GetSection(nameof(DbOptions)).Get<List<DbOptions>>();
            db = new DbContext(dbOptions);
        }
    }
}
