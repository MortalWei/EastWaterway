using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EastWaterway
{
    /// <summary>
    /// Rest Sharp Client
    /// </summary>
    internal class RestSharpClient : EastWaterway.IRestSharp
    {
        RestClient Client;

        string BaseUrl { get; set; }

        string DefaultDateParameterFormat { get; set; }

        IAuthenticator DefaultAuthenticator { get; set; }

        public RestSharpClient(string baseUrl, IAuthenticator authenticator = null)
        {
            BaseUrl = baseUrl;

            DefaultAuthenticator = authenticator;

            DefaultDateParameterFormat = "yyyy-MM-dd HH:mm:ss";

            Client = new RestClient(baseUrl);

            if (DefaultAuthenticator != null)
            {
                Client.Authenticator = DefaultAuthenticator;
            }
        }

        static RestSharpClient()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        public IRestResponse Execute(IRestRequest request)
        {
            if (string.IsNullOrEmpty(request.DateFormat))
            {
                request.DateFormat = DefaultDateParameterFormat;
            }
            var response = Client.Execute(request);
            return response;
        }
    }
}
