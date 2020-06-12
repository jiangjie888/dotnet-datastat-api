using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DataStat.WebCore.Common
{
    public class HTTPClientHelper
    {
        public enum HttpVerb
        {
            GET,
            POST,
            HEAD
        }
        public string Accept { get; set; }
        public Encoding DefaultEncoding { get; set; }
        public string DefaultLanguage { get; set; }
        public int EndPoint { get; set; }
        //public List<HttpUploadingFile> Files { get; }
        public bool KeepContext { get; set; }
        public Dictionary<string, string> PostingData { get; set; }

        public Object PostingObj { get; set; }

        public WebHeaderCollection ResponseHeaders { get; }     
        public int StartPoint { get; set; }
        public int Timeout { get; set; }
        public string Url { get; set; }
        public string UserAgent { get; set; }
        public HttpVerb Verb { get; set; }

        //List<KeyValuePair<string, string>> paramArray
        //private readonly DiscoveryHttpClientHandler _handler;
        private readonly IHttpClientFactory _httpClientFactory;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //public HttpClient Client { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public HTTPClientHelper(IHttpClientFactory httpClientFactory)
        {
            //object factory = ServiceProvider.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));
            //Microsoft.AspNetCore.Http.HttpContext context = ((Microsoft.AspNetCore.Http.HttpContextAccessor)factory).HttpContext;

            this.PostingData = new Dictionary<string, string>();
            //var client = IocManager.Instance.Resolve<IDiscoveryClient>();
            // _handler = new DiscoveryHttpClientHandler(client);
            _httpClientFactory = httpClientFactory;
            //_httpContextAccessor = IocManager.Instance.Resolve<IHttpContextAccessor>();
            //httpContextAccessor = _httpContextAccessor;
            //var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };
            //HttpClient = new HttpClient(handler);
        }
        //public HTTPClientHelper(IHttpContextAccessor httpContextAccessor)
        //{
        //    _httpContextAccessor = httpContextAccessor;
        //}

        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            string token = "";
            if (DataStat.WebCore.Common.HttpContext.Current.Request.Headers == null) return token;
            string authorizationStr = DataStat.WebCore.Common.HttpContext.Current.Request.Headers["Authorization"].ToString() ?? "";
            token = string.IsNullOrWhiteSpace(authorizationStr) == false ? authorizationStr.Substring("Bearer ".Length).Trim() : "";
            return token;
        }
       
        public string GetResponse(string jwttoken = null)
        {
            string result = "";
            jwttoken = jwttoken ?? GetToken();

            var client = _httpClientFactory.CreateClient();

            //using (var client = new HttpClient(_handler))
            //{
                string uri = this.Url + "?" + BuildParam();
                if (!string.IsNullOrEmpty(jwttoken)) client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwttoken);

                var response = client.GetAsync(uri).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("远程请求失败：" + uri + "！ StatusCode: " + response.StatusCode + ", ReasonPhrase: " + response.ReasonPhrase);
                }
                else
                {
                    Stream myResponseStream = response.Content.ReadAsStreamAsync().Result;
                    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                    result = myStreamReader.ReadToEnd();
                    myStreamReader.Close();
                    myResponseStream.Close();
                }
            //}
            return result;
        }

        public async Task<string> DeleteResponse(string jwttoken = null)
        {
            string result = "";
            jwttoken = jwttoken ?? GetToken();

            var client = _httpClientFactory.CreateClient();
            //using (var client = new HttpClient(_handler))
            //{
                string uri = this.Url + "?" + BuildParam();
                if (!string.IsNullOrEmpty(jwttoken)) client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwttoken);

                var response = await client.DeleteAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("远程请求失败：" + uri + "！ StatusCode: " + response.StatusCode + ", ReasonPhrase: " + response.ReasonPhrase);
                }
                else
                {
                    Stream myResponseStream = response.Content.ReadAsStreamAsync().Result;
                    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                    result = myStreamReader.ReadToEnd();
                    myStreamReader.Close();
                    myResponseStream.Close();
                }
            //}
            return result;
        }

        public string GetResponseBySimple(string jwttoken = null)
        {
            string result = "";
            jwttoken = jwttoken ?? GetToken();

            var client = _httpClientFactory.CreateClient();

            //using (var client = new HttpClient(_handler))
            //{
            string uri = string.IsNullOrEmpty(BuildParam())==false ? this.Url + "?" + BuildParam(): this.Url;
            if (!string.IsNullOrEmpty(jwttoken)) client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwttoken);

            result = client.GetStringAsync(uri).Result;
            //}
            return result;
        }

        public async Task<string> HttpPostRequestAsync(string jwttoken= null)
        {
            //string ContentType = "application/x-www-form-urlencoded"
            string result = "";
            jwttoken = jwttoken ?? GetToken();

            try
            {
                var client = _httpClientFactory.CreateClient();
                //using (var client = new HttpClient(_handler))
                //{
                //client.BaseAddress = new Uri(_apirul);
                Uri uri = new Uri(Url);

                    //client.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (compatible; Baiduspider/2.0; +http://www.baidu.com/search/spider.html)");
                    //client.DefaultRequestHeaders.Add("Accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*\/*;q=0.8");
                    if(!string.IsNullOrEmpty(jwttoken)) client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwttoken);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP POST
                    StringContent requestContent;
                    if (this.PostingObj != null)
                    {
                        requestContent = new StringContent(JsonConvert.SerializeObject(this.PostingObj, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json");
                    }
                    else
                    {
                        requestContent = new StringContent(JsonConvert.SerializeObject(this.PostingData, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json");
                        
                    }
                    var response = client.PostAsync(uri, requestContent).Result;

                    //response.EnsureSuccessStatusCode();//请求错误抛出异常,确保成功
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("远程请求失败：" + uri + "！ StatusCode: " + response.StatusCode + ", ReasonPhrase: " + response.ReasonPhrase);
                    }
                    result = await response.Content.ReadAsStringAsync();


                    //var data = Encoding.ASCII.GetBytes(postData);
                    //HttpResponseMessage message = null;
                    //using (Stream dataStream = new MemoryStream(data ?? new byte[0]))
                    //{
                    //    using (HttpContent content = new StreamContent(dataStream))
                    //    {
                    //        content.Headers.Add("Content-Type", "application/json");
                    //        var task = http.PostAsync(Url, content);
                    //        message = task.Result;
                    //    }
                    //}

                //}
            }
            catch (Exception ex)
            {
                throw new Exception("远程请求异常：" + ex.Message);
            }
            return result;
        }




        public async Task<string> HttpPutRequestAsync(string jwttoken = null)
        {
            //string ContentType = "application/x-www-form-urlencoded"
            string result = "";
            jwttoken = jwttoken ?? GetToken();

            try
            {
                var client = _httpClientFactory.CreateClient();
                //using (var client = new HttpClient(_handler))
                //{
                    //client.BaseAddress = new Uri(_apirul);
                    Uri uri = new Uri(Url);

                    //client.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (compatible; Baiduspider/2.0; +http://www.baidu.com/search/spider.html)");
                    //client.DefaultRequestHeaders.Add("Accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*\/*;q=0.8");
                    if (!string.IsNullOrEmpty(jwttoken)) client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwttoken);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP POST
                    StringContent requestContent;
                    if (this.PostingObj != null)
                    {
                        requestContent = new StringContent(JsonConvert.SerializeObject(this.PostingObj, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json");
                    }
                    else
                    {
                        requestContent = new StringContent(JsonConvert.SerializeObject(this.PostingData, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), Encoding.UTF8, "application/json");

                    }
                    var response = client.PutAsync(uri, requestContent).Result;

                    //response.EnsureSuccessStatusCode();//请求错误抛出异常,确保成功
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("远程请求失败：" + uri + "！ StatusCode: " + response.StatusCode + ", ReasonPhrase: " + response.ReasonPhrase);
                    }
                    result = await response.Content.ReadAsStringAsync();


                    //var data = Encoding.ASCII.GetBytes(postData);
                    //HttpResponseMessage message = null;
                    //using (Stream dataStream = new MemoryStream(data ?? new byte[0]))
                    //{
                    //    using (HttpContent content = new StreamContent(dataStream))
                    //    {
                    //        content.Headers.Add("Content-Type", "application/json");
                    //        var task = http.PostAsync(Url, content);
                    //        message = task.Result;
                    //    }
                    //}

                //}
            }
            catch (Exception ex)
            {
                throw new Exception("远程请求异常：" + ex.Message);
            }
            return result;
        }




        private string Encode(string content, Encoding encode = null)
        {
            if (encode == null) return content;

            return System.Web.HttpUtility.UrlEncode(content, Encoding.UTF8);

        }

        private string BuildParam(Encoding encode = null)
        {
            var paramArray = this.PostingData;
            string url = "";

            if (encode == null) encode = Encoding.UTF8;

            if (paramArray != null && paramArray.Count > 0)
            {
                var parms = "";
                foreach (var item in paramArray)
                {
                    parms += string.Format("{0}={1}&", Encode(item.Key, encode), Encode(item.Value, encode));
                }
                if (parms != "")
                {
                    parms = parms.TrimEnd('&');
                }
                url += parms;

            }
            return url;
        }


        //private object BuildParamToObj()
        //{
        //    //"{\"unIsRead\":false,\"noticeId\":\"08d5b403-437f-ba49-ba7b-40a2a15768ca\",\"userId\":\"08d5b09c-199c-04ca-449d-1c550d3157ef\"}"
        //    object result = null;
        //    var paramArray = this.PostingData;
        //    if (paramArray != null && paramArray.Count > 0)
        //    {
        //        foreach (var item in paramArray)
        //        {
        //            result.GetType().
        //        }
        //        if (parms != "")
        //        {
        //            parms = parms.TrimEnd('&');
        //        }
        //        url += parms;

        //    }
        //    return result;
        //}
    }
}