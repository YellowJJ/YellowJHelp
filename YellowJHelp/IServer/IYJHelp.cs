
using Microsoft.AspNetCore.Http;

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
        string MD5Encrypt(string strText, bool IsLower);
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data">需要加密的值</param>
        /// <param name="KEY_64">密钥长度8位</param>
        /// <param name="IV_64">密钥长度8位</param>
        /// <returns></returns>
        string Encode(string data, string KEY_64, string IV_64);
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data">需要加密的值</param>
        /// <param name="KEY_64">密钥长度8位</param>
        /// <param name="IV_64">密钥长度8位</param>
        /// <returns></returns>
        string Decode(string data, string KEY_64, string IV_64);
        #endregion

        /// <summary>
        /// 缓存类
        /// </summary>
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
        void YellowJLog(string text, string address);

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
        void SessionAdd(HttpContext ctx, string strSessionName, string strValue);

        /// <summary>
        /// 获取Session
        /// </summary>
        /// <param name="ctx">Microsoft.AspNetCore.Http</param>
        /// <param name="strSessionName">Session对象名称</param>
        string SessionGet(HttpContext ctx, string strSessionName);
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
    }
}
