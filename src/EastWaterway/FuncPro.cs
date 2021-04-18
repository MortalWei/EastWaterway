using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastWaterway
{
    public static class FuncPro
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Tuple<string, string> ToDateStr(this string args)
        {
            if (DateTime.TryParse(args, out DateTime date))
            {
                var str1 = date.Month.ToString();
                var str2 = date.Day.ToString();
                return new Tuple<string, string>(str1, str2);
            }
            var now = DateTime.Now;

            return new Tuple<string, string>(now.Month.ToString(), now.Day.ToString());
        }
    }
}
