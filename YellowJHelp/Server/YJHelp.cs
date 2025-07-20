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
        /// <param name="strText">要加密的字符串</param>
        /// <param name="isLower">是否以小写方式返回（true：小写，false：大写）</param>
        /// <returns>加密后的32位MD5字符串</returns>
        /// <remarks>
        /// 1. 支持异步，避免主线程阻塞，适合Web和高并发场景。
        /// 2. 输入为空时返回空字符串，安全防护。
        /// 3. 结果可选大小写，默认补齐32位。
        /// 4. 使用using自动释放资源，防止内存泄漏。
        /// 5. 推荐仅用于数据校验、签名等场景，不建议用于高安全场景的密码存储。
        /// </remarks>
        public async Task<string> MD5EncryptAsync(string strText, bool isLower)
        {
            // 异步执行，避免阻塞主线程
            return await Task.Run(() =>
            {
                // 输入校验，防止空字符串异常
                if (string.IsNullOrEmpty(strText))
                    return string.Empty;

                // 创建MD5对象，using确保资源释放
                using (var md5 = MD5.Create())
                {
                    // 将字符串按UTF8编码转为字节数组
                    byte[] bytes = Encoding.UTF8.GetBytes(strText);
                    // 计算MD5哈希值
                    byte[] hashBytes = md5.ComputeHash(bytes);

                    // 构建32位MD5字符串
                    StringBuilder sb = new StringBuilder(32);
                    foreach (byte b in hashBytes)
                    {
                        // x2：每个字节转为两位16进制字符串
                        sb.Append(b.ToString("x2"));
                    }
                    // 补齐32位（理论上MD5总是32位，这里防御性处理）
                    string result = sb.ToString().PadLeft(32, '0');
                    // 按需返回大小写
                    return isLower ? result.ToLowerInvariant() : result.ToUpperInvariant();
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

        /// <summary>
        /// SHA256哈希加密（异步，返回64位小写十六进制字符串）
        /// </summary>
        /// <param name="text">要加密的字符串</param>
        /// <returns>SHA256哈希值（64位小写十六进制字符串）</returns>
        /// <remarks>
        /// 1. 适用于数据签名、完整性校验等场景。
        /// 2. 不可逆，仅用于校验和签名，不可解密。
        /// 3. 异步实现，适合Web和高并发场景。
        /// </remarks>
        public async Task<string> YJSha256Async(string text)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(text))
                    return string.Empty;
                using (var sha256 = SHA256.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(text);
                    byte[] hash = sha256.ComputeHash(bytes);
                    StringBuilder sb = new StringBuilder(64);
                    foreach (byte b in hash)
                        sb.Append(b.ToString("x2"));
                    return sb.ToString();
                }
            });
        }

        /// <summary>
        /// AES加密（异步，CBC模式，PKCS7填充，返回Base64字符串）
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="key">密钥（16/24/32字节，建议32字节）</param>
        /// <param name="iv">初始向量（16字节）</param>
        /// <returns>加密后的Base64字符串</returns>
        /// <remarks>
        /// 1. 支持AES-128/192/256，key长度分别为16/24/32字节。
        /// 2. IV必须为16字节，建议随机生成并安全保存。
        /// 3. 适用于敏感数据加密传输、存储等场景。
        /// 4. 异步实现，适合Web和高并发场景。
        /// </remarks>
        public async Task<string> YJAesEncryptAsync(string plainText, string key, string iv)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(iv))
                    throw new ArgumentException("参数无效，明文、密钥和IV不能为空");
                if (key.Length != 16 && key.Length != 24 && key.Length != 32)
                    throw new ArgumentException("密钥长度必须为16/24/32字节");
                if (iv.Length != 16)
                    throw new ArgumentException("IV长度必须为16字节");

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = Encoding.UTF8.GetBytes(iv);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var ms = new MemoryStream())
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs, Encoding.UTF8))
                    {
                        sw.Write(plainText);
                        sw.Flush();
                        cs.FlushFinalBlock();
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            });
        }

        /// <summary>
        /// AES解密（异步，CBC模式，PKCS7填充，输入Base64字符串）
        /// </summary>
        /// <param name="cipherText">密文（Base64字符串）</param>
        /// <param name="key">密钥（16/24/32字节，建议32字节）</param>
        /// <param name="iv">初始向量（16字节）</param>
        /// <returns>解密后的明文</returns>
        /// <remarks>
        /// 1. 密钥和IV需与加密时一致。
        /// 2. 解密失败返回空字符串。
        /// 3. 异步实现，适合Web和高并发场景。
        /// </remarks>
        public async Task<string> YJAesDecryptAsync(string cipherText, string key, string iv)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(cipherText) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(iv))
                    throw new ArgumentException("参数无效，密文、密钥和IV不能为空");
                if (key.Length != 16 && key.Length != 24 && key.Length != 32)
                    throw new ArgumentException("密钥长度必须为16/24/32字节");
                if (iv.Length != 16)
                    throw new ArgumentException("IV长度必须为16字节");

                try
                {
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = Encoding.UTF8.GetBytes(key);
                        aes.IV = Encoding.UTF8.GetBytes(iv);
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;

                        byte[] cipherBytes = Convert.FromBase64String(cipherText);
                        using (var ms = new MemoryStream(cipherBytes))
                        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        using (var sr = new StreamReader(cs, Encoding.UTF8))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
                catch
                {
                    return string.Empty;
                }
            });
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
        /// 日志（支持自动分目录、按天分文件、UTF-8带BOM编码，便于扩展）
        /// </summary>
        /// <param name="text">日志内容</param>
        /// <param name="address">日志分类/相对目录（如"System"、"业务模块"）</param>
        /// <param name="logLevel">日志级别（可选，默认Info）</param>
        /// <param name="customFileName">自定义文件名（可选，默认按日期）</param>
        /// <returns></returns>
        /// <remarks>
        /// 1. 自动在应用根目录下创建 YellowJ_Logs/分类目录，便于日志分模块管理。
        /// 2. 日志文件按天分割，便于归档和查找。
        /// 3. 支持自定义日志级别（如Info、Warn、Error等），便于后续扩展。
        /// 4. 支持自定义文件名，满足特殊场景需求。
        /// 5. 使用UTF-8带BOM编码，兼容主流编辑器。
        /// 6. 异步写入，避免阻塞主线程。
        /// </remarks>
        public async Task YellowJLogAsync(
            string text,
            string address,
            string logLevel = "Info",
            string customFileName = null)
        {
            // 1. 构建日志目录（如：根目录/YellowJ_Logs/业务模块/）
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string logDir = Path.Combine(basePath, "YellowJ_Logs", address ?? "Default");
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            // 2. 构建日志文件名（如：20250720.txt 或自定义名）
            string fileName = customFileName ?? $"{DateTime.Now:yyyyMMdd}.txt";
            string fileFullName = Path.Combine(logDir, fileName);

            // 3. 日志内容格式化（含时间、级别、内容）
            string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {text}";

            // 4. 异步写入日志，UTF-8带BOM编码，追加模式
            using (var stream = new FileStream(fileFullName, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(true)))
            {
                await writer.WriteLineAsync(logLine).ConfigureAwait(false);
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
        /// 分配数据（单线程版）
        /// </summary>
        /// <param name="yAllocations">分配集合（资源池，每项Key唯一，RemQty为可分配剩余量）</param>
        /// <param name="yAllocations1">被分配集合（需求池，每项Key唯一，AQty为已分配量）</param>
        /// <returns>
        /// List[0]：分配集合（分配后剩余资源）
        /// List[1]：被分配集合（分配后已分配量）
        /// List[2]：分配明细（每次分配的详细记录）
        /// </returns>
        /// <remarks>
        /// 1. 以Key为分组依据，将资源池与需求池按Key一一对应。
        /// 2. 需求池每项依次尝试从资源池分配，优先分配RemQty充足的资源。
        /// 3. 分配优先满足需求，资源池RemQty不足时可多次分配直至满足或资源耗尽。
        /// 4. 分配明细记录每次分配的来源、数量、目标等，便于追溯。
        /// 5. 返回分配后资源池、需求池和分配明细，便于后续业务处理。
        /// </remarks>
        public List<List<YAllocationInfo>> YAlloctionlist(List<YAllocationInfo> yAllocations, List<YAllocationInfo> yAllocations1)
        {
            // 判空保护，避免空引用异常
            if (yAllocations == null || yAllocations1 == null)
                return new List<List<YAllocationInfo>>();

            // 结果容器：0-资源池，1-需求池，2-分配明细
            List<List<YAllocationInfo>> yAllocationInfos = new();
            List<YAllocationInfo> resultList = new();

            // 以Key分组，便于快速查找资源池中同Key的所有资源
            var allocationDict = yAllocations
                .GroupBy(a => a.Key)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 遍历需求池，每个需求项尝试分配资源
            foreach (var demand in yAllocations1)
            {
                // 计算当前需求剩余未分配量
                decimal needQty = demand.Qty - demand.AQty;
                if (needQty <= 0) continue; // 已满足，无需分配

                // 查找资源池中Key相同的资源列表
                if (allocationDict.TryGetValue(demand.Key, out var resourceList))
                {
                    foreach (var resource in resourceList)
                    {
                        // 跳过已分配完的资源
                        if (resource.RemQty <= 0) continue;

                        // 构建分配明细对象
                        YAllocationInfo allocationDetail = new()
                        {
                            Number = demand.Number,   // 需求编号
                            Key = demand.Key,         // 分配标识
                            Qty = demand.Qty,         // 需求总量
                            FPNumber = resource.Number // 分配来源编号
                        };

                        if (resource.RemQty >= needQty)
                        {
                            // 资源充足，全部满足当前需求
                            demand.AQty += needQty;         // 需求已分配量增加
                            resource.RemQty -= needQty;     // 资源剩余量减少
                            allocationDetail.AQty = needQty;// 本次分配量
                            resultList.Add(allocationDetail);
                            break; // 当前需求已满足，跳出资源分配循环
                        }
                        else
                        {
                            // 资源不足，分配所有剩余资源
                            demand.AQty += resource.RemQty;     // 需求已分配量增加
                            needQty -= resource.RemQty;         // 需求剩余量减少
                            allocationDetail.AQty = resource.RemQty; // 本次分配量
                            resource.RemQty = 0;                // 资源耗尽
                            resultList.Add(allocationDetail);
                            // 继续尝试下一个资源
                        }
                    }
                }
            }

            // 返回分配后资源池、需求池、分配明细
            yAllocationInfos.Add(yAllocations);
            yAllocationInfos.Add(yAllocations1);
            yAllocationInfos.Add(resultList);
            return yAllocationInfos;
        }

        /// <summary>
        /// 分配数据-多线程优化版
        /// </summary>
        /// <param name="yAllocations">分配集合（资源池，每项Key唯一，RemQty为可分配剩余量）</param>
        /// <param name="yAllocations1">被分配集合（需求池，每项Key唯一，AQty为已分配量）</param>
        /// <returns>
        /// List[0]：分配集合（分配后剩余资源）
        /// List[1]：被分配集合（分配后已分配量）
        /// List[2]：分配明细（每次分配的详细记录）
        /// </returns>
        /// <remarks>
        /// 1. 按Key分组，将资源池和需求池的同Key分组配对，形成并行分配任务。
        /// 2. 每个Key分组独立分配，互不影响，适合大数据量、Key分布均匀场景。
        /// 3. 内部调用单线程分配方法，保证每组分配逻辑一致。
        /// 4. 结果采用线程安全集合（ConcurrentBag）收集，避免多线程冲突。
        /// 5. 返回结构与单线程版一致，便于统一处理。
        /// 6. 注意：如需全局唯一性或跨Key分配，需单线程或加锁处理。
        /// </remarks>
        public List<List<YAllocationInfo>> YAlloctionlistThred(List<YAllocationInfo> yAllocations, List<YAllocationInfo> yAllocations1)
        {
            // 判空保护
            if (yAllocations == null || yAllocations1 == null)
                return new List<List<YAllocationInfo>>();

            // 1. 按Key分组，构建资源池和需求池的Key->List映射
            var resourceGroups = yAllocations.GroupBy(a => a.Key).ToDictionary(g => g.Key, g => g.ToList());
            var demandGroups = yAllocations1.GroupBy(a => a.Key).ToDictionary(g => g.Key, g => g.ToList());

            // 2. 构建并行任务队列，每个Key一组
            var keyValuePairs = new List<KeyValueInfo<List<YAllocationInfo>, List<YAllocationInfo>>>();
            foreach (var kv in resourceGroups)
            {
                if (demandGroups.TryGetValue(kv.Key, out var demandList))
                {
                    // 深拷贝，避免多线程下原集合被修改（如有副作用需加此步）
                    keyValuePairs.Add(new KeyValueInfo<List<YAllocationInfo>, List<YAllocationInfo>>
                    {
                        Key = kv.Value.DeepClone(),
                        Value = demandList.DeepClone()
                    });
                }
            }

            // 3. 线程安全结果收集器，分别存放资源池、需求池、分配明细
            var resultResource = new ConcurrentBag<YAllocationInfo>();
            var resultDemand = new ConcurrentBag<YAllocationInfo>();
            var resultDetail = new ConcurrentBag<YAllocationInfo>();

            // 4. 并行处理每个Key分组
            Parallel.ForEach(keyValuePairs, new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) }, pair =>
            {
                // 调用单线程分配方法，保证分配逻辑一致
                var ret = YAlloctionlist(pair.Key, pair.Value);
                // 合并每组结果到全局结果
                foreach (var r in ret[0]) resultResource.Add(r);
                foreach (var r in ret[1]) resultDemand.Add(r);
                foreach (var r in ret[2]) resultDetail.Add(r);
            });

            // 5. 返回结构与单线程版一致
            return new List<List<YAllocationInfo>>
            {
                resultResource.ToList(),
                resultDemand.ToList(),
                resultDetail.ToList()
            };
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
        /// 高性能对象映射器
        /// </summary>
        /// <returns></returns>
        //public FastMapper Mapper()
        //{
        //    List<string> sd = null;
        //    var sdfd=FastMapper.Adapt(this, sd);
        //    var df=sd.Adapt<List<string>>();
        //    return new FastMapper();
        //}
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
        /// 根据指定字段去重，生成全新且不扰动原集合的去重List
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
        /// 将List集合根据指定键选择器生成字典，便于快速查找
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
        /// 将List集合根据指定集合Key选择器生成字典，支持Key为集合类型（如List&lt;string&gt;），便于快速查找
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