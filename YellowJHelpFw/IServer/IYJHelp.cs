
namespace YellowJHelpFw.IServer
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
        /// 文件流日志记录
        /// </summary>
        /// <param name="text">参数</param>
        /// <param name="address">新建文件名（地址）</param>
        void YellowJLog(string text, string address);


        /// <summary>
        /// string中是否包含某个值
        /// </summary>
        /// <param name="data">数据包</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        bool IsString(string data, string value);
    }

    /// <summary>
    /// 缓存类
    /// </summary>
    public interface ICache
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
}
