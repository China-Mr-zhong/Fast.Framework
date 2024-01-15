using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// PostgreSql Ado提供者
    /// </summary>
    public class PostgreSqlAdoProvider : AdoProvider
    {
        public override DbProviderFactory DbProviderFactory => NpgsqlFactory.Instance;
        public PostgreSqlAdoProvider(DbOptions dbOptions) : base(dbOptions)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }
    }
}
