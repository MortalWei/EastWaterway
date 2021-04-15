using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
            else
                return "登陆失败";
        }

        public string AddInfo(string url)
        {
            var result = Client.GetAsync(url).Result;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return result.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return "添加失败";
            }
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
            {
                return result.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return "添加失败";
            }
        }
    }
}
