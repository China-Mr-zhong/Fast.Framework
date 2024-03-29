﻿using Microsoft.Extensions.DependencyInjection;

namespace Fast.Framework
{

    /// <summary>
    /// 服务集合接口扩展类
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// 添加Fast数据库上下文
        /// </summary>
        /// <param name="services">服务</param>
        /// <returns></returns>
        public static IServiceCollection AddFastDbContext(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IDbContext), typeof(DbContext));
        }

        /// <summary>
        /// 添加工作单元
        /// </summary>
        /// <param name="services">服务</param>
        /// <returns></returns>
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
        }
    }
}
