using Mapster;
using NewLife.Caching;

namespace YellowJHelp.IServer
{
    /// <summary>
    /// redis通用使用
    /// </summary>
    public interface IYJHelpRedis
    {
        /// <summary>
        /// redis 配置中心
        /// </summary>
        /// <returns></returns>
       FullRedis RedisCli(string? Create = null);


        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="key">健</param>
        /// <returns></returns>
        T Get<T>(string key);
        /// <summary>
        /// 获取列表List
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="key">健</param>
        /// <returns></returns>
        List<T> GetList<T>(string key);


        /// <summary>
        /// 写入单项实体
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="key">健</param>
        /// <param name="enety">值</param>
        /// <param name="expire">过期时间，秒。小于0时采用默认缓存时间NewLife.Caching.Cache.Expire</param>
        void Set<T>(string key, T enety, int expire = -1);


        /// <summary>
        /// 添加，已存在时不更新
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="key">健</param>
        /// <param name="enety">值</param>
        /// <param name="expire">过期时间，秒。小于0时采用默认缓存时间NewLife.Caching.Cache.Expire</param>
        void Add<T>(string key, T enety, int expire = -1);

        /// <summary>
        /// 写入集合：尾部增加
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="key">健</param>
        /// <param name="enety">值</param>
        void ListSetW<T>(string key, List<T> enety);
        /// <summary>
        /// 写入集合：头部增加
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="key">健</param>
        /// <param name="enety">值</param>
        void ListSetT<T>(string key, List<T> enety);

        /// <summary>
        /// 清空所有缓存项
        /// </summary>
        void Clear();


        /// <summary>
        /// 清空所有缓存项
        /// </summary>
        void Remove(string[] Keys);


        /// <summary>
        /// 获取缓存项有效期
        /// </summary>
        void GetExpire(string Key);

        /// <summary>
        /// 是否存在
        /// </summary>
        bool ContainsKey(string Key);


        /// <summary>
        /// 缓存个数
        /// </summary>
        int Count();

        /// <summary>
        /// 返回集合个数
        /// </summary>
        int SCARD(string Key);

    }
}
