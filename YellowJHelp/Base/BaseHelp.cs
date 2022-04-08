using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YellowJHelp.Entry;

namespace YellowJHelp.Base
{
    internal class BaseHelp
    {

        /// <summary>
        /// 获取自动注入程序集名称
        /// </summary>
        /// <returns></returns>
        public static List<KeyValueInfo<Assembly, Assembly>> GetAssembly()
        {
            List<KeyValueInfo<Assembly, Assembly>> vs = new List<KeyValueInfo<Assembly, Assembly>>();

            var path = AppDomain.CurrentDomain.BaseDirectory;
            var assemblies = Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom).ToList();

            foreach (var assembly in assemblies)
            {
                if (!string.IsNullOrEmpty(assembly.FullName))
                {
                    //忽略系统带Microsoft的dll
                    if (assembly.FullName.IndexOf("Microsoft", StringComparison.OrdinalIgnoreCase) > -1) { continue; }
                }
                //获取绑定AutoInjectAttribute的程序集
                var types = assembly.GetTypes().Where(a => a.GetCustomAttribute<AutoInjectAttribute>() != null).ToList();
                if (types.Count <= 0) continue;
                if (types.Count > 0)
                {
                    var attr = types[0].GetCustomAttribute<AutoInjectAttribute>();
                    KeyValueInfo<Assembly, Assembly> keyValueInfo = new KeyValueInfo<Assembly, Assembly>();
                    keyValueInfo.Key = assembly;
                    keyValueInfo.Value = attr.Type.Assembly;
                    vs.Add(keyValueInfo);
                }

            }
            return vs;
        }
    }
}
