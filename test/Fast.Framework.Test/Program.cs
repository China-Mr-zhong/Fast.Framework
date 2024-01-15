using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading;
using Fast.Framework.Test.Models;

namespace Fast.Framework.Test
{
    public class Program
    {

        static void Main(string[] args)
        {
            try
            {
                IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();

                var dbOptions = configuration.GetSection("DbOptions").Get<List<DbOptions>>();
                var db = new DbContext(dbOptions);

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

                db.Aop.SlaveDbFault = (options, ex) =>
                {
                    Console.WriteLine($"从库ID:{options.DbId} 发生故障!!! 异常信息:{ex.Message}");
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}