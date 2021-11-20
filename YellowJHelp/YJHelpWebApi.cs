using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace YellowJHelp
{
    public class YJHelpWebApi
    {
        /// <summary>
        /// 调用接口方法get
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {

            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Accept-Encoding"] = "gzip,deflase";
            request.AutomaticDecompression = DecompressionMethods.GZip;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 调用接口方法Post
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="Jsoncontent">参数</param>
        /// <returns></returns>
        public static string HttPost(string url, string Jsoncontent)
        {

            //定义request并设置request的路径
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "post";
            //设置参数的编码格式，解决中文乱码
            byte[] byteArray = Encoding.UTF8.GetBytes(Jsoncontent);

            //设置request的MIME类型及内容长度
            request.ContentType = "application/json";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            //打开request字符流
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //定义response为前面的request响应
            WebResponse response = request.GetResponse();

            //获取相应的状态代码
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            //定义response字符流
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string data = reader.ReadToEnd();//读取所有

            return data;
        }


        /// <summary>
        /// 调用接口方法Post（新增head头）
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="Jsoncontent">参数</param>
        /// <param name="webHeaderCollection">head</param>
        /// <returns></returns>
        public static string HttHeadersPost(string url, string Jsoncontent, WebHeaderCollection webHeaderCollection)
        {

            //定义request并设置request的路径
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "post";
            //设置参数的编码格式，解决中文乱码
            byte[] byteArray = Encoding.UTF8.GetBytes(Jsoncontent);

            //设置request的MIME类型及内容长度
            request.ContentType = "application/json";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            request.Headers = webHeaderCollection;
            //打开request字符流
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //定义response为前面的request响应
            WebResponse response = request.GetResponse();

            //获取相应的状态代码
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            //定义response字符流
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string data = reader.ReadToEnd();//读取所有

            return data;
        }


    }
}
