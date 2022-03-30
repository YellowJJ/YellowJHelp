using System.Net;

namespace YellowJHelpFw.IServer
{
    /// <summary>
    /// 接口调用方法
    /// </summary>
    public interface IYJHelpWebApi
    {
        /// <summary>
        /// 调用接口方法get
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        string HttpGet(string url);

        /// <summary>
        /// 调用接口方法Post
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="Jsoncontent">参数</param>
        /// <returns></returns>
        string HttPost(string url, string Jsoncontent);


        /// <summary>
        /// 调用接口方法Post（新增head头）
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="Jsoncontent">参数</param>
        /// <param name="webHeaderCollection">head</param>
        /// <returns></returns>
        string HttHeadersPost(string url, string Jsoncontent, WebHeaderCollection webHeaderCollection);


    }
}
