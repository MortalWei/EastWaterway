using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace EastWaterway
{
    public partial class FmMainNew : Form
    {

        JObject DataSource { get; set; }

        const string SuccessStr = "登录成功!";

        bool LoginFlag = false;

        object bLock = new object();

        string m_Mode = "普通模式";
        string Mode
        {
            get { return m_Mode; }
            set
            {
                switch (value)
                {
                    case "普通模式":
                        //radioButton1.Checked = true;
                        break;
                    case "简洁模式":
                        //radioButton2.Checked = true;
                        break;
                    case "标准模式":
                        //radioButton3.Checked = true;
                        break;
                }
            }
        }

        WebApiClient ApiClient { get; set; }

        public FmMainNew()
        {
            InitializeComponent();
            toolStripTextBox2.TextBox.PasswordChar = '*';
            ApiClient = new WebApiClient();
        }

        private void tsBtnLogin_Click(object sender, EventArgs e)
        {
            if (LoginFlag) return;
            lock (bLock)
            {
                if (LoginFlag) return;
                var userName = toolStripTextBox1.Text;
                var userPWD = toolStripTextBox2.Text;

                if (string.IsNullOrEmpty(userName))
                {
                    FuncMsg.Warn("请输入用户名");
                    toolStripTextBox1.Focus();
                    return;
                }

                else if (string.IsNullOrEmpty(userPWD))
                {
                    FuncMsg.Warn("请输入密码");
                    toolStripTextBox2.Focus();
                    return;
                }

                Login(userName, userPWD);
            }
        }

        private void tsBtnLogout_Click(object sender, EventArgs e)
        {
            lock (bLock)
            {
                var baseUrl = "http://www.012395.com/e/enews/?enews=exit&ecmsfrom=/e/member/login/";

                var response = ApiClient.Logout(baseUrl);

                groupBox2.Enabled = false;
                LoginFlag = false;
                tsBtnLogin.Enabled = true;
            }
        }

        private void Login(string userName, string userPWD)
        {
            var baseUrl = "http://www.012395.com/e/enews/index.php";

            var response = ApiClient.Login(baseUrl, userName, userPWD);

            if (response == FuncConst.LoginErr)
            {
                FuncMsg.Error($"{FuncConst.LoginErr}\r\n{FuncConst.NetworkErr}");
                return;
            }

            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(response);

            var node = html.DocumentNode.SelectSingleNode("//b");
            if (node != null)
            {
                var msg = node.InnerText;
                if (msg.Equals(SuccessStr))
                {
                    OpenEditor();
                    RefreshLately();
                }
                else
                {
                    FuncMsg.Error(msg);
                    return;
                }
            }
        }

        /// <summary>
        /// 刷新我的发布
        /// </summary>
        private void RefreshLately()
        {
            var baseUrl = "http://www.012395.com/e/DoInfo/ListInfo.php?mid=15";
            var response = ApiClient.RefreshLately(baseUrl);
            if (response.Equals(FuncConst.RefreshErr))
            {
                FuncMsg.Error($"{FuncConst.RefreshErr}\r\n{FuncConst.NetworkErr}");
                return;
            }

            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(response);

            var node = html.DocumentNode.SelectSingleNode("//title");

            if (node != null)
            {
                var title = node.InnerText;
                if (title == FuncConst.ExTitle)
                {
                    node = html.DocumentNode.SelectSingleNode("//b");
                    if (node != null)
                    {
                        FuncMsg.Error(node.InnerText);
                        return;
                    }
                }
                else
                {
                    var nodes = html.DocumentNode.SelectNodes("//*[contains(@class,'table_list')]");
                    var list = new List<ListViewItem>();
                    listView1.Items.Clear();
                    foreach (var item in nodes)
                    {
                        var tds = item.SelectNodes("tr/td");

                        var ctl = new ListViewItem();
                        ctl.Text = tds[0].InnerText;
                        ctl.SubItems.Add(tds[3].InnerText);
                        list.Add(ctl);
                        listView1.Items.Add(ctl);
                    }
                }
            }
        }

        private void OpenEditor()
        {
            groupBox2.Enabled = true;
            tsBtnLogout.Enabled = true;
            tsBtnLogin.Enabled = false;
            richTextBox1.Focus();
        }

        private void FmMain_Load(object sender, EventArgs e)
        {
        }

        private void FmMain_Shown(object sender, EventArgs e)
        {
            toolStripTextBox1.Focus();

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var str = richTextBox1.Text;
            var dd = str.Split('\n');
            MessageBox.Show(dd.Length.ToString());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(richTextBox1.Text))
            {
                FuncMsg.Warn("请输入要发布的内容");
                return;
            };

            //if (chkSimple.Checked)
            //{
            if (OnSave(richTextBox1.Text))
            {
                RefreshLately();
                richTextBox1.Text = "";
            }
            //}
        }

        private bool InitSave()
        {
            var baseUrl = "http://www.012395.com/e/DoInfo/AddInfo.php?mid=15&enews=MAddInfo&classid=115&Submit=%E6%B7%BB%E5%8A%A0%E4%BF%A1%E6%81%AF";

            var response = ApiClient.AddInfo(baseUrl);

            if (response == FuncConst.AddErr)
            {
                FuncMsg.Error($"{FuncConst.AddErr}\r\n{FuncConst.NetworkErr}");
                return false;
            }

            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(response);

            if (!IsLogin(html)) return false;

            DataSource.Add("enews", GetNodeVal(html, "enews"));
            DataSource.Add("classid", GetNodeVal(html, "classid"));
            DataSource.Add("id", "");
            DataSource.Add("filepass", GetNodeVal(html, "filepass"));
            DataSource.Add("mid", "15");
            DataSource.Add("tokenid", "");
            return true;
        }

        private bool OnSave(string args)
        {
            var orders = args.Split('\n');
            if (orders.Length < 12)
            {
                FuncMsg.Warn("数据填写不全，请补全后重新提交！");
                return false;
            }

            DataSource = new JObject();

            if (!InitSave()) return false;

            if (chkSimple.Checked)
                SimpleModel(orders);
            else
                StandardModel(orders);

            return OnCallSave();
        }

        private bool OnCallSave()
        {
            var postUrl = "http://www.012395.com/e/DoInfo/ecms.php";

            var result = ApiClient.SaveInfo(postUrl, DataSource);

            if (IsSuccess(result))
            {
                FuncMsg.Info($"货源信息：[{DataSource["title"].ToString()}]发布成功!");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        /// <param name="htmlStr"></param>
        /// <returns></returns>
        private bool IsSuccess(string htmlStr)
        {
            if (htmlStr == FuncConst.AddErr)
            {
                FuncMsg.Error($"{FuncConst.AddErr}\r\n{FuncConst.NetworkErr}");
                return false;
            }

            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(htmlStr);
            var node = html.DocumentNode.SelectSingleNode("//title");
            if (node != null)
            {
                var title = node.InnerText;
                if (title == FuncConst.ExTitle)
                {
                    node = html.DocumentNode.SelectSingleNode("//b");
                    if (node != null)
                    {
                        var msg = node.InnerText;
                        if (msg == "提交信息成功")
                        {
                            return true;
                        }
                        else
                        {
                            FuncMsg.Error(msg);
                            return false;
                        }
                    }
                }
            }
            FuncMsg.Error("其它错误,请稍后重试。");
            return false;
        }

        private bool IsLogin(HtmlAgilityPack.HtmlDocument html)
        {
            var node = html.DocumentNode.SelectSingleNode("//title");
            if (node != null)
            {
                var title = node.InnerText;
                if (title == FuncConst.ExTitle)
                {
                    node = html.DocumentNode.SelectSingleNode("//b");
                    var msg = "您的账号在其它地方登录，请重新登录后重新发布";
                    if (node != null)
                    {
                        msg = node.InnerText + "\r\n请重新登录后再次发布";
                    }
                    FuncMsg.Error(msg);
                    groupBox2.Enabled = false;
                    toolStripTextBox1.Focus();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否切换模式
        /// </summary>
        /// <returns></returns>
        private bool IsSwitch()
        {
            return FuncMsg.YesNo("切换模式当清空当前已经填写的内容\r\n您确定要切换吗？");
        }

        private void StandardModel(string[] orders)
        {
            // 名称
            DataSource.Add("title", GetStandardVal(orders[0]));
            // 吨位
            DataSource.Add("h_dunwei", GetStandardVal(orders[1]));
            // 发出地
            DataSource.Add("h_fachu", GetStandardVal(orders[2]));
            // 到达地
            DataSource.Add("h_daoda", GetStandardVal(orders[3]));
            // 包装
            DataSource.Add("h_xings", GetStandardVal(orders[4]));
            // 哪条水路
            DataSource.Add("h_leix", GetStandardVal(orders[5]));
            // 月
            DataSource.Add("h_yue", GetStandardVal(orders[6]).Split('.')[0]);
            // 日
            DataSource.Add("h_ri", GetStandardVal(orders[6]).Split('.')[1]);
            // 月
            DataSource.Add("h_yue1", GetStandardVal(orders[7]).Split('.')[0]);
            // 日
            DataSource.Add("h_ri1", GetStandardVal(orders[7]).Split('.')[1]);
            // 有效期
            DataSource.Add("h_xiaoqi", GetStandardVal(orders[8]));
            // 联系人
            DataSource.Add("hy_lxr", GetStandardVal(orders[9]));
            // 电话
            DataSource.Add("hy_lxdh", GetStandardVal(orders[10]));
            // 备注
            var remark = "";
            for (int i = 11; i < orders.Length; i++)
            {
                if (i == 11)
                    remark += GetStandardVal(orders[i]);
                remark += orders[i];
            }
            DataSource.Add("h_conter", remark);
        }

        /// <summary>
        /// 获取标准模式的值
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private string GetStandardVal(string args)
        {
            var arr = args.Split('|');
            if (arr.Length == 2)
            {
                return arr[1].TrimStart();
            }
            else
            {
                throw new Exception($"{args}内容格式不正确,应该为：[名称| 内容]");
            }
        }

        /// <summary>
        /// 简单模式
        /// </summary>
        /// <param name="orders"></param>
        private void SimpleModel(string[] orders)
        {
            // 名称
            DataSource.Add("title", orders[0]);
            // 吨位
            DataSource.Add("h_dunwei", orders[1]);
            // 发出地
            DataSource.Add("h_fachu", orders[2]);
            // 到达地
            DataSource.Add("h_daoda", orders[3]);
            // 包装
            DataSource.Add("h_xings", orders[4]);
            // 哪条水路
            DataSource.Add("h_leix", orders[5]);
            // 月
            DataSource.Add("h_yue", orders[6].Split('.')[0]);
            // 日
            DataSource.Add("h_ri", orders[6].Split('.')[1]);
            // 月
            DataSource.Add("h_yue1", orders[7].Split('.')[0]);
            // 日
            DataSource.Add("h_ri1", orders[7].Split('.')[1]);
            // 有效期
            DataSource.Add("h_xiaoqi", orders[8]);
            // 联系人
            DataSource.Add("hy_lxr", orders[9]);
            // 电话
            DataSource.Add("hy_lxdh", orders[10]);
            // 备注
            var remark = "";
            for (int i = 11; i < orders.Length; i++)
            {
                remark += orders[i];
            }
            DataSource.Add("h_conter", remark);
        }

        private string GetNodeVal(HtmlAgilityPack.HtmlDocument html, string args)
        {
            var node = html.DocumentNode.SelectSingleNode($"//input[@name='{args}']");
            var arr = node.OuterHtml.Split('"');
            return arr[3];
        }

        private void chkSimple_CheckedChanged(object sender, EventArgs e)
        {
            //if (!IsSwitch()) return;
            if (chkSimple.Checked)
            {
                richTextBox2.Visible = btnSave.Visible = true;
                btnSave.Dock = DockStyle.Fill;
                button1.Visible = button2.Visible = false;
            }
            else
            {
                richTextBox2.Visible = btnSave.Visible = false;
                button1.Visible = button2.Visible = true;
            }
        }

        /// <summary>
        /// 货源信息发布：模式一
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "") { return; }
            DataSource = new JObject();

            //OnSaveNew();
            if (!InitSave()) return;

            if (!Issue1(richTextBox1.Text)) return;

            if (OnCallSave())
            {
                RefreshLately();
                richTextBox1.Text = "";
            }
        }

        /// <summary>
        /// 货源信息发布：模式二
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "") { return; }
            DataSource = new JObject();
            if (!InitSave()) return;
            if (!Issue2(richTextBox1.Text)) return;

            if (OnCallSave())
            {
                RefreshLately();
                richTextBox1.Text = "";
            }
        }

        /// <summary>
        /// 货源信息发布：模式1
        /// </summary>
        /// <param name="args"></param>
        private bool Issue1(string args)
        {
            /*货源名称煤炭航线类别长江-内河所需船型单机货物吨位5700发出港镇江　到达港芜湖　发布时间2021-03-19装运期限2021-03-19至2021-03-19浏览次数6次运价元/吨联系人： 顾孝飞
联系电话：15896025008 */

            var idx1 = args.IndexOf("货源名称");
            var idx2 = args.IndexOf("航线类别");
            var plac = 0;
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[货源信息]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
                return false;
            }
            plac = 4;
            DataSource.Add("title", args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim());

            idx1 = args.IndexOf("吨位");
            idx2 = args.IndexOf("发出港");
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[吨位]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
                return false;
            }
            plac = 2;
            DataSource.Add("h_dunwei", args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim());

            idx1 = args.IndexOf("发出港");
            idx2 = args.IndexOf("到达港");
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[发出港]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
                return false;
            }
            plac = 3;
            DataSource.Add("h_fachu", args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim());

            idx1 = args.IndexOf("到达港");
            idx2 = args.IndexOf("发布时间");
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[到达港]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
                return false;
            }
            plac = 3;
            DataSource.Add("h_daoda", args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim());

            DataSource.Add("h_xings", "散装");
            DataSource.Add("h_leix", "内河");

            idx1 = args.IndexOf("装运期限");
            idx2 = args.IndexOf("至");
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[装运期限（起始）]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
                return false;
            }
            plac = 4;
            var startDate = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();
            var tuple = startDate.ToDateStr();

            // 月
            DataSource.Add("h_yue", tuple.Item1);
            // 日
            DataSource.Add("h_ri", tuple.Item2);

            idx1 = args.IndexOf("至");
            idx2 = args.IndexOf("浏览次数");
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[装运期限（结束）]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
                return false;
            }
            plac = 1;
            var endDate = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();
            var tuple2 = endDate.ToDateStr();
            // 月
            DataSource.Add("h_yue1", tuple2.Item1);
            // 日
            DataSource.Add("h_ri1", tuple2.Item2);
            // 有效期
            DataSource.Add("h_xiaoqi", "7天");

            idx1 = args.IndexOf("联系人");
            idx2 = args.IndexOf("联系电话");
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[联系人]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
                return false;
            }
            plac = 4;
            DataSource.Add("hy_lxr", args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim());

            idx1 = args.IndexOf("联系电话");
            idx2 = args.IndexOf("备注");
            if (idx2 == idx1 || (idx2 > -1 && idx1 > idx2))
            {
                FuncMsg.Error("[联系电话]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
                return false;
            }

            var phone = "";
            var remark = "";
            if (idx2 < 0)
            {
                phone = args.Substring(idx1 + 5).Trim();
            }
            else
            {
                plac = 4;
                phone = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

                remark = args.Substring(idx2 + 3);
            }
            DataSource.Add("hy_lxdh", phone);
            DataSource.Add("h_conter", remark);

            return true;
        }

        /// <summary>
        /// 货源信息发布：模式2
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool Issue2(string args)
        {
            var idx1 = args.IndexOf("名称");
            var idx2 = args.IndexOf("运价");
            var plac = 0;
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[货源信息]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
                return false;
            }
            plac = 2;
            DataSource.Add("title", args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim());

            idx1 = args.IndexOf("吨位");
            idx2 = args.IndexOf("发出港");
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[吨位]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
                return false;
            }
            plac = 2;
            DataSource.Add("h_dunwei", args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim());

            idx1 = args.IndexOf("发出港");
            idx2 = args.IndexOf("所属省市");
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[发出港]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
                return false;
            }
            plac = 3;
            DataSource.Add("h_fachu", args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim());

            idx1 = args.IndexOf("到达港");
            idx2 = args.IndexOf("到达省市");
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[到达港]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
                return false;
            }
            plac = 3;
            DataSource.Add("h_daoda", args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim());

            idx1 = args.IndexOf("包装");
            idx2 = args.IndexOf("类型");
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[包装形式]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
                return false;
            }
            plac = 2;
            DataSource.Add("h_xings", args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim());

            idx1 = args.IndexOf("类型");
            idx2 = args.IndexOf("发货日期");
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[航运类型]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
                return false;
            }
            plac = 2;
            DataSource.Add("h_leix", args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim());

            idx1 = args.IndexOf("发货日期");
            idx2 = args.IndexOf("截止日期");
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[装运期限（起始）]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
                return false;
            }
            plac = 4;
            var startDate = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();
            var tuple = startDate.ToDateStr();

            // 月
            DataSource.Add("h_yue", tuple.Item1);
            // 日
            DataSource.Add("h_ri", tuple.Item2);

            idx1 = args.IndexOf("截止日期");
            idx2 = args.IndexOf("是否交易");
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[装运期限（结束）]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
                return false;
            }
            plac = 4;
            var endDate = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();
            var tuple2 = endDate.ToDateStr();
            // 月
            DataSource.Add("h_yue1", tuple2.Item1);
            // 日
            DataSource.Add("h_ri1", tuple2.Item2);
            // 有效期
            DataSource.Add("h_xiaoqi", "7天");

            idx1 = args.IndexOf("联系人");
            idx2 = args.IndexOf("手机");
            if (idx2 == idx1 || idx1 > idx2)
            {
                FuncMsg.Error("[联系人]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
                return false;
            }
            plac = 4;
            DataSource.Add("hy_lxr", args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim());

            idx1 = args.IndexOf("手机");
            idx2 = args.IndexOf("QQ号");
            if (idx2 == idx1 || (idx2 > -1 && idx1 > idx2))
            {
                FuncMsg.Error("[联系电话]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
                return false;
            }
            plac = 3;
            DataSource.Add("hy_lxdh", args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim());

            idx1 = args.IndexOf("备注");
            idx2 = args.IndexOf("联系方式");
            if (idx2 == idx1 || (idx2 > -1 && idx1 > idx2))
            {
                FuncMsg.Error("[备注]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
                return false;
            }
            plac = 3;
            DataSource.Add("h_conter", args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim());

            return true;
        }
    }
}
