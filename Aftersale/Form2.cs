using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aftersale
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            InitializeConnection();
            InitializeDataTable();
            this.Text = "新增";
            caozuo = "lujunhua";
            gridControl1.DataSource = mingXiData;
        }
        public Form2(string biaoTi,string pramcaozuo,string parmguid)
        {
            InitializeComponent();
            InitializeConnection();
            InitializeDataTable();
            this.Text = biaoTi;
            caozuo = pramcaozuo;
            myGuid = parmguid;
            //如果传过来的guid不是空的话就获取数据信息
            if(parmguid!="")
            {
                getData(parmguid);
            }
            gridControl1.DataSource = mingXiData;
        }
        private void getData(string parmguid)
        {
            DataTable lindt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter("select `taitou`,`shuihao`,`dizhi`,`zhanghao`,`dingdan` from sanli.fapiao_KaiPiaoXinXi where guid='"+parmguid+"'", conn);
            da.Fill(lindt);
            if(lindt.Rows.Count>0)
            {
                DataRow dr = lindt.Rows[0];
                textBox1.Text = dr["taitou"].ToString();
                textBox2.Text = dr["shuihao"].ToString();
                textBox3.Text = dr["dizhi"].ToString();
                textBox4.Text = dr["zhanghao"].ToString();
                foreach(string s in dr["dingdan"].ToString().Split(','))
                {
                    DataRow dr2 = mingXiData.NewRow();
                    dr2[0] = s;
                    mingXiData.Rows.Add(dr2);
                }
            }
        }
        private void InitializeConnection()
        {
            ConfigurationManager.RefreshSection("appSettings");
            conn = new MySqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        }

        private void InitializeDataTable()
        {
            mingXiData.Columns.Add("dd");

            //初始化订单明细表
            shangPinData.Columns.Add("product_code");
            shangPinData.Columns.Add("sku_name");
            shangPinData.Columns.Add("num",typeof(int));
            shangPinData.Columns.Add("real_payment",typeof(float));
            shangPinData.Columns.Add("leixing");

            //初始化商品分类开
            FenLeiData.Columns.Add("kaitou");
            FenLeiData.Columns.Add("leixing");


            //mingXiData.Columns.Add("ww");
            //设置主键
            DataColumn[] clos = new DataColumn[1];
            clos[0] = mingXiData.Columns["dd"];
            mingXiData.PrimaryKey = clos;
            fenLeiData();
        }
        string caozuo;
        string myGuid = "";
        DataTable mingXiData = new DataTable("mingxi");
        MySqlConnection conn;
        bool zctc = false;//控制是否可以关闭窗口
        HttpGetData myHttpGetData = new HttpGetData();
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //e.Value = 1;
            Console.WriteLine(e.Value);
            shuaXinData(e.Value.ToString());

            //shangPinMingXiData(e.Value.ToString());
            //this.gridView1.FocusedRowHandle = this.gridView1.DataRowCount - 1;//焦点转移到最后一行
            //gridView1.FocusedRowHandle = 0;
            //DevExpress.XtraGrid.Columns.GridColumn dc = gridView1.Columns["dd"];
            //gridView1.FocusedColumn = dc;
        }
        /// <summary>
        /// 获取订单备注信息
        /// </summary>
        /// <param name="parmhuohao"></param>
        private void shuaXinData(string parmhuohao)
        {
            
            string biaoti = myHttpGetData.shuaXinData("http://121.199.161.59/sanli/Getdata?server=2&comm=5%2C%27select+concat%28buyer_message%2CCHAR%2810%29%2Cseller_memo%29+as+bz+from+oms_order+where+order_no%3D%27%27" + parmhuohao + "%27%27%27");
            textBox5.Text += biaoti.Substring(8, biaoti.Length - 11);
        }
        DataTable shangPinData = new DataTable("shangpin");
        DataTable FenLeiData = new DataTable("fenlei");
        /// <summary>
        /// 获取商品明细
        /// </summary>
        /// <param name="parmhuohao"></param>
        private void shangPinMingXiData(string parmhuohao)
        {

            string biaoti = myHttpGetData.shuaXinData("http://121.199.161.59/sanli/Getdata?server=2&comm=5%2C%27select+product_code%2Csku_name%2Cnum%2Creal_payment+from+oms_order+as+a+inner+join+oms_item+as+b+on+a.id%3Db.order_id+where+a.order_no%3D%27%27" + parmhuohao + "%27%27+and+is_gift%3D0%27");
            List<oms_item> mylist = DeserializeJsonToList<oms_item>(biaoti);
            foreach (oms_item mydata in mylist)
            {
                DataRow newRow = shangPinData.NewRow();
                newRow["product_code"] = mydata.product_code;
                newRow["sku_name"] = mydata.sku_name;
                newRow["num"] = mydata.num;
                newRow["real_payment"] = mydata.real_payment;
                shangPinData.Rows.Add(newRow);
                //Console.WriteLine("{0},{1}", mydata.skuId, mydata.颜色);
            }

            //处理货号对应的开票类型
            foreach(DataRow dr in shangPinData.Rows)
            {
                bool zd = false;
                foreach(DataRow dr2 in FenLeiData.Rows)
                {
                    string kaiton = dr2["kaitou"].ToString().ToUpper();
                    //Console.WriteLine("货号开头：{0}，现在要对比的开头{1}，完整的货号：{2}",dr["product_code"].ToString().Substring(0, kaiton.Length),kaiton, dr["product_code"].ToString());
                    string fingHuoHao = dr["product_code"].ToString();
                    
                    if(kaiton.Length <= fingHuoHao.Length && kaiton == fingHuoHao.Substring(0, kaiton.Length).ToUpper())
                    {
                        Console.WriteLine("huohao:{0},leixing:{1},kaitou:{2}", fingHuoHao, dr2["leixing"], kaiton);
                        dr["leixing"] = dr2["leixing"];
                        zd = true;
                        break;

                    }                    
                }
                if(!zd)
                {
                    dr["leixing"] = "毛巾";
                }
            }
            
            //汇总
            DataTable dtResult = shangPinData.Clone();
            DataTable dtName = shangPinData.DefaultView.ToTable(true, "leixing");
            for (int i = 0; i < dtName.Rows.Count; i++)
            {
                DataRow[] rows = shangPinData.Select("leixing='" + dtName.Rows[i]["leixing"] + "'");
                //temp用来存储筛选出来的数据  
                DataTable temp = dtResult.Clone();
                foreach (DataRow row in rows)
                {
                    temp.Rows.Add(row.ItemArray);
                }

                DataRow dr = dtResult.NewRow();
                dr["leixing"] = dtName.Rows[i][0].ToString();
                dr["num"] = temp.Compute("sum(num)", "");
                dr["real_payment"] = temp.Compute("sum(real_payment)", "");
                dtResult.Rows.Add(dr);

            }
            foreach(DataRow dr in dtResult.Rows)
            {
                Console.WriteLine("类型：{0}，数量{1}，金额：{2}",dr["leixing"], dr["num"], dr["real_payment"]);
            }
            

        }
        /// <summary>
        /// 获取商品分类
        /// </summary>
        /// <param name="parmhuohao"></param>
        private void fenLeiData()
        {
            MySqlDataAdapter da = new MySqlDataAdapter("SELECT `kaitou`,`leixing` FROM sanli.`fapiaofenlei` ORDER BY -LENGTH(kaitou)", conn);
            da.Fill(FenLeiData);

            //string biaoti = myHttpGetData.shuaXinData("http://121.199.161.59/sanli/Getdata?server=1&comm=5%2C%27SELECT+%60kaitou%60%2C%60leixing%60+FROM+sanli.%60fapiaofenlei%60+ORDER+BY+-LENGTH%28kaitou%29%27");
            //List <oms_fenlei> mylist = DeserializeJsonToList<oms_fenlei>(biaoti);
            //foreach (oms_fenlei mydata in mylist)
            //{
            //    DataRow newRow = FenLeiData.NewRow();
            //    newRow["kaitou"] = mydata.kaitou;
            //    newRow["leixing"] = mydata.leixing;
            //    FenLeiData.Rows.Add(newRow);
            //    //Console.WriteLine("{0},{1}", mydata.skuId, mydata.颜色);
            //}
            Console.WriteLine(FenLeiData.Rows.Count);

        }
        /// <summary>
        /// 商品明细
        /// </summary>
        private class oms_item
        {
            public string product_code;
            public string sku_name;
            public int num;
            public float real_payment;
        }
        /// <summary>
        /// 商品分类
        /// </summary>
        private class oms_fenlei
        {
            public string kaitou;
            public string leixing;
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
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 数据录入是否完整
        /// </summary>
        /// <returns></returns>
        private bool getXinXiWanZheng()
        {
            bool wz = true;
            if(textBox1.Text=="" || textBox2.Text == "" || mingXiData.Rows.Count>0)
            {
                wz = false;
            }
            return wz;
        }
        /// <summary>
        /// 是否有数据录入
        /// </summary>
        /// <returns></returns>
        private bool getXinXiWanZheng2()
        {
            bool wz =false;
            if (textBox1.Text != "" || textBox2.Text != "" || textBox3.Text != "" || textBox4.Text != "" || textBox5.Text != "" || mingXiData.Rows.Count > 0)
            {
                wz = true;
            }
            return wz;
        }
        /// <summary>
        /// 有数据录入时是否可以关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (!zctc && getXinXiWanZheng2())
            {
                if (MessageBox.Show("要放弃现在录入的数据吗?", "提示", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
        //保存数据
        private void button1_Click(object sender, EventArgs e)
        {
            MySqlCommand cm;
            try
            {
                string dainhao = "";
                foreach(DataRow dr in mingXiData.Rows)
                {
                    dainhao += "," + dr["dd"].ToString();
                }
                if (this.Text == "新增")
                {
                    cm = new MySqlCommand("INSERT INTO sanli.`fapiao_KaiPiaoXinXi` (`guid`,`taitou`,`shuihao`,`dizhi`,`zhanghao`,`dingdan`,`creat`,`creattime`,`zhuangtai`,`biaoji`) VALUES (uuid(),'" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + textBox4.Text + "','" + dainhao.Substring(1) + "','" + caozuo + "',NOW(),'待处理',FALSE)", conn);

                }
                else
                {
                    cm = new MySqlCommand("update sanli.`fapiao_KaiPiaoXinXi` set `taitou`='" + textBox1.Text + "',`shuihao`='" + textBox2.Text + "',`dizhi`='" + textBox3.Text + "',`zhanghao`='" + textBox4.Text + "',`dingdan`='" + dainhao.Substring(1) + "' where guid='" + myGuid + "'", conn);

                }
                //Console.WriteLine("INSERT INTO sanli.`fapiao_KaiPiaoXinXi` (`taitou`,`shuihao`,`dizhi`,`zhanghao`,`dingdan`,`creat`,`creattime`,`zhuangtai`,`biaoji`) VALUES ('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + textBox4.Text + "','" + dainhao.Substring(1) + "','" + caozuo + "',NOW(),'',FALSE)");
                if (conn.State == 0) conn.Open();
                if(cm.ExecuteNonQuery()>0)
                {
                    zctc = true;
                    //MessageBox.Show("成功保存");
                    this.Close();

                }
                else
                {
                    MessageBox.Show("保存失败");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
      
    }
}
