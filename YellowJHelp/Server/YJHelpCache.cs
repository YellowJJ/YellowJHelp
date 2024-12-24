using Microsoft.Extensions.Caching.Memory;
using YellowJHelp.IServer;

namespace YellowJHelp.Server
{
    [AutoInject(typeof(IYJHelpCache))]
    public class YJHelpCache: IYJHelpCache
    {
        private readonly IMemoryCache _cache;

        public YJHelpCache(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        public bool TryGetValue(string key, out string value)
        {
            return _cache.TryGetValue(key,out value);
        }
        public object Get(string key)
        {
            return _cache.Get(key);
        }
        public string Set(string key,string value, MemoryCacheEntryOptions options)
        {
            return _cache.Set(key, value, options);
        }
        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
