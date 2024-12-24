using Microsoft.Extensions.Caching.Memory;

namespace YellowJHelp.IServer
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface IYJHelpCache
    {
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">存在返回数据，不存在覆盖</param>
        /// <returns></returns>
        bool TryGetValue(string key, out string value);
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key);
        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public string Set(string key, string value, MemoryCacheEntryOptions options);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key);
    }
}
