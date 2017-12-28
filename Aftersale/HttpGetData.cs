using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aftersale
{
    class HttpGetData
    {
        /// <summary>
        /// 获取mq
        /// </summary>
        /// <param name="parmhuohao"></param>
        public string shuaXinData(string parmUrl)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(parmUrl);
            req.Method = "GET";
            req.Timeout = 30000;
            string biaoti = "";
            //httpRequest.Headers.Add("content", "text/html; charset=gb2312");
            using (WebResponse wr = req.GetResponse())
            {
                Stream strm = wr.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
                StreamReader sr = new StreamReader(strm, encode);
                
                string line = "";
                //biaoti += line;
                while ((line = sr.ReadLine()) != null)
                {
                    biaoti += line;
                }
                //textBox5.Text += biaoti.Substring(8, biaoti.Length - 12);
                //Console.WriteLine(biaoti);
            }
            return biaoti;
        }
    }
}
