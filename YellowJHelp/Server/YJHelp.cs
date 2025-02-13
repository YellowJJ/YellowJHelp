global using YellowJAutoInjection;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using YellowJAutoInjection.Entry;
using YellowJHelp.Entry;
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
        public async Task<string> EncodeAsync(string data,string KEY_64, string IV_64)
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
        public async Task<string> DecodeAsync(string data, string KEY_64, string IV_64)
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
        [AutoInject(typeof(IYJHelp.ICache)),Obsolete("请使用的新的接口方法：YJHelpCache")]
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
        public async Task YellowJLogAsync(string text, string address)
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
                await output.WriteLineAsync(DateTime.Now.ToString() + " 日志信息：" + text);

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
        public async Task<string> MidStrExAsync(string sourse, string startstr, string endstr)
        {
            return await Task.Run(() => {
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
            });
            
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
        /// <summary>
        /// 分配数据
        /// </summary>
        /// <param name="yAllocations">分配集合</param>
        /// <param name="yAllocations1">被分配集合</param>
        /// <returns>返回分配集合（剩余），被分配集合（已分），分配结果详情</returns>
        public List<List<YAllocationInfo>> YAlloctionlist(List<YAllocationInfo> yAllocations, List<YAllocationInfo> yAllocations1)
        {
            List<List<YAllocationInfo>> yAllocationInfos = new();
            List<YAllocationInfo> list = new();
            Dictionary<string, List<YAllocationInfo>> allocationDict = yAllocations.GroupBy(a => a.Key).ToDictionary(g => g.Key, g => g.ToList());

            //foreach (var item in yAllocations)
            //{
            //    item.RemQty = item.Qty;
            //}

            foreach (var item in yAllocations1)
            {
                decimal itemqty = item.Qty - item.AQty;
                if (itemqty <= 0) continue;

                if (allocationDict.TryGetValue(item.Key, out var aa))
                {
                    foreach (var it in aa)
                    {
                        if (it.RemQty <= 0) continue;

                        YAllocationInfo info = new()
                        {
                            Number = item.Number,
                            Key = item.Key,
                            Qty = item.Qty,
                            FPNumber = it.Number
                        };

                        if (it.RemQty >= itemqty)
                        {
                            item.AQty += itemqty;
                            it.RemQty -= itemqty;
                            info.AQty = itemqty;
                            list.Add(info);
                            break;
                        }
                        else
                        {
                            item.AQty += it.RemQty;
                            itemqty -= it.RemQty;
                            info.AQty = it.RemQty;
                            it.RemQty = 0;
                            list.Add(info);
                        }
                    }
                }
            }

            yAllocationInfos.Add(yAllocations);
            yAllocationInfos.Add(yAllocations1);
            yAllocationInfos.Add(list);
            return yAllocationInfos;
        }

        /// <summary>
        /// 分配数据-多线程
        /// </summary>
        /// <param name="yAllocations">分配集合</param>
        /// <param name="yAllocations1">被分配集合</param>
        /// <returns>返回分配集合（剩余），被分配集合（已分），分配结果详情</returns>
        public List<List<YAllocationInfo>> YAlloctionlistThred(List<YAllocationInfo> yAllocations, List<YAllocationInfo> yAllocations1)
        {
            ConcurrentBag<KeyValueInfo<List<YAllocationInfo>, List<YAllocationInfo>>> keyValuePairs = new();
            #region 分组
            //List<List<YAllocationInfo>> listYA1 = new();
            #region 注释方法
            //foreach (var item in yAllocations)
            //{
            //    bool isnull = true;
            //    var df = listYA1.Select(a => a.Where(b => b.Key == item.Key).FirstOrDefault()).ToList();
            //    if (df.Count > 0 && df[0] != null)
            //    {
            //        YJHelpT<YAllocationInfo> yJHelpT = new();
            //        df.Add(yJHelpT.Copy(item));
            //        isnull = false;
            //    }
            //    else
            //    {
            //        List<YAllocationInfo> yss = new();
            //        YJHelpT<YAllocationInfo> yJHelpT = new();
            //        yss.Add(yJHelpT.Copy(item));
            //        listYA1.Add(yss);
            //    }
            //    //foreach (var item1 in listYA1)
            //    //{
            //    //    var llt1 = item1.Where(a => a.Key == item.Key).FirstOrDefault();
            //    //    if (llt1 != null)
            //    //    {

            //    //        break;
            //    //    }
            //    //}
            //    //if (isnull)
            //    //{
            //    //    List<YAllocationInfo> yss = new();
            //    //    YJHelpT<YAllocationInfo> yJHelpT = new();
            //    //    yss.Add(yJHelpT.Copy(item));
            //    //    listYA1.Add(yss);
            //    //}
            //}
            #endregion
            var listYA1s = yAllocations.GroupBy(a => a.Key).ToDictionary(g => g.Key, g => g.ToList());

            //List<List<YAllocationInfo>> listYA2 = new();
            #region 注释方法
            //foreach (var item in yAllocations1)
            //{
            //    bool isnull = true;
            //    foreach (var item1 in listYA2)
            //    {
            //        var llt1 = item1.Where(a => a.Key == item.Key).FirstOrDefault();
            //        if (llt1 != null)
            //        {
            //            YJHelpT<YAllocationInfo> yJHelpT = new();
            //            item1.Add(yJHelpT.Copy(item));
            //            isnull = false;
            //            break;
            //        }
            //    }
            //    if (isnull)
            //    {
            //        List<YAllocationInfo> yss = new();
            //        YJHelpT<YAllocationInfo> yJHelpT = new();
            //        yss.Add(yJHelpT.Copy(item));
            //        listYA2.Add(yss);
            //    }
            //}


            //foreach (var item in listYA1)
            //{
            //    KeyValueInfo<List<YAllocationInfo>, List<YAllocationInfo>> keyValueInfo = new();
            //    bool isnull = true;
            //    foreach (var item1 in listYA2)
            //    {
            //        var iit = item1.Where(a => a.Key == item[0].Key).FirstOrDefault();
            //        if (iit != null)
            //        {
            //            keyValueInfo.Value = item1;
            //            isnull = false;
            //            break;
            //        }
            //    }
            //    if (!isnull)
            //    {
            //        keyValueInfo.Key = item;
            //        keyValuePairs.Add(keyValueInfo);
            //    }
            //}
            #endregion
            var listYA2s = yAllocations1.GroupBy(a => a.Key).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var item in listYA1s)
            {
                KeyValueInfo<List<YAllocationInfo>, List<YAllocationInfo>> keyValueInfo = new();
                bool isnull = false;
                if (listYA2s.TryGetValue(item.Key, out var aa))
                {
                    keyValueInfo.Value = aa;
                    isnull = true;
                }
                if (isnull)
                {
                    keyValueInfo.Key = item.Value;
                    keyValuePairs.Add(keyValueInfo);
                }

            }

            #endregion
            #region 分配
            ConcurrentBag<ConcurrentBag<YAllocationInfo>> yAllInfos = new();
            for(int i=0;i<3;i++)
            {
                ConcurrentBag<YAllocationInfo> ys = new();
                yAllInfos.Add(ys);
            }
            Parallel.ForEach(keyValuePairs, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount-1 }, item => {
                ConcurrentBag<YAllocationInfo> ys = new();
                var ret=YAlloctionlist(item.Key,item.Value);
                int i = 0;
                foreach (var item1 in yAllInfos) 
                {
                    foreach (var item2 in ret[i]) 
                    {
                        item1.Add(item2);
                    }
                    
                    i++;
                }
            });
            #endregion

            //List<List<YAllocationInfo>> allocationInfos = new();
            //foreach (var item in yAllInfos)
            //{
            //    List<YAllocationInfo> yAllocationInfos = new();
            //    foreach (var item1 in item)
            //    {
            //        yAllocationInfos.Add(item1);
            //    }
            //    allocationInfos.Add(yAllocationInfos);
            //}

            return yAllInfos.Select(a => a.ToList()).ToList();
        }

        /// <summary>
        /// 生成雪花ID
        /// </summary>
        /// <param name="workerId">工作者的标识</param>
        /// <returns></returns>
        public long NextId(long workerId) 
        {
            SnowflakeIdGenerator snowflakeIdGenerator = new(workerId);
            return snowflakeIdGenerator.NextId();
        }

        public FastMapper Mapper()
        {
            return new FastMapper();
        }
    }
}