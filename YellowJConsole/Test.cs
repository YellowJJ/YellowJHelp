using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowJHelp.IServer;

namespace YellowJConsole
{
    public class Test
    {
        public readonly IYJHelpCache _yJHelpCache;

        public Test(IYJHelpCache yJHelpCache)
        {
            _yJHelpCache = yJHelpCache;
        }
        public bool sd(string key) 
        {
            return _yJHelpCache.TryGetValue(key, out var value);
        }
    }
}
