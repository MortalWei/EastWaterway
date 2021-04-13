using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EastWaterway
{
    public class HttpHelper
    {
        public static CookieContainer Container;

        public static Tuple<HttpStatusCode, string> GetApi(string baseUrl, JObject cookie, string args = "")
        {
            var client = new RestSharpClient(baseUrl);
            var request = string.IsNullOrEmpty(args) ? new RestRequest("", Method.GET) : new RestRequest($"/{args}", Method.GET);

            var properties = cookie.Properties();
            foreach (var item in properties)
            {
                request.AddCookie(item.Name, item.Value.ToString());
            }

            var response = client.Execute(request: request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new Tuple<HttpStatusCode, string>(response.StatusCode, response.ErrorMessage);
            }
            return new Tuple<HttpStatusCode, string>(response.StatusCode, response.Content);
        }

        public static IRestResponse PostApi(string baseUrl, string apiName, List<Tuple<string, string>> args = null)
        {
            var client = new RestSharpClient(baseUrl);

            var request = new RestRequest();
            request.Method = Method.POST;
            request.Resource = apiName;
            request.AddHeader("Accept", "application/json");

            if (args != null)
            {
                args.ForEach(c =>
                {
                    request.AddParameter(c.Item1, c.Item2);
                });
            }

            var response = client.Execute(request);

            return response;
        }

        public static string GetApiNew(string baseUrl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl);
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
            request.CookieContainer = Container;

            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        string retString = myStreamReader.ReadToEnd();
                        return retString;
                    }
                }
            }
            else
            {
                return "添加失败";
            }
        }

        public static string PostApiNew(string baseUrl, string apiName, List<Tuple<string, string>> args)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl);
            request.Method = "POST";
            /*
             * ecmsfrom=&enews=login&username=changjiang2&password=owen1216&lifetime=0&%E7%99%BB%E5%BD%95=&ecmsfrom=..%2Fmember%2Fcp%2F
             */

            var parms = "";
            var num = 0;
            args.ForEach(c =>
            {
                if (num == args.Count - 1)
                    parms += c.Item1 + "=" + c.Item2;
                else
                    parms += c.Item1 + "=" + c.Item2 + "&";
                num++;
            });
            ASCIIEncoding encoding = new ASCIIEncoding();
            var data = encoding.GetBytes(parms);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.CookieContainer = new CookieContainer();

            var stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            var response = (HttpWebResponse)request.GetResponse();

            Container = request.CookieContainer;

            var result = "";

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        string retString = myStreamReader.ReadToEnd();
                        return retString;
                    }
                }
            }
            else
            {
                return "登录失败";
            }
        }

        public static IRestResponse PostApi(string baseUrl, JObject cookie, JObject args)
        {
            var client = new RestSharpClient(baseUrl);

            var request = new RestRequest();
            request.Method = Method.POST;
            request.AddHeader("ContentType", "multipart/form-data; boundary=----WebKitFormBoundaryiQG6uCAfWtXO4al6");
            request.AddHeader("Accept-Language", "zh-cn");

            var cookiePro = cookie.Properties();
            foreach (var item in cookiePro)
            {
                request.AddCookie(item.Name, item.Value.ToString());
            }

            var argsPro = args.Properties();
            foreach (var item in argsPro)
            {
                request.AddParameter(item.Name, item.Value);
            }

            //request.AddFile()

            //request.

            var response = client.Execute(request);

            return response;
        }


        public static IRestResponse PostFormApi(string baseUrl, JObject cookie, JObject args)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl);
            #region 初始化请求对象
            request.Method = "POST";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.KeepAlive = false;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
            //if (!string.IsNullOrEmpty(refererUrl))
            //request.Referer = @"http://www.012395.com/e/DoInfo/AddInfo.php?mid=15&enews=MAddInfo&classid=115&Submit=%E6%B7%BB%E5%8A%A0%E4%BF%A1%E6%81%AF";

            request.CookieContainer = Container;
            #endregion

            string boundary = "----WebKitFormBoundarygrwZCrs0qOaFOwKE";//分隔符
            //boundary=----WebKitFormBoundarygrwZCrs0qOaFOwKE
            request.ContentType = boundary;

            var postStream = new MemoryStream();
            /*
             ------WebKitFormBoundarygrwZCrs0qOaFOwKE
Content-Disposition: form-data; name="enews"

MAddInfo
             */

            /*
                string dataFormdataTemplate =
                    "\r\n--" + boundary +
                    "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                    "\r\n\r\n{1}";
             */

            string dataFormdataTemplate = "\r\n--" + boundary +
                "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                "\r\n\r\n{1}";

            var argsPro = args.Properties();
            foreach (var item in argsPro)
            {
                var formdata = string.Format(dataFormdataTemplate, item.Name, item.Value.ToString());

                //统一处理
                byte[] formdataBytes = null;
                //第一行不需要换行
                if (postStream.Length == 0)
                    formdataBytes = Encoding.UTF8.GetBytes(formdata.Substring(2, formdata.Length - 2));
                else
                    formdataBytes = Encoding.UTF8.GetBytes(formdata);
                postStream.Write(formdataBytes, 0, formdataBytes.Length);
            }
            //结尾
            var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            postStream.Write(footer, 0, footer.Length);
            request.ContentLength = postStream.Length;

            #region 输入二进制流
            if (postStream != null)
            {
                postStream.Position = 0;
                //直接写入流
                Stream requestStream = request.GetRequestStream();

                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }

                postStream.Close();//关闭文件访问
            }
            #endregion

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            var result = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    string retString = myStreamReader.ReadToEnd();
                    return retString;
                }
            }
        }
    }
}
