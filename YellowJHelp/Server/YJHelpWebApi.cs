using System.Net;
using System.Net.Http.Headers;
using System.Text;
using YellowJAutoInjection.Entry;
using YellowJHelp.IServer;

namespace YellowJHelp
{

    /// <summary>
    /// 接口调用方法
    /// </summary>
    [AutoInject(typeof(IYJHelpWebApi))]
    public class YJHelpWebApi: IYJHelpWebApi
    {
        /// <summary>
        /// 调用接口方法get
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public async Task<string> HttpGetAsync(string url)
        {
            // 创建 HttpClient 实例（最好使用 IHttpClientFactory 来管理 HttpClient 的生命周期，但在这里为了简单起见，我们直接实例化）
            using HttpClient client = new HttpClient();
            HttpRequestHeaders httpHeaders;
            // 设置请求头
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // 发送 请求并获取响应
            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {

            }
            // 确保响应成功
            // 读取响应内容
            string responseBody = await response.Content.ReadAsStringAsync();
            // 处理响应内容（这里只是简单地打印出来）
            return responseBody;
        }

        /// <summary>
        /// 调用接口方法get
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="keyValues">请求头</param>
        /// <returns></returns>
        public async Task<string> HttpGetAsync(string url, List<KeyValueInfo<string, string>> keyValues)
        {

            // 创建 HttpClient 实例（最好使用 IHttpClientFactory 来管理 HttpClient 的生命周期，但在这里为了简单起见，我们直接实例化）
            using HttpClient client = new HttpClient();
            HttpRequestHeaders httpHeaders;
            // 设置请求头
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            foreach (var item in keyValues)
            {
                client.DefaultRequestHeaders.Add(item.Key, item.Value); // 自定义头信息
            }
            // 发送 请求并获取响应
            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {

            }
            // 确保响应成功
            // 读取响应内容
            string responseBody = await response.Content.ReadAsStringAsync();
            // 处理响应内容
            return responseBody;
        }

        /// <summary>
        /// 调用接口方法Post
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="Jsoncontent">参数</param>
        /// <returns></returns>
        public async Task<string> HttPostAsync(string url, string Jsoncontent)
        {
            using HttpClient client = new HttpClient();
            HttpRequestHeaders httpHeaders;
            var content = new StringContent(Jsoncontent, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PostAsync(url, content);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        /// <summary>
        /// 调用接口方法Post
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="Jsoncontent">参数</param>
        /// <param name="keyValues">请求头</param>
        /// <returns></returns>
        public async Task<string> HttPostAsync(string url, string Jsoncontent, List<KeyValueInfo<string, string>> keyValues)
        {
            using HttpClient client = new HttpClient();
            HttpRequestHeaders httpHeaders;
            var content = new StringContent(Jsoncontent, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            foreach (var item in keyValues)
            {
                client.DefaultRequestHeaders.Add(item.Key, item.Value); // 自定义头信息
            }
            HttpResponseMessage response = await client.PostAsync(url, content);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        /// <summary>
        /// 调用接口方法Post（新增head头）
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="Jsoncontent">参数</param>
        /// <param name="webHeaderCollection">head</param>
        /// <returns></returns>
        [Obsolete("作废")]
        public async Task<string> HttHeadersPostAsync(string url, string Jsoncontent, WebHeaderCollection webHeaderCollection)
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
            Stream dataStream = await request.GetRequestStreamAsync();
            await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //定义response为前面的request响应
            WebResponse response =await request.GetResponseAsync();

            //获取相应的状态代码
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            //定义response字符流
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string data =await reader.ReadToEndAsync();//读取所有

            return data;
        }


    }
}
