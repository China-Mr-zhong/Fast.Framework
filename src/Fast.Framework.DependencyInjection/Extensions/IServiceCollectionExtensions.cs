using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fast.Framework.DependencyInjection
{

    /// <summary>
    /// 服务集合接口扩展类
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="injectDlls">注入Dll</param>
        public static void RegisterServices(this IServiceCollection services, List<InjectDll> injectDlls)
        {
            foreach (var injectDll in injectDlls)
            {
                var serviceAssemblyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, injectDll.ServiceDll);
                var implementationAssemblyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, injectDll.ImplementationDll);

                var serviceAssembly = Assembly.LoadFrom(serviceAssemblyFile);
                var implementationAssembly = Assembly.LoadFrom(implementationAssemblyFile);

                if (injectDll.InjectItems != null && injectDll.InjectItems.Count > 0)
                {
                    foreach (var injectItem in injectDll.InjectItems)
                    {
                        Type serviceType;
                        Type implementationType;

                        if (string.IsNullOrWhiteSpace(injectItem.ServiceType) && !string.IsNullOrWhiteSpace(injectItem.ImplementationType))
                        {
                            implementationType = implementationAssembly.GetType(injectItem.ImplementationType);
                            if (implementationType == null)
                            {
                                throw new Exception($"ImplementationType:{injectItem.ImplementationType}不存在");
                            }
                            switch (injectItem.InjectType)
                            {
                                case InjectType.Singleton:
                                    services.AddSingleton(implementationType);
                                    break;
                                case InjectType.Transient:
                                    services.AddTransient(implementationType);
                                    break;
                                case InjectType.Scoped:
                                    services.AddScoped(implementationType);
                                    break;
                                default:
                                    services.AddTransient(implementationType);
                                    break;
                            }
                        }
                        else
                        {
                            serviceType = serviceAssembly.GetType(injectItem.ServiceType);
                            implementationType = implementationAssembly.GetType(injectItem.ImplementationType);
                            if (serviceType == null)
                            {
                                throw new Exception($"ServiceType:{injectItem.ServiceType}不存在");
                            }
                            if (implementationType == null)
                            {
                                throw new Exception($"ImplementationType:{injectItem.ImplementationType}不存在");
                            }
                            switch (injectItem.InjectType)
                            {
                                case InjectType.Singleton:
                                    services.AddSingleton(serviceType, implementationType);
                                    break;
                                case InjectType.Transient:
                                    services.AddTransient(serviceType, implementationType);
                                    break;
                                case InjectType.Scoped:
                                    services.AddScoped(serviceType, implementationType);
                                    break;
                                default:
                                    services.AddTransient(serviceType, implementationType);
                                    break;
                            }
                        }
                    }
                }
            }
        }

    }
}
