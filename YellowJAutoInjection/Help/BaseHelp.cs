﻿using System.Reflection;
using YellowJAutoInjection.Entry;

namespace YellowJAutoInjection.Help
{
    /// <summary>
    /// 帮助类
    /// </summary>
    public class BaseHelp
    {
        /// <summary>
        /// 获取自动注入程序集名称
        /// </summary>
        /// <returns></returns>
        public List<KeyValueInfo<Assembly, Assembly>> GetAssembly()
        {
            List<KeyValueInfo<Assembly, Assembly>> vs = new List<KeyValueInfo<Assembly, Assembly>>();

            var path = AppDomain.CurrentDomain.BaseDirectory;
            var assemblies = Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom).ToList();

            foreach (var assembly in assemblies)
            {
                if (!string.IsNullOrEmpty(assembly.FullName))
                {
                    //忽略系统带Microsoft和System开头的dll
                    if (assembly.FullName.StartsWith("Microsoft")) { continue; }
                    if (assembly.FullName.StartsWith("System")) { continue; }

                }
                //获取绑定AutoInjectAttribute的程序集
                var types = assembly.GetTypes().Where(a => a.GetCustomAttribute<AutoInjectAttribute>() != null).ToList();
                AutoHelp yJHelp = new AutoHelp();
                if (types.Count <= 0)
                {
                    yJHelp.YellowJLog($"{assembly.FullName},程序集筛选:不满足标签条件", "YellowJ自动注入程序集信息");
                    continue;
                }
                yJHelp.YellowJLog($"{assembly.FullName},程序集筛选，满足标签条件", "YellowJ自动注入程序集信息");

                foreach (var type in types)
                {
                    var attr = type.GetCustomAttribute<AutoInjectAttribute>();
                    KeyValueInfo<Assembly, Assembly> keyValueInfo = new KeyValueInfo<Assembly, Assembly>();
                    keyValueInfo.Key = assembly;
                    keyValueInfo.Value = attr.Type.Assembly;
                    //if (keyValueInfo.Key==null || keyValueInfo.Value==null) { continue; }
                    vs.Add(keyValueInfo);
                    break;
                }

            }
            return vs;
        }
    }
        
}
