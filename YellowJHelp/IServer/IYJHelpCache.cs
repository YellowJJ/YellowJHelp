using Microsoft.Extensions.Caching.Memory;

namespace YellowJHelp.IServer
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface IYJHelpCache
    {
        /// <summary>
        /// 判断指定Key是否存在，并返回对应的字符串值
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">输出参数，存在时返回对应值</param>
        /// <returns>存在返回true，否则false</returns>
        bool TryGetValue(string key, out string value);

        /// <summary>
        /// 判断指定Key是否存在，并返回对应的泛型值
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="value">输出参数，存在时返回对应值</param>
        /// <returns>存在且类型匹配返回true，否则false</returns>
        bool TryGetValue<T>(string key, out T value);

        /// <summary>
        /// 判断指定Key是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>存在返回true，否则false</returns>
        bool TryGetValue(string key);
        /// <summary>
        /// 获取指定Key的缓存对象（支持任意类型）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>缓存对象，未命中返回null</returns>
        public object Get(string key);
        /// <summary>
        /// 泛型获取缓存对象，类型安全
        /// </summary>
        /// <typeparam name="T">期望返回的类型</typeparam>
        /// <param name="key">缓存Key</param>
        /// <returns>缓存对象，未命中返回默认值</returns>
        public T Get<T>(string key);
        /// <summary>
        /// 写入缓存（支持过期策略，返回写入的字符串值）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存值（string）</param>
        /// <param name="options">缓存项配置（过期时间、优先级等）</param>
        /// <returns>写入的字符串值</returns>
        public string Set(string key, string value, MemoryCacheEntryOptions options);
        /// <summary>
        /// 泛型写入缓存（支持任意类型，支持过期策略）
        /// </summary>
        /// <typeparam name="T">缓存值类型</typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存值</param>
        /// <param name="options">缓存项配置</param>
        public void Set<T>(string key, T value, MemoryCacheEntryOptions options);
        /// <summary>
        /// 移除指定Key的缓存项
        /// </summary>
        /// <param name="key">缓存Key</param>
        public void Remove(string key);
    }
}
