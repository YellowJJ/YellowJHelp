using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YellowJHelpFw.Base;
using YellowJHelpFw.Entry;

namespace YellowJHelpFw
{
    /// <summary>
    /// 自动注入
    /// </summary>
    public static class YJDiInject
    {
        /// <summary>
        /// 自动注入所有的程序集（Autofac继承默认生命周期）
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IHostBuilder YJAutofacDiInJect(this IHostBuilder host)
        {
            //Autofac自动依赖注入
            host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                //新模块组件注册    
                builder.RegisterModule<AutofacModuleRegister>();
            });

            return host;
        }

        /// <summary>
        /// 自动注入所有的程序集(可控制生命周期)
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection YJDiInJect(this IServiceCollection serviceCollection)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var assemblies = Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom).ToList();
            foreach (var assembly in assemblies)
            {
                if (!string.IsNullOrEmpty(assembly.FullName))
                {
                    if (assembly.FullName.IndexOf("Microsoft", StringComparison.OrdinalIgnoreCase) > -1) { continue; }
                }

                var types = assembly.GetTypes().Where(a => a.GetCustomAttribute<AutoInjectAttribute>() != null)
                    .ToList();
                if (types.Count <= 0) continue;
                foreach (var type in types)
                {
                    var attr = type.GetCustomAttribute<AutoInjectAttribute>();
                    if (attr?.Type == null) continue;
                    switch (attr.InjectType)
                    {
                        case InjectType.Scope:
                            serviceCollection.AddScoped(attr.Type, type);
                            break;
                        case InjectType.Single:
                            serviceCollection.AddSingleton(attr.Type, type);
                            break;
                        case InjectType.Transient:
                            serviceCollection.AddTransient(attr.Type, type);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return serviceCollection;
        }
    }

    /// <summary>
    /// Autofac自动依赖注入配置
    /// </summary>
    public class AutofacModuleRegister : Autofac.Module
    {
        //重写Autofac管道Load方法，在这里注册注入
        protected override void Load(ContainerBuilder builder)
        {
            var dllname = BaseHelp.GetAssembly();

            foreach (var item in dllname)
            {
                //注册程序集
                builder.RegisterAssemblyTypes(item.Key, item.Value).AsImplementedInterfaces();
            }
        }
        /// <summary>
        /// 根据程序集名称获取程序集
        /// </summary>E
        /// <param name="AssemblyName">程序集名称</param>
        /// <returns></returns>
        public static Assembly GetAssemblyByName(String AssemblyName)
        {
            return Assembly.Load(AssemblyName);
        }
    }
}
