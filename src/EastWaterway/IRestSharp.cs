using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastWaterway
{
    internal interface IRestSharp
    {
        /// <summary>
        /// Synchronous execution
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IRestResponse Execute(IRestRequest request);
    }
}
