using Microsoft.Extensions.Caching.Memory;
using YellowJHelp.IServer;

namespace YellowJHelp.Server
{
    [AutoInject(typeof(IYJHelpCache))]
    public class YJHelpCache: IYJHelpCache
    {
        // 内部缓存对象，依赖注入方式获取，便于单元测试和扩展
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 构造函数，注入IMemoryCache实例
        /// </summary>
        /// <param name="memoryCache">内存缓存实例</param>
        public YJHelpCache(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        /// <summary>
        /// 判断指定Key是否存在，并返回对应的泛型值
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="value">输出参数，存在时返回对应值</param>
        /// <returns>存在且类型匹配返回true，否则false</returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            if (_cache.TryGetValue(key, out object obj) && obj is T typedValue)
            {
                value = typedValue;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 判断指定Key是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>存在返回true，否则false</returns>
        public bool TryGetValue(string key)
        {
            return _cache.TryGetValue(key, out _);
        }

        /// <summary>
        /// 判断指定Key是否存在，并返回对应的字符串值
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">输出参数，存在时返回对应值</param>
        /// <returns>存在返回true，否则false</returns>
        public bool TryGetValue(string key, out string value)
        {
            // 只支持string类型，若缓存为其它类型会返回null
            if (_cache.TryGetValue(key, out object obj) && obj is string str)
            {
                value = str;
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// 获取指定Key的缓存对象（支持任意类型）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>缓存对象，未命中返回null</returns>
        public object Get(string key)
        {
            return _cache.Get(key);
        }

        /// <summary>
        /// 泛型获取缓存对象，类型安全
        /// </summary>
        /// <typeparam name="T">期望返回的类型</typeparam>
        /// <param name="key">缓存Key</param>
        /// <returns>缓存对象，未命中返回默认值</returns>
        public T Get<T>(string key)
        {
            return _cache.TryGetValue(key, out var obj) && obj is T t ? t : default;
        }

        /// <summary>
        /// 写入缓存（支持过期策略，返回写入的字符串值）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存值（string）</param>
        /// <param name="options">缓存项配置（过期时间、优先级等）</param>
        /// <returns>写入的字符串值</returns>
        public string Set(string key, string value, MemoryCacheEntryOptions options)
        {
            return _cache.Set(key, value, options);
        }

        /// <summary>
        /// 泛型写入缓存（支持任意类型，支持过期策略）
        /// </summary>
        /// <typeparam name="T">缓存值类型</typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存值</param>
        /// <param name="options">缓存项配置</param>
        public void Set<T>(string key, T value, MemoryCacheEntryOptions options)
        {
            _cache.Set(key, value, options);
        }

        /// <summary>
        /// 移除指定Key的缓存项
        /// </summary>
        /// <param name="key">缓存Key</param>
        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// 清空所有缓存（需IMemoryCache支持，默认实现不支持全清）
        /// </summary>
        /// <remarks>
        /// 1. IMemoryCache默认不支持全清，如需全清可扩展为自定义MemoryCache实现或记录所有Key。
        /// 2. 可扩展为支持分组清理、按前缀清理等。
        /// </remarks>
        //public void ClearAll()
        //{
        //    // 仅做接口预留，实际IMemoryCache不支持全清
        //    // 可通过自定义MemoryCache实现或记录所有Key来实现
        //    // throw new NotSupportedException("IMemoryCache默认不支持全清，如需全清请自定义实现。");
        //}
    }
}
