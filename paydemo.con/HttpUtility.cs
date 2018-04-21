using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace paydemo.con
{
    /// <summary>
    /// http请求工具
    /// </summary>
    public class HttpUtility
    {
        /// <summary>
        /// 获取对指定url发送Get请求返回的内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetString(string url)
        {
            HttpMessageHandler handler = new HttpClientHandler();
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("CharSet", "utf-8");
                var task = client.GetAsync(url);
                task.Wait();
                using (var response = task.Result)
                {
                    response.Content.Headers.ContentType.CharSet = "utf-8";
                    return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
            }
        }

        
        /// <summary>
        /// 获取对指定url发送Post请求返回的内容
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <param name="postData">请求数据</param>
        /// <param name="cert"></param>
        /// <param name="contentType">默认 application/x-www-form-urlencoded,multipart/form-data,application/json</param>
        /// <returns>string</returns>
        public static string PostString(string url, string postData, string contentType = "application/x-www-form-urlencoded", X509Certificate2 cert = null)
        {
            HttpClientHandler handler = new HttpClientHandler();//所用的默认消息版本
            if (cert != null)
            {
                handler.ClientCertificates.Add(cert);
            }
            using (HttpClient client = new HttpClient(handler))//HttpClient
            {
                StringContent content = new StringContent(postData);//StringContent
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                //
                //content.Headers.ContentType.MediaType = "xxxx";

                content.Headers.ContentType.CharSet = "utf-8";
                content.Headers.ContentEncoding.Add("utf-8");
                var c= client.PostAsync(url, content);//PostAsync
                c.Wait();
                using (var response = c.Result)
                {
                    var r = response.Content.ReadAsStringAsync();
                    r.Wait();
                    var str = r.Result;
                    return str;
                }
            }
        }   
    }
}