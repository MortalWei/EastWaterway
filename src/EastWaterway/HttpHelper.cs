using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EastWaterway
{
    public class HttpHelper
    {
        public static Tuple<HttpStatusCode, string> GetApi(string baseUrl, string apiName, string args = "")
        {
            var client = new RestSharpClient(baseUrl);
            var requset = string.IsNullOrEmpty(args) ? new RestRequest(apiName, Method.GET) : new RestRequest($"{apiName}/{args}", Method.GET);

            var response = client.Execute(request: requset);

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
            ////request.AddHeader("Content-Type", "application/json");
            ////request.RequestFormat = DataFormat.Json;
            ////request.AddJsonBody(args);
            //request.AddParameter("ecmsfrom", "");
            //request.AddParameter("enews", "login");
            //request.AddParameter("username", "changjiang2");
            //request.AddParameter("password", "owen1216");
            //request.AddParameter("lifetime", 0);
            //request.AddParameter(Uri.EscapeDataString("登录"), "");
            //request.AddParameter("ecmsfrom", "../member/cp/");

            if (args != null)
            {
                args.ForEach(c =>
                {
                    request.AddParameter(c.Item1, c.Item2);
                });
            }


            var response = client.Execute(request);

            return response;

            //if (response.StatusCode != HttpStatusCode.OK)
            //{
            //    return new Tuple<HttpStatusCode, string>(response.StatusCode, response.ErrorMessage);
            //}
            //return new Tuple<HttpStatusCode, string>(response.StatusCode, response.Content);
        }
    }
}
