using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace EastWaterway
{
    public partial class FmMain : Form
    {

        JObject DataSource { get; set; }

        const string SuccessStr = "登录成功!";

        bool LoginFlag = false;

        object bLock = new object();

        WebApiClient ApiClient { get; set; }

        public FmMain()
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

            var orders = richTextBox1.Text.Split('\n');
            if (orders.Length < 12)
            {
                FuncMsg.Warn("数据填写不全，请补全后重新提交！");
                return;
            }
            DataSource = new JObject();

            var baseUrl = "http://www.012395.com/e/DoInfo/AddInfo.php?mid=15&enews=MAddInfo&classid=115&Submit=%E6%B7%BB%E5%8A%A0%E4%BF%A1%E6%81%AF";

            var response = ApiClient.AddInfo(baseUrl);

            if (response == FuncConst.AddErr)
            {
                FuncMsg.Error($"{FuncConst.AddErr}\r\n{FuncConst.NetworkErr}");
                return;
            }

            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(response);

            if (!IsLogin(html)) return;

            DataSource.Add("enews", GetNodeVal(html, "enews"));
            DataSource.Add("classid", GetNodeVal(html, "classid"));
            DataSource.Add("id", "");
            DataSource.Add("filepass", GetNodeVal(html, "filepass"));
            DataSource.Add("mid", "15");
            DataSource.Add("tokenid", "");

            if (chkSimple.Checked)
                SimpleModel(orders);
            else
                StandardModel(orders);

            var postUrl = "http://www.012395.com/e/DoInfo/ecms.php";

            var result = ApiClient.SaveInfo(postUrl, DataSource);

            if (IsSuccess(result))
            {
                FuncMsg.Info($"货源信息：[{DataSource["title"].ToString()}]发布成功!");
                RefreshLately();
                if (chkSimple.Checked)
                {
                    richTextBox2.Visible = true;
                    SwitchSimpleModel();
                }
                else
                {
                    richTextBox2.Visible = false;
                    SwitchStandardModel();
                }
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

        /// <summary>
        /// 切换成标准模式
        /// </summary>
        private void SwitchStandardModel()
        {
            var sb = new StringBuilder();
            sb.AppendLine("货源名称| ");
            sb.AppendLine("货物吨位| ");
            sb.AppendLine("发出地(港)| ");
            sb.AppendLine("到达地(港)| ");
            sb.AppendLine("包装形式| 散装");
            sb.AppendLine("航运类型| 内河");
            sb.AppendLine("装运期限(开始)| " + DateTime.Now.ToString("M.d"));
            sb.AppendLine("装运期限(结束)| " + DateTime.Now.ToString("M.d"));
            sb.AppendLine("有效期| 7天");
            sb.AppendLine("联系人| ");
            sb.AppendLine("联系电话| ");
            sb.AppendLine("联系备注| ");
            richTextBox1.Text = sb.ToString();
            richTextBox1.SelectionStart = 6;
            richTextBox1.Focus();
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
        /// 切换成简单模式
        /// </summary>
        private void SwitchSimpleModel()
        {
            richTextBox1.Text = "";
        }

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
            if (!IsSwitch()) return;
            if (chkSimple.Checked)
            {
                richTextBox2.Visible = true;
                SwitchSimpleModel();
            }
            else
            {
                richTextBox2.Visible = false;
                SwitchStandardModel();
            }
        }

    }
}
