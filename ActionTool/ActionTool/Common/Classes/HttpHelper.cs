using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ActionTool.Common.Classes
{
    public class HttpHelper
    {
        /// <summary>
        /// 使用json格式进行post提交
        /// </summary>
        /// <param name="jsonData"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string PostWithJson(string jsonData, string url, string type = "POST")
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var data = Encoding.UTF8.GetBytes(jsonData);

                request.Method = type;
                request.ContentType = "text/json";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch (Exception ex)
            {
                return "error";
            }
        }
    }
}
