using Fast.Framework.Implements;
using Fast.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.Oracle
{

    /// <summary>
    /// Oracle数据提供者
    /// </summary>
    public class OracleAdoProvider : AdoProvider
    {

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dbOptions">数据库选项</param>
        public OracleAdoProvider(DbOptions dbOptions) : base(dbOptions)
        {
            this.Command.GetType().GetProperty("BindByName").SetValue(Command, true);
        }
    }
}
