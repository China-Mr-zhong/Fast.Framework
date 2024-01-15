using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// Sqlite Ado提供者
    /// </summary>
    public class SqliteAdoProvider : AdoProvider
    {
        public override DbProviderFactory DbProviderFactory => SQLiteFactory.Instance;

        public SqliteAdoProvider(DbOptions dbOptions) : base(dbOptions)
        {
        }
    }
}
