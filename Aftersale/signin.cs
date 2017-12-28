using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aftersale
{
    public partial class signin : Form
    {
        public signin()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            System.Environment.Exit(0); //这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Console.WriteLine(textBox2.Text);
            HttpGetData myHttpGetData = new HttpGetData();
            string biaoti = myHttpGetData.shuaXinData("http://121.199.161.59/sanli/Getdata?server=2&comm=5%2C%27select+count%28*%29+as+sl+from+sys_user+where+password%3D%27%27" + outJiaMiString(textBox2.Text) + "%27%27+and+login_name%3D%27%27" + textBox1.Text + "%27%27%27");
            //Console.WriteLine("http://121.199.161.59/sanli/Getdata?server=2&comm=5%2C%27select+count%28*%29+as+sl+from+sys_user+where+password%3D%27%27" + outJiaMiString(textBox2.Text) + "%27%27+and+login_name%3D%27%27" + textBox1.Text + "%27%27%27");
            List<loginjieguo> mylist = DeserializeJsonToList<loginjieguo>(biaoti);
            if (mylist[0].sl > 0)
            {
                Form1.useid = textBox1.Text;
                this.Close();
                //MessageBox.Show("登陆成功");
            }
            else
            {
                MessageBox.Show("帐号和密码不正确");
            }




            //foreach (loginjieguo mydata in mylist)
            //{
            //    DataRow newRow = shangPinData.NewRow();
            //    newRow["product_code"] = mydata.product_code;
            //    newRow["sku_name"] = mydata.sku_name;
            //    newRow["num"] = mydata.num;
            //    newRow["real_payment"] = mydata.real_payment;
            //    shangPinData.Rows.Add(newRow);
            //    //Console.WriteLine("{0},{1}", mydata.skuId, mydata.颜色);
            //}
        }
        private string outJiaMiString(string pass)
        {
            byte[] result = Encoding.Default.GetBytes(pass);    //tbPass为输入密码的文本框  
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            //this.tbMd5pass.Text = BitConverter.ToString(output).Replace("-", "");  //tbMd5pass为输出加密文本的文本框  
            Console.WriteLine("原字符串：{0},新字符串：{1}",pass,BitConverter.ToString(output).Replace("-", ""));
            return BitConverter.ToString(output).Replace("-", "");
        }
        /// <summary>
        /// 商品分类
        /// </summary>
        private class loginjieguo
        {
            public int sl;
        }
        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
        /// <returns>对象实体集合</returns>
        public static List<T> DeserializeJsonToList<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = o as List<T>;
            return list;
        }
    }
}
