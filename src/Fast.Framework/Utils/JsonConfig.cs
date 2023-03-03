using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Fast.Framework.Cache;

namespace Fast.Framework.Utils
{

    /// <summary>
    /// Json配置工具类
    /// </summary>
    public static class JsonConfig
    {

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public static IConfiguration GetInstance(string fileName = "appsettings.json")
        {
            return StaticCache<IConfiguration>.GetOrAdd(fileName, () =>
            {
                return new ConfigurationBuilder().AddJsonFile(fileName, true, true).Build();
            });
        }
    }
}

