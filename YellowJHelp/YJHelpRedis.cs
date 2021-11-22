using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace YellowJHelp
{
    /// <summary>
    /// redis通用使用（基于ServiceStack.Redis）
    /// </summary>
    public class YJHelpRedis
    {
        /// <summary>
        /// 最大读链接数(默认12)
        /// </summary>
        public static int RedisMaxReadPool = 12;
        /// <summary>
        /// 最大写链接数(默认8)
        /// </summary>
        public static int RedisMaxWritePool = 5;
        /// <summary>
        /// 连接地址（列：123456@127.0.0.1:6379）
        /// </summary>
        public static string redisSession = null;
        private static readonly PooledRedisClientManager pool = null;
        private static readonly string[] redisHosts = null;
        public static RedisPubSubServer pubSubServer;

        static YJHelpRedis()
        {
            var redisHostStr = redisSession;

            if (!string.IsNullOrEmpty(redisHostStr))
            {
                redisHosts = redisHostStr.Split(',');

                if (redisHosts.Length > 0)
                {
                    pool = new PooledRedisClientManager(redisHosts, redisHosts,
                        new RedisClientManagerConfig()
                        {
                            MaxWritePoolSize = RedisMaxWritePool,
                            MaxReadPoolSize = RedisMaxReadPool,
                            AutoStart = true
                        });
                }
            }


        }

        /// <summary>
        /// 存储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry">时效日期</param>
        public static void Add<T>(string key, T value, DateTime expiry)
        {
            if (value == null)
            {
                return;
            }

            if (expiry <= DateTime.Now)
            {
                Remove(key);

                return;
            }

            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.Set(key, value, expiry - DateTime.Now);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "存储", key);
                YJHelp.YellowJLog(msg + ex.Message, "Redis异常");
            }

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            r.Remove(key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "缓存", "删除", key);
                YJHelp.YellowJLog( msg+ ex.Message , "Redis异常");
            }

        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            return r.ContainsKey(key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "是否存在", key);
                YJHelp.YellowJLog(msg + ex.Message, "Redis异常");
            }

            return false;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(T);
            }

            T obj = default(T);

            try
            {
                if (pool != null)
                {
                    using (var r = pool.GetClient())
                    {
                        if (r != null)
                        {
                            r.SendTimeout = 1000;
                            obj = r.Get<T>(key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}:{1}发生异常!{2}", "cache", "获取", key);
                YJHelp.YellowJLog(msg + ex.Message, "Redis异常");
            }


            return obj;
        }

    }
}
