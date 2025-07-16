global using YellowJAutoInjection;
using Force.DeepCloner;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
                if (string.IsNullOrEmpty(strText))
                    return string.Empty;

                using (MD5 md5 = MD5.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(strText);
                    byte[] hashBytes = md5.ComputeHash(bytes);

                    StringBuilder sb = new StringBuilder(32);
                    foreach (byte b in hashBytes)
                    {
                        sb.Append(b.ToString("x2"));
                    }
                    string result = sb.ToString().PadLeft(32, '0');
                    return IsLower ? result.ToLower() : result.ToUpper();
                }
            });
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data">需要加密的值</param>
        /// <param name="KEY_64">密钥长度8位</param>
        /// <param name="IV_64">密钥长度8位</param>
        /// <returns></returns>
        public async Task<string> EncodeAsync(string data, string KEY_64, string IV_64)
        {
            if (string.IsNullOrEmpty(data) || KEY_64?.Length != 8 || IV_64?.Length != 8)
                throw new ArgumentException("参数无效，密钥和IV必须为8位");

            byte[] byKey = Encoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = Encoding.ASCII.GetBytes(IV_64);

            using (DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider())
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write))
            using (StreamWriter sw = new StreamWriter(cst))
            {
                await sw.WriteAsync(data);
                await sw.FlushAsync();
                cst.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
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
            if (string.IsNullOrEmpty(data) || KEY_64?.Length != 8 || IV_64?.Length != 8)
                throw new ArgumentException("参数无效，密钥和IV必须为8位");

            try
            {
                byte[] byKey = Encoding.ASCII.GetBytes(KEY_64);
                byte[] byIV = Encoding.ASCII.GetBytes(IV_64);
                byte[] byEnc = Convert.FromBase64String(data);

                using (DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider())
                using (MemoryStream ms = new MemoryStream(byEnc))
                using (CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cst))
                {
                    return await sr.ReadToEndAsync();
                }
            }
            catch
            {
                return string.Empty;
            }
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
            path = System.IO.Path.Combine(path, "YellowJ_Logs/" + address + "\\");

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string fileFullName = System.IO.Path.Combine(path, string.Format("{0}.txt", DateTime.Now.ToString("yyyyMMdd")));

            // 显式指定带 BOM 的 UTF-8 编码  
            using (var stream = new FileStream(fileFullName, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (var output = new StreamWriter(stream, new UTF8Encoding(true)))
            {
                await output.WriteLineAsync($"{DateTime.Now} 日志信息：{text}").ConfigureAwait(false);
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
            return data?.IndexOf(value, StringComparison.OrdinalIgnoreCase) > -1;
        }
        /// <summary>
        /// 分配数据
        /// </summary>
        /// <param name="yAllocations">分配集合</param>
        /// <param name="yAllocations1">被分配集合</param>
        /// <returns>返回分配集合（剩余），被分配集合（已分），分配结果详情</returns>
        public List<List<YAllocationInfo>> YAlloctionlist(List<YAllocationInfo> yAllocations, List<YAllocationInfo> yAllocations1)
        {
            if (yAllocations == null || yAllocations1 == null)
                return new List<List<YAllocationInfo>>();

            List<List<YAllocationInfo>> yAllocationInfos = new();
            List<YAllocationInfo> resultList = new();
            var allocationDict = yAllocations.GroupBy(a => a.Key).ToDictionary(g => g.Key, g => g.ToList());

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
                            resultList.Add(info);
                            break;
                        }
                        else
                        {
                            item.AQty += it.RemQty;
                            itemqty -= it.RemQty;
                            info.AQty = it.RemQty;
                            it.RemQty = 0;
                            resultList.Add(info);
                        }
                    }
                }
            }

            yAllocationInfos.Add(yAllocations);
            yAllocationInfos.Add(yAllocations1);
            yAllocationInfos.Add(resultList);
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
            if (yAllocations == null || yAllocations1 == null)
                return new List<List<YAllocationInfo>>();

            var keyValuePairs = new ConcurrentBag<KeyValueInfo<List<YAllocationInfo>, List<YAllocationInfo>>>();
            var listYA1s = yAllocations.GroupBy(a => a.Key).ToDictionary(g => g.Key, g => g.ToList());
            var listYA2s = yAllocations1.GroupBy(a => a.Key).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var item in listYA1s)
            {
                if (listYA2s.TryGetValue(item.Key, out var aa))
                {
                    keyValuePairs.Add(new KeyValueInfo<List<YAllocationInfo>, List<YAllocationInfo>>
                    {
                        Key = item.Value,
                        Value = aa
                    });
                }
            }

            var yAllInfos = new ConcurrentBag<ConcurrentBag<YAllocationInfo>>();
            for (int i = 0; i < 3; i++)
            {
                yAllInfos.Add(new ConcurrentBag<YAllocationInfo>());
            }

            Parallel.ForEach(keyValuePairs, new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) }, item =>
            {
                var ret = YAlloctionlist(item.Key, item.Value);
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
        /// <summary>
        /// 对象映射器
        /// </summary>
        /// <returns></returns>
        public FastMapper Mapper()
        {
            return new FastMapper();
        }
        /// <summary>
        /// 日期是否在目标年月内
        /// </summary>
        /// <param name="date">输入日期</param>
        /// <param name="targetDate">目标日期</param>
        /// <returns></returns>
        public bool IsDateInTargetMonth(DateTime date, DateTime targetDate)
        {
            return date.Year == targetDate.Year && date.Month == targetDate.Month;
        }

        #region 泛型方法
        /// <summary>
        /// 对象副本
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>将对象复制成全新的对象，且不互相影响</returns>
        public T? Copy<T>(T data)
        {
            return data.DeepClone();
        }
        /// <summary>
        /// YJ版本：合并两个集合的函数-不允许有重复项
        /// </summary>
        /// <param name="list1">第一个集合</param>
        /// <param name="list2">第二个集合</param>
        /// <returns>返回结果</returns>
        public List<T> YJMerge<T>(List<T> list1, List<T> list2)
        {
            List<string> strings = new();
            foreach (var item in list1)
            {
                var js = JsonSerializer.Serialize(item);
                strings.Add(js);
            }
            foreach (var item in list2)
            {
                var js = JsonSerializer.Serialize(item);
                strings.Add(js);
            }

            HashSet<string> ha = new HashSet<string>(strings);
            var re = new List<string>(ha);
            List<T> listRet = new();
            foreach (var item in re)
            {
                listRet.Add(JsonSerializer.Deserialize<T>(item));
            }
            return listRet;//返回第一项
        }
        /// <summary>
        /// 集合去重
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public List<T> Distinct<T>(List<T> list)
        {
            List<string> strings = new();
            foreach (var item in list)
            {
                var js = JsonSerializer.Serialize(item);
                strings.Add(js);
            }
            HashSet<string> ha = new HashSet<string>(strings);
            var re = new List<string>(ha);
            List<T> listRet = new();
            foreach (var item in re)
            {
                listRet.Add(JsonSerializer.Deserialize<T>(item));
            }

            return listRet;
        }
        /// <summary>
        /// 异步根据指定字段去重，生成全新且不扰动原集合的去重List
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <typeparam name="TKey">唯一性字段类型（可为单字段、匿名类型、元组等）</typeparam>
        /// <param name="list">待去重的集合</param>
        /// <param name="keySelector">唯一性字段选择器（如：item => item.Key，或 item => new { item.Key, item.Name }）</param>
        /// <returns>去重后的新集合（原集合不变）</returns>
        /// <remarks>
        /// 优势：
        /// 1. 支持任意字段或字段组合去重，灵活性极高，满足复杂业务唯一性需求。
        /// 2. 不修改原集合，返回全新List，安全无副作用，线程安全。
        /// 3. 泛型实现，适用于任何类型的List，支持匿名类型、元组等多字段组合。
        /// 4. 性能高，基于HashSet实现，O(n)复杂度，适合大数据量。
        /// 5. 异步实现，适合Web、服务端等需要避免阻塞主线程的场景。
        ///
        /// 使用场景：
        /// - 需要根据业务唯一性规则（如单字段或多字段）去重的场景。
        /// - 需要保留原集合不变，生成新集合的场景。
        /// - 适用于实体对象、DTO、ViewModel等各种类型。
        /// - 数据导入、批量处理、接口返回前的去重。
        ///
        /// 使用方法示例：
        /// var newList = await DistinctByFieldsAsync(list, x => x.Key); // 单字段
        /// var newList = await DistinctByFieldsAsync(list, x => new { x.Key, x.Name }); // 多字段
        /// var newList = await DistinctByFieldsAsync(list, x => Tuple.Create(x.Key, x.Name)); // 多字段（元组）
        /// </remarks>
        public async Task<List<T>> DistinctAsync<T, TKey>(List<T> list, Func<T, TKey> keySelector)
        {
            if (list == null) throw new ArgumentNullException(nameof(list), "集合不能为空");
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector), "Key选择器不能为空");

            return await Task.Run(() =>
            {
                var seen = new HashSet<TKey>();
                var result = new List<T>();
                foreach (var item in list)
                {
                    var key = keySelector(item);
                    if (seen.Add(key))
                    {
                        result.Add(item);
                    }
                }
                return result;
            });
        }

        /// <summary>
        /// 异步将List集合根据指定键选择器生成字典，便于快速查找
        /// </summary>
        /// <typeparam name="TSource">集合元素类型</typeparam>
        /// <typeparam name="TKey">字典Key类型</typeparam>
        /// <param name="list">要转换的集合</param>
        /// <param name="keySelector">用于获取Key的委托（如：item => item.Key）</param>
        /// <param name="allowDuplicate">是否允许重复Key（true时后者覆盖前者，false时遇到重复抛异常）</param>
        /// <returns>以指定Key为键的字典</returns>
        /// <exception cref="ArgumentNullException">参数为空时抛出</exception>
        /// <exception cref="ArgumentException">存在重复Key且不允许重复时抛出,默认允许重复</exception>
        public async Task<Dictionary<TKey, TSource>> ToDictAsync<TSource, TKey>(
            List<TSource> list,
            Func<TSource, TKey> keySelector,
            bool allowDuplicate = false)
            where TKey : notnull
        {
            // 参数校验
            if (list == null) throw new ArgumentNullException(nameof(list), "集合不能为空");
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector), "Key选择器不能为空");

            // 异步执行字典构建
            return await Task.Run(() =>
            {
                var dict = new Dictionary<TKey, TSource>();
                foreach (var item in list)
                {
                    var key = keySelector(item);
                    if (dict.ContainsKey(key))
                    {
                        if (allowDuplicate)
                        {
                            // 允许重复时，后者覆盖前者
                            dict[key] = item;
                        }
                        else
                        {
                            // 不允许重复时，抛出异常
                            throw new ArgumentException($"集合中存在重复的Key: {key}");
                        }
                    }
                    else
                    {
                        dict.Add(key, item);
                    }
                }
                return dict;
            });
        }

        /// <summary>
        /// 异步将List集合根据指定集合Key选择器生成字典，支持Key为集合类型（如List&lt;string&gt;），便于快速查找
        /// </summary>
        /// <typeparam name="TSource">集合元素类型</typeparam>
        /// <typeparam name="TKeyItem">集合Key的元素类型</typeparam>
        /// <param name="list">要转换的集合</param>
        /// <param name="keySelector">用于获取集合Key的委托（如：item => item.Number）</param>
        /// <param name="allowDuplicate">是否允许重复Key（true时后者覆盖前者，false时遇到重复抛异常）</param>
        /// <returns>以集合Key为键的字典（Key为集合的唯一字符串表示）</returns>
        /// <exception cref="ArgumentNullException">参数为空时抛出</exception>
        /// <exception cref="ArgumentException">存在重复Key且不允许重复时抛出，默认允许重复</exception>
        public async Task<Dictionary<string, TSource>> ToDictAsync<TSource, TKeyItem>(
            List<TSource> list,
            Func<TSource, IEnumerable<TKeyItem>> keySelector,
            bool allowDuplicate = false)
        {
            // 参数校验
            if (list == null) throw new ArgumentNullException(nameof(list), "集合不能为空");
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector), "Key选择器不能为空");

            // 异步执行字典构建
            return await Task.Run(() =>
            {
                var dict = new Dictionary<string, TSource>();
                foreach (var item in list)
                {
                    var keyCollection = keySelector(item);
                    if (keyCollection == null)
                        throw new ArgumentException("Key集合不能为空");

                    // 将集合Key序列化为唯一字符串（顺序敏感，元素用逗号分隔）
                    string key = string.Join(",", keyCollection);

                    if (dict.ContainsKey(key))
                    {
                        if (allowDuplicate)
                        {
                            dict[key] = item;
                        }
                        else
                        {
                            throw new ArgumentException($"集合中存在重复的集合Key: {key}");
                        }
                    }
                    else
                    {
                        dict.Add(key, item);
                    }
                }
                return dict;
            });
        }
        #endregion
    }
}