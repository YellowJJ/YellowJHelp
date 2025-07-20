
using Microsoft.AspNetCore.Http;
using YellowJHelp.Entry;

namespace YellowJHelp.IServer
{
    /// <summary>
    /// 通用帮助方法
    /// </summary>
    public interface IYJHelp
    {
        /// <summary>
        /// 消息注明
        /// </summary>
        /// <returns></returns>
        string message();
        #region 加解密
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="strText">要加密字符串</param>
        /// <param name="IsLower">是否以小写方式返回</param>
        Task<string> MD5EncryptAsync(string strText, bool IsLower);
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data">需要加密的值</param>
        /// <param name="KEY_64">密钥长度8位</param>
        /// <param name="IV_64">密钥长度8位</param>
        /// <returns></returns>
        Task<string> EncodeAsync(string data, string KEY_64, string IV_64);
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data">需要加密的值</param>
        /// <param name="KEY_64">密钥长度8位</param>
        /// <param name="IV_64">密钥长度8位</param>
        /// <returns></returns>
        Task<string> DecodeAsync(string data, string KEY_64, string IV_64);
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
        Task<string> YJSha256Async(string text);
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
        Task<string> YJAesEncryptAsync(string plainText, string key, string iv);
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
        Task<string> YJAesDecryptAsync(string cipherText, string key, string iv);
        #endregion

        /// <summary>
        /// 缓存类
        /// </summary>
        [Obsolete("请使用的新的接口：IYJHelpCache")]
        interface ICache 
        {
            /// <summary>
            /// 添加缓存
            /// </summary>
            /// <param name="key">键</param>
            /// <param name="value">值</param>
            void Add(string key, object value);
            /// <summary>
            /// 获取缓存
            /// </summary>
            /// <typeparam name="T">接收实体</typeparam>
            /// <param name="key">键</param>
            /// <returns></returns>
            T Get<T>(string key);
            /// <summary>
            /// 判断缓存是否存在
            /// </summary>
            /// <param name="key">键</param>
            /// <returns></returns>
            bool Exsits(string key);
            /// <summary>
            /// 删除对应缓存
            /// </summary>
            /// <param name="key">键</param>
            void Clear(string key);
            /// <summary>
            /// 删除所有缓存
            /// </summary>
            void Clear();
        };
        /// <summary>
        /// 文件流日志记录
        /// </summary>
        /// <param name="text">参数</param>
        /// <param name="address">新建文件名（地址）</param>
        Task YellowJLogAsync(string text, string address, string logLevel = "Info",
            string customFileName = null);

        #region --cookie--
        /// <summary>
        /// 设置本地cookie
        /// </summary>
        /// <param name="ctx">Microsoft.AspNetCore.Http</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>  
        /// <param name="minutes">过期时长，单位：分钟</param>      
        void SetCookies(HttpContext ctx, string key, string value, int minutes = 30);

        /// <summary>
        /// 删除指定的cookie
        /// </summary>
        /// <param name="ctx">Microsoft.AspNetCore.Http</param>
        /// <param name="key">键</param>
        void DeleteCookies(HttpContext ctx, string key);

        /// <summary>
        /// 获取cookies
        /// </summary>
        /// <param name="ctx">Microsoft.AspNetCore.Http</param>
        /// <param name="key">键</param>
        /// <returns>返回对应的值</returns>
        string GetCookies(HttpContext ctx, string key);
        #endregion
        #region --Session--
        /// <summary>
        /// 添加Session
        /// </summary>
        /// <param name="ctx">Microsoft.AspNetCore.Http</param>
        /// <param name="strSessionName">Session对象名称</param>
        /// <param name="strValue">Session值</param>
        void SessionAdd(HttpContext ctx, string strSessionName, byte[] strValue);

        /// <summary>
        /// 获取Session
        /// </summary>
        /// <param name="ctx">Microsoft.AspNetCore.Http</param>
        /// <param name="cancellationToken"></param>
        Task SessionGet(HttpContext ctx, CancellationToken cancellationToken);
        /// <summary>
        /// 删除Session
        /// </summary>
        /// <param name="ctx">Microsoft.AspNetCore.Http</param>
        /// <param name="strSessionName">Session对象名称</param>
        void SessionDel(HttpContext ctx, string strSessionName);

        #endregion

        /// <summary>
        /// string中是否包含某个值
        /// </summary>
        /// <param name="data">数据包</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        bool IsString(string data, string value);

        /// <summary>
        /// 分配数据
        /// </summary>
        /// <param name="yAllocations">分配集合</param>
        /// <param name="yAllocations1">被分配集合</param>
        /// <returns>返回分配集合（剩余），被分配集合（已分），分配结果详情</returns>
        List<List<YAllocationInfo>> YAlloctionlist(List<YAllocationInfo> yAllocations, List<YAllocationInfo> yAllocations1);
        /// <summary>
        /// 分配数据-多线程
        /// </summary>
        /// <param name="yAllocations">分配集合</param>
        /// <param name="yAllocations1">被分配集合</param>
        /// <returns>返回分配集合（剩余），被分配集合（已分），分配结果详情</returns>
        List<List<YAllocationInfo>> YAlloctionlistThred(List<YAllocationInfo> yAllocations, List<YAllocationInfo> yAllocations1);
        /// <summary>
        /// 生成雪花ID
        /// </summary>
        /// <param name="workerId">工作者的标识</param>
        /// <returns></returns>
        long NextId(long workerId);
        /// <summary>
        /// 高性能对象映射器（Mapster风格，支持属性名匹配、类型兼容、集合映射、嵌套对象、缓存优化，兼容netstandard2.0）
        /// </summary>
        /// <returns></returns>
        //FastMapper Mapper();


        /// <summary>
        /// 日期是否在目标年月内
        /// </summary>
        /// <param name="date">输入日期</param>
        /// <param name="targetDate">目标日期</param>
        /// <returns></returns>
        bool IsDateInTargetMonth(DateTime date, DateTime targetDate);
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
        Task<Dictionary<TKey, TSource>> ToDictAsync<TSource, TKey>(
            List<TSource> list,
            Func<TSource, TKey> keySelector,
            bool allowDuplicate = false);

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
        Task<Dictionary<string, TSource>> ToDictAsync<TSource, TKeyItem>(
            List<TSource> list,
            Func<TSource, IEnumerable<TKeyItem>> keySelector,
            bool allowDuplicate = false);
        #region --泛型方法--
        /// <summary>
        /// 对象副本
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>将对象复制成全新的对象，且不互相影响</returns>
        T? Copy<T>(T data);
        /// <summary>
        /// YJ版本：合并两个集合的函数-不允许有重复项
        /// </summary>
        /// <param name="list1">第一个集合</param>
        /// <param name="list2">第二个集合</param>
        /// <returns>返回结果</returns>
        public List<T> YJMerge<T>(List<T> list1, List<T> list2);
        /// <summary>
        /// 集合去重
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public List<T> Distinct<T>(List<T> list);
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
        Task<List<T>> DistinctAsync<T, TKey>(List<T> list, Func<T, TKey> keySelector);
        #endregion
    }
}
