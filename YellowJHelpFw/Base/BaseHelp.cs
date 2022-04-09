using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YellowJHelpFw.Entry;

namespace YellowJHelpFw.Base
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
                    //忽略系统带Microsoft和System开头的dll
                    if (assembly.FullName.StartsWith("Microsoft")) { continue; }
                    if (assembly.FullName.StartsWith("System")) { continue; }

                }
                //获取绑定AutoInjectAttribute的程序集
                var types = assembly.GetTypes().Where(a => a.GetCustomAttribute<AutoInjectAttribute>() != null).ToList();
                YJHelp yJHelp = new YJHelp();
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
                    vs.Add(keyValueInfo);
                    break;
                }

            }
            return vs;
        }
    }
}
