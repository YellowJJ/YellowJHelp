using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using YellowJAutoInjection.Entry;

namespace YellowJHelp.IServer
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
        Task<string> HttpGetAsync(string url);
        /// <summary>
        /// 调用接口方法get
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="keyValues">请求头</param>
        /// <returns></returns>
        Task<string> HttpGetAsync(string url, List<KeyValueInfo<string, string>> keyValues);
        /// <summary>
        /// 调用接口方法Post
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="Jsoncontent">参数</param>
        /// <returns></returns>
        Task<string> HttPostAsync(string url, string Jsoncontent);
        /// <summary>
        /// 调用接口方法Post
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="Jsoncontent">参数</param>
        /// <param name="keyValues">请求头</param>
        /// <returns></returns>
        Task<string> HttPostAsync(string url, string Jsoncontent, List<KeyValueInfo<string, string>> keyValues);

        /// <summary>
        /// 调用接口方法Post（新增head头）
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="Jsoncontent">参数</param>
        /// <param name="webHeaderCollection">head</param>
        /// <returns></returns>
        [Obsolete("作废")]
        Task<string> HttHeadersPostAsync(string url, string Jsoncontent, WebHeaderCollection webHeaderCollection);


    }
}
