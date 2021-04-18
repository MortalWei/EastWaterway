using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;

namespace EastWaterway
{
    public class WebApiClient
    {
        public HttpClient Client { get; set; }

        public WebApiClient()
        {
            var handle = new HttpClientHandler() { UseCookies = true };
            Client = new HttpClient(handle);
        }


        public string Login(string url, string userName, string userPWD)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("ecmsfrom", ""),
                new KeyValuePair<string, string>("enews", "login"),
                new KeyValuePair<string, string>("username", userName),
                new KeyValuePair<string, string>("password", userPWD),
                new KeyValuePair<string, string>("lifetime", "0"),
                new KeyValuePair<string, string>("登录", ""),
                new KeyValuePair<string, string>("ecmsfrom", "../member/cp/"),
            });
            var result = Client.PostAsync(url, content).Result;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return result.Content.ReadAsStringAsync().Result;
            return FuncConst.LoginErr;
        }

        /// <summary>
        /// 刷新我的发布
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string RefreshLately(string url)
        {
            var result = Client.GetAsync(url).Result;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return result.Content.ReadAsStringAsync().Result;
            return FuncConst.RefreshErr;
        }

        internal object Logout(string baseUrl)
        {
            var result = Client.GetAsync(baseUrl).Result;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return result.Content.ReadAsStringAsync().Result;
            return FuncConst.LogoutErr;
        }

        public string AddInfo(string url)
        {
            var result = Client.GetAsync(url).Result;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return result.Content.ReadAsStringAsync().Result;
            return FuncConst.AddErr;
        }

        public string SaveInfo(string url, JObject args)
        {
            var list = new List<KeyValuePair<string, string>>();
            var argsProperties = args.Properties();
            foreach (var item in argsProperties)
            {
                list.Add(new KeyValuePair<string, string>(item.Name, item.Value.ToString()));
            }
            var content = new FormUrlEncodedContent(list);
            var result = Client.PostAsync(url, content).Result;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return result.Content.ReadAsStringAsync().Result;
            return FuncConst.AddErr;
        }
    }
}
