global using YellowJAutoInjection;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using YellowJHelp.IServer;

namespace YellowJHelp
{
    /// <summary>
    /// 通用帮助方法
    /// </summary>
    [AutoInject(typeof(IYJHelp))]
    public class YJHelp: IYJHelp
    {
        /// <summary>
        /// 消息注明
        /// </summary>
        public string message() { return "秃头猿YellowJ温馨提醒您："; }

        #region 加解密
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="strText">要加密字符串</param>
        /// <param name="IsLower">是否以小写方式返回</param>
        /// <returns></returns>
        public Task<string> MD5EncryptAsync(string strText, bool IsLower)
        {
            return Task.Run(() =>
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
            });
            
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data">需要加密的值</param>
        /// <param name="KEY_64">密钥长度8位</param>
        /// <param name="IV_64">密钥长度8位</param>
        /// <returns></returns>
        public async Task<string> Encode(string data,string KEY_64, string IV_64)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            await sw.WriteAsync(data);
            await sw.FlushAsync();
            cst.FlushFinalBlock();
            await sw.FlushAsync();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

        }
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data">需要加密的值</param>
        /// <param name="KEY_64">密钥长度8位</param>
        /// <param name="IV_64">密钥长度8位</param>
        /// <returns></returns>
        public async Task<string> Decode(string data, string KEY_64, string IV_64)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return  "";
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return await sr.ReadToEndAsync();
        }


        #endregion


        /// <summary>
        /// 缓存类
        /// </summary>
        [AutoInject(typeof(IYJHelp.ICache))]
        public class Cache: IYJHelp.ICache
        {
            #region 缓存定义
            //缓存容器 
            private static Dictionary<string, object> CacheDictionary = new Dictionary<string, object>();
            /// <summary>
            /// 添加缓存
            /// </summary>
            public void Add(string key, object value)
            {
                CacheDictionary.Add(key, value);
            }

            /// <summary>
            /// 获取缓存
            /// </summary>
            public T Get<T>(string key)
            {
                return (T)CacheDictionary[key];
            }

            /// <summary>
            /// 判断缓存是否存在
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public bool Exsits(string key)
            {
                return CacheDictionary.ContainsKey(key);
            }
            
            /// <summary>
            /// 删除缓存
            /// </summary>
            /// <param name="key"></param>
            public void Clear(string key)
            {
                CacheDictionary.Remove(key);
            }

            /// <summary>
            /// 删除所有
            /// </summary>
            /// <returns></returns>
            public void Clear()
            {
                CacheDictionary.Clear();
            }

            #endregion
        }
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="text">消息</param>
        /// <param name="address">相对文件名</param>
        public void YellowJLog(string text, string address)
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
        /// <summary>
        /// 字段截取
        /// </summary>
        /// <param name="sourse">值</param>
        /// <param name="startstr">前字符</param>
        /// <param name="endstr">后字符</param>
        /// <returns></returns>
        public string MidStrEx(string sourse, string startstr, string endstr)
        {
            string result = string.Empty;
            int startindex, endindex;
            startindex = sourse.IndexOf(startstr);
            if (startindex == -1)
                return result;
            string tmpstr = sourse.Substring(startindex + startstr.Length);
            endindex = tmpstr.IndexOf(endstr);
            if (endindex == -1)
                return result;
            result = tmpstr.Remove(endindex);

            return result;
        }

        #region --cookie--
        /// <summary>
        /// 设置本地cookie
        /// </summary>
        /// <param name="ctx">Microsoft.AspNetCore.Http</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>  
        /// <param name="minutes">过期时长，单位：分钟</param>      
        public void SetCookies(HttpContext ctx, string key, string value, int minutes = 30)
        {
            ctx.Response.Cookies.Append(key, value);
        }

        /// <summary>
        /// 删除指定的cookie
        /// </summary>
        /// <param name="ctx">Microsoft.AspNetCore.Http</param>
        /// <param name="key">键</param>
        public void DeleteCookies(HttpContext ctx, string key)
        {
            ctx.Response.Cookies.Delete(key);
        }

        /// <summary>
        /// 获取cookies
        /// </summary>
        /// <param name="ctx">Microsoft.AspNetCore.Http</param>
        /// <param name="key">键</param>
        /// <returns>返回对应的值</returns>
        public string GetCookies(HttpContext ctx, string key)
        {
            ctx.Request.Cookies.TryGetValue(key, out string value);
            if (string.IsNullOrEmpty(value))
                value = string.Empty;
            return value;
        }
        #endregion
        #region --Session--
        /// <summary>
        /// 添加Session
        /// </summary>
        /// <param name="ctx">Microsoft.AspNetCore.Http</param>
        /// <param name="strSessionName">Session对象名称</param>
        /// <param name="strValue">Session值</param>
        public void SessionAdd(HttpContext ctx, string strSessionName, byte[] strValue)
        {
            ctx.Session.Set(strSessionName, strValue);
        }

        /// <summary>
        /// 获取Session
        /// </summary>
        /// <param name="ctx">Microsoft.AspNetCore.Http</param>
        /// <param name="cancellationToken"></param>
        public async Task SessionGet(HttpContext ctx, CancellationToken cancellationToken)
        {
              await ctx.Session.LoadAsync(cancellationToken);
        }
        /// <summary>
        /// 删除Session
        /// </summary>
        /// <param name="ctx">Microsoft.AspNetCore.Http</param>
        /// <param name="strSessionName">Session对象名称</param>
        public void SessionDel(HttpContext ctx, string strSessionName)
        {
            ctx.Session.Remove(strSessionName);

        }

        #endregion

        /// <summary>
        /// string中是否包含某个值
        /// </summary>
        /// <param name="data">数据包</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool IsString(string data,string value) {
            if (data.IndexOf(value, StringComparison.OrdinalIgnoreCase)>-1) return true;else return false;
        }

    }
}