using Mapster;
using NewLife.Caching;
using YellowJHelp.IServer;

namespace YellowJHelp
{

    /// <summary>
    /// redis通用使用
    /// </summary>
    [AutoInject(typeof(IYJHelpRedis))]
    public class YJHelpRedis: IYJHelpRedis
    {
        /// <summary>
        /// redis 配置中心
        /// </summary>
        /// <returns></returns>
        public FullRedis RedisCli(string? Create=null)
        {
            var ret = new FullRedis();
            if (!string.IsNullOrEmpty(Create)) { ret= FullRedis.Create(Create);}
            //var ret = FullRedis.Create(OPSDbs.Configuration["Redis:Create"]);
            //ret.UserName = OPSDbs.Configuration["Redis:UserName"];
            //ret.Password = OPSDbs.Configuration["Redis:Password"];
            //*fullRedis.StartPipeline();*/
            //if (!string.IsNullOrEmpty(OPSDbs.Configuration["Redis:MaxMessageSize"]))
            //{
            //    ret.MaxMessageSize = 1024 * 1024 * (Convert.ToInt32(OPSDbs.Configuration["Redis:MaxMessageSize"]));
            //}
            //ret.Db = Convert.ToInt32(OPSDbs.Configuration["Redis:Db"]);
            //ret.Expire = Convert.ToInt32(OPSDbs.Configuration["Redis:Expire"]);
            return ret;
        }


        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="key">健</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            var ret = RedisCli().Get<T>(key);
            return ret;
        }
        /// <summary>
        /// 获取列表List
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="key">健</param>
        /// <returns></returns>
        public List<T> GetList<T>(string key)
        {
            var ret = RedisCli().GetList<T>(key).ToList();

            return ret;
        }


        /// <summary>
        /// 写入单项实体
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="key">健</param>
        /// <param name="enety">值</param>
        /// <param name="expire">过期时间，秒。小于0时采用默认缓存时间NewLife.Caching.Cache.Expire</param>
        public void Set<T>(string key, T enety, int expire = -1)
        {
            var ret = RedisCli().Set(key, enety, expire);
        }


        /// <summary>
        /// 添加，已存在时不更新
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="key">健</param>
        /// <param name="enety">值</param>
        /// <param name="expire">过期时间，秒。小于0时采用默认缓存时间NewLife.Caching.Cache.Expire</param>
        public void Add<T>(string key, T enety, int expire = -1)
        {
            var ret = RedisCli().Add(key, enety, expire);
        }

        /// <summary>
        /// 写入集合：尾部增加
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="key">健</param>
        /// <param name="enety">值</param>
        public void ListSetW<T>(string key, List<T> enety)
        {
            T[] ts = enety.Adapt<T[]>();
            var ret = RedisCli().RPUSH<T>(key, ts);

        }
        /// <summary>
        /// 写入集合：头部增加
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="key">健</param>
        /// <param name="enety">值</param>
        public void ListSetT<T>(string key, List<T> enety)
        {
            T[] ts = enety.Adapt<T[]>();
            var ret = RedisCli().LPUSH<T>(key, ts);

        }

        /// <summary>
        /// 清空所有缓存项
        /// </summary>
        public void Clear()
        {
            RedisCli().Clear();
        }


        /// <summary>
        /// 清空所有缓存项
        /// </summary>
        public void Remove(string[] Keys)
        {
            RedisCli().Remove(Keys);
        }


        /// <summary>
        /// 获取缓存项有效期
        /// </summary>
        public void GetExpire(string Key)
        {
            RedisCli().GetExpire(Key);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        public bool ContainsKey(string Key)
        {
            return RedisCli().ContainsKey(Key);
        }


        /// <summary>
        /// 缓存个数
        /// </summary>
        public int Count()
        {
            return RedisCli().Count;
        }

        /// <summary>
        /// 返回集合个数
        /// </summary>
        public int SCARD(string Key)
        {
            return RedisCli().SCARD(Key);
        }

    }
}
