using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Security.Cryptography;

namespace YellowJHelp
{
    public class YJHelp
    {
        /// <summary>
        /// 消息注明
        /// </summary>
        public static string message = "秃头猿YellowJ温馨提醒您：";

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="strText">要加密字符串</param>
        /// <param name="IsLower">是否以小写方式返回</param>
        /// <returns></returns>
        public static string MD5Encrypt(string strText, bool IsLower)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strText);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();

            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return ret.PadLeft(32, '0');
        }

        #region --cache 缓存--
        /// <summary>
        /// 定义cache
        /// </summary>
        private static MemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// 设置cache
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetCacheValue(string key, object value)
        {
            if (key != null)
            {
                cache.Set(key, value, new MemoryCacheEntryOptions
                {
                    //SlidingExpiration = TimeSpan.FromSeconds(10)
                });
            }
        }

        /// <summary>
        /// 获取cache
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static object GetCacheValue(string key)
        {
            object val = null;
            if (key != null && cache.TryGetValue(key, out val))
            {
                return val;
            }
            else
            {
                return default(object);
            }
        }
        #endregion
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="text">消息</param>
        /// <param name="address">相对文件地址</param>
        public static void YellowJLog(string text, string address)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = System.IO.Path.Combine(path
            , "YellowJ_Logs/" + address + "\\");

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string fileFullName = System.IO.Path.Combine(path
            , string.Format("{0}.txt", DateTime.Now.ToString("yyyyMMdd")));


            using (StreamWriter output = System.IO.File.AppendText(fileFullName))
            {
                output.WriteLine(DateTime.Now.ToString() + " 日志信息：" + text);

                output.Close();
            }
        }
    }
}