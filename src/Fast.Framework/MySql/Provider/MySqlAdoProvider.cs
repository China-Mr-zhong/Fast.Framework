using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework
{

    /// <summary>
    /// MySql Ado提供者
    /// </summary>
    public class MySqlAdoProvider : AdoProvider
    {
        public override DbProviderFactory DbProviderFactory => MySqlConnectorFactory.Instance;

        public MySqlAdoProvider(DbOptions dbOptions) : base(dbOptions)
        {
        }
    }
}
