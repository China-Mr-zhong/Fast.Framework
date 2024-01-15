using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// SqlServer Ado提供者
    /// </summary>
    public class SqlServerAdoProvider : AdoProvider
    {
        public override DbProviderFactory DbProviderFactory => SqlClientFactory.Instance;

        public SqlServerAdoProvider(DbOptions dbOptions) : base(dbOptions)
        {
        }
    }
}
