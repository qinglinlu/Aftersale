using DevExpress.XtraPrinting.HtmlExport.Native;
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
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace Aftersale
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            ////显示登陆窗口
            //signin w1 = new signin();
            //w1.ShowDialog();
            InitializeComponent();

            //初始化信息
            getwentileixing();
            dateEdit2.Text = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
            dateEdit1.Text = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd HH:MM:ss");
            InitializeConnection();
            gridControl1.DataSource = wenTiData;
            InitializeDataTable();//初始化发票要用表格
        }

        public static string useid ="lujunhua";
        MySqlConnection conn;
        DataTable wenTiData = new DataTable();
        HttpGetData myHttpGetData = new HttpGetData();
        private void InitializeConnection()
        {
            ConfigurationManager.RefreshSection("appSettings");
            conn = new MySqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        }
        /// <summary>
        /// 获取问题类型
        /// </summary>
        private void getwentileixing()
        {
            string line = getRetrunData("http://121.199.161.59/sanli/Getdata?server=2&comm=10%2C%27%27");
            List<wtlx> mylist = DeserializeJsonToList<wtlx>(line);
            if (mylist != null)
            {
                comboBox1.Items.Clear();
                //myquhuoDataTable.Clear();
                comboBox1.Items.Add("");
                foreach (wtlx mydata in mylist)
                {
                    //DataRow newRow = myquhuoDataTable.NewRow();

                    comboBox1.Items.Add(mydata.mengcheng);
                    //myquhuoDataTable.Rows.Add(newRow);
                }
                //gridControl1.DataSource = myquhuoDataTable;
                //gridView1.BestFitColumns();
            }
            else
            {
                MessageBox.Show("未能获取数据");
            }
            //HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://121.199.161.59/sanli/Getdata?server=2&comm=10%2C%27%27");
            //req.Method = "GET";
            //req.Timeout = 300000;
            //using (WebResponse wr = req.GetResponse())
            //{
            //    Stream strm = wr.GetResponseStream();
            //    StreamReader sr = new StreamReader(strm);
            //    string line = sr.ReadLine();
            //    //while ((line = sr.ReadLine()) != null)
            //    //{
            //    //    Console.WriteLine(line);
            //    //    //listBox1.Items.Add(line);
            //    //}
            //    strm.Close();

            //    List<wtlx> mylist = DeserializeJsonToList<wtlx>(line);
            //    if (mylist != null)
            //    {
            //        comboBox1.Items.Clear();
            //        //myquhuoDataTable.Clear();
            //        comboBox1.Items.Add("");
            //        foreach (wtlx mydata in mylist)
            //        {
            //            //DataRow newRow = myquhuoDataTable.NewRow();

            //            comboBox1.Items.Add(mydata.mingcheng);        
            //            //myquhuoDataTable.Rows.Add(newRow);
            //        }
            //        //gridControl1.DataSource = myquhuoDataTable;
            //        //gridView1.BestFitColumns();
            //    }
            //    else
            //    {
            //        MessageBox.Show("未能获取数据");
            //    }
            //}
        }
        private string getRetrunData(string reponse)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(reponse);
            req.Method = "GET";
            req.Timeout = 300000;
            using (WebResponse wr = req.GetResponse())
            {
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm);
                string line = sr.ReadLine();
                //while ((line = sr.ReadLine()) != null)
                //{
                //    Console.WriteLine(line);
                //    //listBox1.Items.Add(line);
                //}
                strm.Close();
                return line;
            }
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
        /// <summary>
        /// 问题类型
        /// </summary>
        private class wtlx
        {
            public int id;
            public string mengcheng;
        }
        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <returns>sql条件文本</returns>
        private string gettiaojian()
        {
            string tj = "";
            if(dateEdit1.Text!="")
            {
                tj += " and creattime>='" + dateEdit1.Text + "'";
            }
            if (dateEdit2.Text != "")
            {
                tj += " and creattime<='" + dateEdit2.Text + "'";
            }
            if (comboBox1.Text != "")
            {
                tj += " and leixing='" + comboBox1.Text + "'";
            }
            if (comboBox2.Text != "")
            {
                tj += " and zhuangtai='" + comboBox2.Text + "'";
            }
            if (buttonEdit1.Text != "")
            {
                tj += " and chuli='" + buttonEdit1.Text + "'";
            }
            if (buttonEdit2.Text != "")
            {
                tj += " and creat='" + buttonEdit2.Text + "'";
            }
            if (textBox1.Text != "")
            {
                tj += " and dingdan='" + textBox1.Text + "'";
            }
            if (textBox2.Text != "")
            {
                tj += " and wenti like '%" + textBox2.Text + "%'";
            }
            if(tj!="")
            {
                tj = " where " + tj.Substring(5);
            }
            return tj;
        }

        /// <summary>
        /// 向表格填充数据
        /// </summary>
        /// <param name="tiaojian">查询条件</param>
        private void getRenWuData(string tiaojian)
        {
            if (dateEdit1.Text == "" || dateEdit2.Text == "")
            {
                MessageBox.Show("时间不能为空");
                return;
            }
            toolStripStatusLabel1.Text = "正在处理...";
            statusStrip1.Refresh();//刷新脚本显示
            //DataTable dt = new DataTable();
            DateTime startTime = DateTime.Now;
            wenTiData.Clear();
            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM sanli.`fapiao_KaiPiaoXinXi`"+ tiaojian, conn);
            Console.WriteLine("SELECT * FROM sanli.`fapiao_KaiPiaoXinXi`" + tiaojian);
            da.Fill(wenTiData);
            chuLiQiTaBiaoGe(wenTiData);
            TimeSpan ts = startTime.Subtract(DateTime.Now).Duration();
            toolStripStatusLabel1.Text = "共" + wenTiData.Rows.Count.ToString() + "条记录  耗时：" + string.Format("{0:D2}", ts.Hours) + ":" + string.Format("{0:D2}", ts.Minutes) + ":" + string.Format("{0:D2}", ts.Seconds) + ":" + string.Format("{0:D3}", ts.Milliseconds);
            gridView1.BestFitColumns();
            gridView2.BestFitColumns();
            gridView3.BestFitColumns();
            gridView4.BestFitColumns();
        }
        private void chuLiQiTaBiaoGe(DataTable zhuDataTable)
        {
            //初始化表结构
            DataTable yichuliTable = zhuDataTable.Clone();
            DataTable weichuliTable= zhuDataTable.Clone();
            DataTable wodeTable = zhuDataTable.Clone();
            //Console.WriteLine(yichuliTable.Columns.Count);
            DataRow[] newDataRows1 = zhuDataTable.Select("zhuangtai='完结'");
            DataRow[] newDataRows2 = zhuDataTable.Select("zhuangtai<>'完结'");
            DataRow[] newDataRows3 = zhuDataTable.Select("chuli='"+ useid + "'");

            foreach (DataRow dr in newDataRows1)
            {
                DataRow newdr = yichuliTable.NewRow();
                for (int i = 0; i < zhuDataTable.Columns.Count; i++)
                {
                    newdr[i] = dr[i];
                }
                yichuliTable.Rows.Add(newdr);
            }

            foreach (DataRow dr in newDataRows2)
            {
                DataRow newdr = weichuliTable.NewRow();
                for (int i = 0; i < zhuDataTable.Columns.Count; i++)
                {
                    newdr[i] = dr[i];
                }
                weichuliTable.Rows.Add(newdr);
            }

            foreach (DataRow dr in newDataRows3)
            {
                DataRow newdr = wodeTable.NewRow();
                for (int i = 0; i < zhuDataTable.Columns.Count; i++)
                {
                    newdr[i] = dr[i];
                }
                wodeTable.Rows.Add(newdr);
            }
            gridControl2.DataSource = yichuliTable;
            gridControl3.DataSource = weichuliTable;
            gridControl4.DataSource = wodeTable;
        }
        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            getRenWuData(gettiaojian());
        }
        //新增
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form2 w1 = new Form2("新增","lujunhua","");
            w1.ShowDialog();
        }
        //修改
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Console.WriteLine(xtraTabControl1.SelectedTabPageIndex);

            DevExpress.XtraGrid.Views.Grid.GridView g2= getSelectGridView();

            if (g2.SelectedRowsCount > 0 && g2.GetFocusedRowCellValue("zhuangtai").ToString() == "完结")
            {
                MessageBox.Show("已完结记录不能修改");
            }
            else
            {
                Form2 w1 = new Form2("修改", useid, gridView3.GetFocusedRowCellValue("guid").ToString());
                w1.ShowDialog();
            }
            //Form2 w1;
            //switch (xtraTabControl1.SelectedTabPageIndex)
            //{
            //    case 0:
            //        if(gridView1.SelectedRowsCount>0)
            //        {
            //            w1 = new Form2("修改", "lujunhua", gridView1.GetFocusedRowCellValue("guid").ToString());
            //            w1.ShowDialog();
            //        }
                    
            //        break;
            //    case 1:
            //        MessageBox.Show("已完结记录不能修改");
            //        break;
            //    case 2:
            //        if (gridView2.SelectedRowsCount > 0)
            //        {
            //            w1 = new Form2("修改", "lujunhua", gridView2.GetFocusedRowCellValue("guid").ToString());
            //            w1.ShowDialog();
            //        }

            //        break;
            //    case 3:
            //        if (gridView3.SelectedRowsCount > 0 && gridView3.GetFocusedRowCellValue("zhuangtai").ToString()!="完结")
            //        {
            //            w1 = new Form2("修改", "lujunhua", gridView3.GetFocusedRowCellValue("guid").ToString());
            //            w1.ShowDialog();
            //        }

            //        break;
            //    default:
            //        break;
            //}
            //Form2 w1 = new Form2("修改","lujunhua","");
            
        }
        //删除数据
        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView g2 = getSelectGridView();
            //Console.WriteLine(g2.select);
            if (g2.SelectedRowsCount <= 0)
            {
                //MessageBox.Show("已完结记录不能修改");
                return;
            }
            if (g2.GetFocusedRowCellValue("zhuangtai").ToString() == "完结")
            {
                MessageBox.Show("完结数据不能删除");
                return;
            }
            if (MessageBox.Show("真要要删除这个记录吗？","提示", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                MySqlCommand cm;
                try
                {
                    cm = new MySqlCommand("delete from sanli.`fapiao_KaiPiaoXinXi` where guid='" +g2.GetFocusedRowCellValue("guid").ToString() + "'", conn);
                    //Console.WriteLine("INSERT INTO sanli.`fapiao_KaiPiaoXinXi` (`taitou`,`shuihao`,`dizhi`,`zhanghao`,`dingdan`,`creat`,`creattime`,`zhuangtai`,`biaoji`) VALUES ('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + textBox4.Text + "','" + dainhao.Substring(1) + "','" + caozuo + "',NOW(),'',FALSE)");
                    if (conn.State == 0) conn.Open();
                    if (cm.ExecuteNonQuery() > 0)
                    {
                        getRenWuData(gettiaojian());
                        g2.MoveFirst();
                        //MessageBox.Show("成功保存");
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
        //获取当前是哪个选择夹在激活状态
        private DevExpress.XtraGrid.Views.Grid.GridView getSelectGridView()
        {
            DevExpress.XtraGrid.Views.Grid.GridView w1;
            switch (xtraTabControl1.SelectedTabPageIndex)
            {
                case 0:
                    w1 = gridView1;
                    break;
                case 1:
                    w1 = gridView2;
                    break;
                case 2:
                    w1 = gridView3;
                    break;
                case 3:
                    w1 = gridView4;
                    break;
                default:
                    w1 = gridView1;
                    break;
            }
            return w1;
        }
        //生成发票文件
        private void barButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView g2 = getSelectGridView();
            //Console.WriteLine(g2.select);
            if (g2.SelectedRowsCount <= 0)
            {
                return;
            }

            int[] selectId = g2.GetSelectedRows();
            foreach (int i in selectId)
            {
                if (g2.GetRowCellValue(i, "zhuangtai").ToString() != "处理中")
                {
                    MessageBox.Show("只能生成处理中任务");
                    return;
                }
                if (g2.GetRowCellValue(i, "chuli").ToString() != useid)
                {
                    MessageBox.Show("只能生成自己接的任务");
                    return;
                }
            }
            foreach (int i in selectId)
            {
                shangPinMingXiData(g2.GetRowCellValue(i, "dingdan").ToString());
            }
            

        }
        //接任务
        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView g2 = getSelectGridView();
            //Console.WriteLine(g2.select);
            if (g2.SelectedRowsCount <= 0)
            {
                //MessageBox.Show("已完结记录不能修改");
                return;
            }
            int[] selectId=g2.GetSelectedRows();
            foreach(int i in selectId)
            {
                if (g2.GetRowCellValue(i, "zhuangtai").ToString() != "待处理")
                {
                    MessageBox.Show("只能接手待处理任务");
                    return;
                }
            }
            string jieshouguid = "";
            foreach (int i in selectId)
            {
                jieshouguid +=",'"+g2.GetRowCellValue(i, "guid").ToString()+"'";
            }
            MySqlCommand cm;
            try
            {
                cm = new MySqlCommand("update sanli.`fapiao_KaiPiaoXinXi` set chuli='"+useid+ "',chulitime=now(),zhuangtai='处理中' where guid in (" + jieshouguid.Substring(1) + ")", conn);
                if (conn.State == 0) conn.Open();
                if (cm.ExecuteNonQuery() > 0)
                {
                    getRenWuData(gettiaojian());
                    g2.MoveFirst();
                    //MessageBox.Show("成功保存");
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
        //撤任务
        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView g2 = getSelectGridView();
            //Console.WriteLine(g2.select);
            if (g2.SelectedRowsCount <= 0)
            {
                //MessageBox.Show("已完结记录不能修改");
                return;
            }
            int[] selectId = g2.GetSelectedRows();
            foreach (int i in selectId)
            {
                if (g2.GetRowCellValue(i, "zhuangtai").ToString() != "处理中")
                {
                    MessageBox.Show("只能接手处理中任务");
                    return;
                }
                if (g2.GetRowCellValue(i, "chuli").ToString() != useid)
                {
                    MessageBox.Show("只能撤消自己任务");
                    return;
                }
            }
            string jieshouguid = "";
            foreach (int i in selectId)
            {
                jieshouguid += ",'" + g2.GetRowCellValue(i, "guid").ToString() + "'";
            }
            MySqlCommand cm;
            try
            {
                cm = new MySqlCommand("update sanli.`fapiao_KaiPiaoXinXi` set chuli=null,chulitime=null,zhuangtai='待处理' where guid in (" + jieshouguid.Substring(1) + ")", conn);
                if (conn.State == 0) conn.Open();
                if (cm.ExecuteNonQuery() > 0)
                {
                    getRenWuData(gettiaojian());
                    g2.MoveFirst();
                    //MessageBox.Show("成功保存");
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
        //完结任务
        private void barButtonItem14_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView g2 = getSelectGridView();
            //Console.WriteLine(g2.select);
            if (g2.SelectedRowsCount <= 0)
            {
                //MessageBox.Show("已完结记录不能修改");
                return;
            }
            int[] selectId = g2.GetSelectedRows();
            foreach (int i in selectId)
            {
                if (g2.GetRowCellValue(i, "zhuangtai").ToString() != "处理中")
                {
                    MessageBox.Show("只能完结处理中任务");
                    return;
                }
                if (g2.GetRowCellValue(i, "chuli").ToString() != useid)
                {
                    MessageBox.Show("只能完结自己任务");
                    return;
                }
            }
            string jieshouguid = "";
            foreach (int i in selectId)
            {
                jieshouguid += ",'" + g2.GetRowCellValue(i, "guid").ToString() + "'";
            }
            MySqlCommand cm;
            try
            {
                cm = new MySqlCommand("update sanli.`fapiao_KaiPiaoXinXi` set zhuangtai='完结' where guid in (" + jieshouguid.Substring(1) + ")", conn);
                if (conn.State == 0) conn.Open();
                if (cm.ExecuteNonQuery() > 0)
                {
                    getRenWuData(gettiaojian());
                    g2.MoveFirst();
                    //MessageBox.Show("成功保存");
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



        DataTable shangPinData = new DataTable("shangpin");
        DataTable FenLeiData = new DataTable("fenlei");
        //初始化发票要用的表格
        private void InitializeDataTable()
        {

            //初始化订单明细表
            shangPinData.Columns.Add("product_code");
            shangPinData.Columns.Add("sku_name");
            shangPinData.Columns.Add("num", typeof(int));
            shangPinData.Columns.Add("real_payment", typeof(float));
            shangPinData.Columns.Add("leixing");

            //初始化商品分类开
            FenLeiData.Columns.Add("kaitou");
            FenLeiData.Columns.Add("leixing");


        }
        /// <summary>
        /// 获取商品明细
        /// </summary>
        /// <param name="parmhuohao"></param>
        private void shangPinMingXiData(string parmhuohao)
        {
            shangPinData.Clear();
            //Console.WriteLine("http://121.199.161.59/sanli/Getdata?server=2&comm=5%2C%27select+product_code%2Csku_name%2Cnum%2Creal_payment+from+oms_order+as+a+inner+join+oms_item+as+b+on+a.id%3Db.order_id+where+a.order_no+in+%28%27%27" + parmhuohao.Replace(",", "%27%27%2C%27%27") + "%27%27%29+and+b.is_gift%3D0%27");
            string biaoti = myHttpGetData.shuaXinData("http://121.199.161.59/sanli/Getdata?server=2&comm=5%2C%27select+product_code%2Csku_name%2Cnum%2Creal_payment+from+oms_order+as+a+inner+join+oms_item+as+b+on+a.id%3Db.order_id+where+a.order_no+in+%28%27%27" + parmhuohao.Replace(",", "%27%27%2C%27%27") + "%27%27%29+and+b.is_gift%3D0%27");
            List<oms_item> mylist = DeserializeJsonToList<oms_item>(biaoti);
            if(mylist==null)
            {
                return;
            }
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
            foreach (DataRow dr in shangPinData.Rows)
            {
                bool zd = false;
                string fenlei = "";
                if(dr["product_code"].ToString().IndexOf("//")!=-1 || dr["product_code"].ToString().LastIndexOf("T")== dr["product_code"].ToString().Length-1)
                {
                    fenlei = "毛巾套装";
                    zd = true;
                }
                if(dr["sku_name"].ToString().IndexOf("盒") != -1)
                {
                    fenlei = "毛巾礼盒";
                    zd = true;
                }
                if(fenlei == "")
                {
                    foreach (DataRow dr2 in FenLeiData.Rows)
                    {
                        string kaiton = dr2["kaitou"].ToString().ToUpper();
                        //Console.WriteLine("货号开头：{0}，现在要对比的开头{1}，完整的货号：{2}",dr["product_code"].ToString().Substring(0, kaiton.Length),kaiton, dr["product_code"].ToString());
                        string fingHuoHao = dr["product_code"].ToString();

                        if (kaiton.Length <= fingHuoHao.Length && kaiton == fingHuoHao.Substring(0, kaiton.Length).ToUpper())
                        {
                            Console.WriteLine("huohao:{0},leixing:{1},kaitou:{2}", fingHuoHao, dr2["leixing"], kaiton);
                            dr["leixing"] = dr2["leixing"];
                            zd = true;
                            break;

                        }
                    }
                }
                else
                {
                    dr["leixing"] = fenlei;
                }
                if (!zd)
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
            foreach (DataRow dr in dtResult.Rows)
            {
                Console.WriteLine("类型：{0}，数量{1}，金额：{2}", dr["leixing"], dr["num"], dr["real_payment"]);
            }
            buildInvoiceFile(dtResult);
        }
        private void buildInvoiceFile(DataTable parmInvoice)
        {
            DataTable faPiaoDaiMa = new DataTable();



            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "GBK", "");
            xmlDoc.AppendChild(node);
            //创建根节点
            XmlNode root = xmlDoc.CreateElement("Kp");
            xmlDoc.AppendChild(root);
            CreateNode(xmlDoc, root, "Version", "2.0");//有此节点，则表示用带分类编码

            XmlNode node1 = xmlDoc.CreateNode(XmlNodeType.Element, "Fpxx", null);
            CreateNode(xmlDoc, node1, "Zsl", "1");//此文件含有的单据信息数量
            root.AppendChild(node1);

            XmlNode node2 = xmlDoc.CreateNode(XmlNodeType.Element, "Fpsj", null);
            node1.AppendChild(node2);

            XmlNode node3 = xmlDoc.CreateNode(XmlNodeType.Element, "Fp", null);
            CreateNode(xmlDoc, node3, "Djh", "1");//单据号（20字节）
            CreateNode(xmlDoc, node3, "Gfmc", "1");//购方名称（100字节）
            CreateNode(xmlDoc, node3, "Gfsh", "1");//购方税号
            CreateNode(xmlDoc, node3, "Gfyhzh", "1");//购方银行账号（100字节）
            CreateNode(xmlDoc, node3, "Gfdzdh", "1");//购方地址电话（100字节）

            CreateNode(xmlDoc, node3, "Bz", "1");//备注（240字节）
            CreateNode(xmlDoc, node3, "Fhr", "1");//复核人（8字节）
            CreateNode(xmlDoc, node3, "Skr", "1");//收款人（8字节）

            CreateNode(xmlDoc, node3, "Spbmbbh", "1");//商品编码版本号(20字节)（必输项）
            CreateNode(xmlDoc, node3, "Hsbz", "1");//含税标志 0：不含税税率，1：含税税率，2：差额税;中外合作油气田（原海洋石油）5%税率、1.5%税率为1，差额税为2，其他为0；
            //CreateNode(xmlDoc, node3, "Sgbz", "1");
            node2.AppendChild(node3);

            XmlNode node4 = xmlDoc.CreateNode(XmlNodeType.Element, "Spxx", null);
            node3.AppendChild(node4);
            for(int i=0;i<2;i++)
            {
                XmlNode node5 = xmlDoc.CreateNode(XmlNodeType.Element, "Sph", null);
                CreateNode(xmlDoc, node5, "Xh", (i+1).ToString());//序号
                CreateNode(xmlDoc, node5, "Spmc", "1");////商品名称，金额为负数时此行是折扣行，折扣行的商品名称应与上一行的商品名称一致（100字节）
                CreateNode(xmlDoc, node5, "规格型号", "");//规格型号（40字节）
                CreateNode(xmlDoc, node5, "Jldw", "1");////计量单位（32字节）
                CreateNode(xmlDoc, node5, "Spbm", "1");////商品编码(19字节)（必输项）
                CreateNode(xmlDoc, node5, "Qyspbm", "1");////企业商品编码（20字节）
                CreateNode(xmlDoc, node5, "Syyhzcbz", "0");//是否使用优惠政策标识0：不使用，1：使用（1字节）
                CreateNode(xmlDoc, node5, "Lslbz", "");//零税率标识   空：非零税率，0：出口退税，1：免税，2：不征收，3普通零税率（1字节）
                CreateNode(xmlDoc, node5, "Yhzcsm", "");//优惠政策说明（50字节）
                CreateNode(xmlDoc, node5, "Dj", "1");//单价（中外合作油气田（原海洋石油）5%税率，单价为含税单价）
                CreateNode(xmlDoc, node5, "Sl", "1");//数量
                CreateNode(xmlDoc, node5, "Je", "1");//金额，当金额为负数时为折扣行
                CreateNode(xmlDoc, node5, "Slv", "1");//税率
                CreateNode(xmlDoc, node5, "Kce", "");//扣除额，用于差额税计算
                node4.AppendChild(node5);
            }
            
            try
            {
                xmlDoc.Save("e://data5.xml");
            }
            catch (Exception e)
            {
                //显示错误信息
                Console.WriteLine(e.Message);
            }
            //Console.ReadLine();


            //            <? xml version = "1.0" encoding = "GBK" ?>
            //   < Kp >
            //   < Version > 2.0 </ Version > //有此节点，则表示用带分类编码

            //     < Fpxx >

            //     < Zsl > 2 </ Zsl >                          //此文件含有的单据信息数量

            //     < Fpsj >

            //     < Fp >

            //         < Djh > 1 </ Djh >                    //单据号（20字节）

            //         < Gfmc > 购方名称 </ Gfmc >            //购方名称（100字节）

            //         < Gfsh > 110000000000000 </ Gfsh >  //购方税号

            //         < Gfyhzh > 购方银行账号 </ Gfyhzh >   //购方银行账号（100字节）

            //         < Gfdzdh > 购方地址电话 </ Gfdzdh >   //购方地址电话（100字节）

            //         < Bz > 备注 </ Bz >                   //备注（240字节）

            //         < Fhr > fhr </ Fhr >                    //复核人（8字节）

            //         < Skr > skr </ Skr >                    //收款人（8字节）
            //< Spbmbbh > 商品编码版本号 </ Spbmbbh >    //商品编码版本号(20字节)（必输项）
            //< Hsbz > 含税标志 </ Hsbz > //含税标志 0：不含税税率，1：含税税率，2：差额税;中外合作油气田（原海洋石油）5%税率、1.5%税率为1，差额税为2，其他为0；

            //         < Spxx >

            //             < Sph >

            //                 < Xh > 1 </ Xh >                //序号

            //                 < Spmc > 商品名称 </ Spmc >   //商品名称，金额为负数时此行是折扣行，折扣行的商品名称应与上一行的商品名称一致（100字节）

            //                 < Ggxh > 规格型号 </ Ggxh >   //规格型号（40字节）

            //                 < Jldw > 计量单位 </ Jldw >   //计量单位（32字节）
            //< Spbm > 商品编码 </ Spbm >   //商品编码(19字节)（必输项）
            //< Qyspbm > 企业商品编码 </ Qyspbm > //企业商品编码（20字节）
            //< Syyhzcbz > 优惠政策标识 </ Syyhzcbz > //是否使用优惠政策标识0：不使用，1：使用（1字节）
            //< Lslbz > 零税率标识 </ Lslbz >   //零税率标识   空：非零税率，0：出口退税，1：免税，2：不征收，3普通零税率（1字节）
            //< Yhzcsm > 优惠政策说明 </ Yhzcsm >   //优惠政策说明（50字节）

            //                 < Dj > 100 </ Dj >             //单价（中外合作油气田（原海洋石油）5%税率，单价为含税单价）

            //                 < Sl > 10 </ Sl >          //数量

            //                 < Je > 1000.00 </ Je >         //金额，当金额为负数时为折扣行

            //                 < Slv > 0.17 </ Slv >      //税率
            //< Kce > 扣除额 </ Kce >      //扣除额，用于差额税计算

            //             </ Sph >

            //         </ Spxx >

            //     </ Fp >

            //     < Fp >

            //         < Djh > 2 </ Djh >     //单据号（20字节）

            //         < Gfmc > 购方名称1 </ Gfmc >        //购方名称（100字节）

            //         < Gfsh > 110000000000000 </ Gfsh > //购方税号

            //         < Gfyhzh > 购方银行账号1 </ Gfyhzh > //购方银行账号（100字节）

            //         < Gfdzdh > 购方地址电话1 </ Gfdzdh > //购方地址电话（100字节）

            //         < Bz > 备注1 </ Bz >                  //备注（240字节）

            //         < Fhr > fhr1 </ Fhr >               //复核人（8字节）

            //         < Skr > skr1 </ Skr >               //收款人（8字节）
            //< Spbmbbh > 商品编码版本号 </ Spbmbbh >    //商品编码版本号(20字节) （必输项）
            //< Hsbz > 含税标志 </ Hsbz > //含税标志 0：不含税税率，1：含税税率，2：差额税;中外合作油气田（原海洋石油）5%税率、1.5%税率为1，差额税为2，其他为0；

            //         < Spxx >

            //             < Sph >

            //                 < Xh > 1 </ Xh >             //序号

            //                 < Spmc > 商品名称1 </ Spmc >   //商品名称，金额为负数时此行是折扣行，折扣行的商品名称应与上一行的商品名称一致（100字节）

            //                 < Ggxh > 规格型号1 </ Ggxh >   //规格型号（40字节）

            //                 < Jldw > 计量单位1 </ Jldw >   //计量单位（32字节）
            //< Spbm > 商品编码 </ Spbm >   //商品编码(19字节) （必输项）
            //< Qyspbm > 企业商品编码 </ Qyspbm > //企业商品编码（20字节）
            //< Syyhzcbz > 优惠政策标识 </ Syyhzcbz > //是否使用优惠政策标识0：不使用，1：使用（1字节）
            //< Lslbz > 零税率标识 </ Lslbz >   //零税率标识   空：非零税率，0：出口退税，1：免税，2：不征收，3普通零税率（1字节）
            //< Yhzcsm > 优惠政策说明 </ Yhzcsm >   //优惠政策说明（50字节）

            //                 < Dj > 100 </ Dj >               //单价（中外合作油气田（原海洋石油）5%税率，单价为含税单价）

            //                 < Sl > 10 </ Sl >            //数量

            //                 < Je > 1000.00 </ Je >           //金额，当金额为负数时为折扣行

            //                 < Slv > 0.17 </ Slv >          //税率
            //< Kce > 扣除额 </ Kce >      //扣除额，用于差额税计算

            //             </ Sph >

            //         </ Spxx >

            //     </ Fp >

            //     </ Fpsj >

            //     </ Fpxx >
            //   </ Kp >




        }

        /// <summary>    
        /// 创建节点    
        /// </summary>    
        /// <param name="xmldoc"></param>  xml文档  
        /// <param name="parentnode"></param>父节点    
        /// <param name="name"></param>  节点名  
        /// <param name="value"></param>  节点值  
        ///   
        public static void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }
        /// <summary>
        /// 商品明细表结构
        /// </summary>
        private class oms_item
        {
            public string product_code;
            public string sku_name;
            public int num;
            public float real_payment;
        }
        //标记oms
        private void barButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //buildInvoiceFile();
        }
    }
}
