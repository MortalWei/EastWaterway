using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace EastWaterway
{
    public partial class FmMain : Form
    {
        JObject Cookie { get; set; }

        JObject DataSource { get; set; }

        const string SuccessStr = "登录成功!";

        bool LoginFlag = false;

        object bLock = new object();

        public FmMain()
        {
            InitializeComponent();
            toolStripTextBox2.TextBox.PasswordChar = '*';
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
                    FunMsg.Warn("请输入用户名");
                    toolStripTextBox1.Focus();
                    return;
                }

                else if (string.IsNullOrEmpty(userPWD))
                {
                    FunMsg.Warn("请输入密码");
                    toolStripTextBox2.Focus();
                    return;
                }

                Login(userName, userPWD);
            }
        }

        private void tsBtnLogout_Click(object sender, EventArgs e)
        {
        }

        private void Login(string userName, string userPWD)
        {
            var baseUrl = "http://www.012395.com/e/enews/index.php";

            List<Tuple<string, string>> list = new List<Tuple<string, string>>();
            list.Add(new Tuple<string, string>("ecmsfrom", ""));
            list.Add(new Tuple<string, string>("enews", "login"));
            list.Add(new Tuple<string, string>("username", userName));
            list.Add(new Tuple<string, string>("password", userPWD));
            list.Add(new Tuple<string, string>("lifetime", "0"));
            list.Add(new Tuple<string, string>("登录", ""));
            list.Add(new Tuple<string, string>("ecmsfrom", "../member/cp/"));

            var response = HttpHelper.PostApiNew(baseUrl, "", list);


            if (response == "登录失败")
            {
                FunMsg.Error("登录失败");
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
                }
                else
                {
                    FunMsg.Error(msg);
                    return;
                }
            }

            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //{
            //    FunMsg.Error(response.ErrorMessage);
            //    return;
            //}

            //var xmlDoc = new System.Xml.XmlDocument();

            //var start = response.Content.IndexOf("<table");
            //var end = response.Content.IndexOf("</table>");

            //var num = end - start + 8;
            //var xmlStr = response.Content.Substring(start, num);
            //xmlStr = xmlStr.Replace("<br>", "");
            //xmlDoc.LoadXml(xmlStr);
            //var node = xmlDoc.SelectSingleNode("//b");

            //if (node != null)
            //{
            //    var str = node.InnerText;
            //    if (str.Equals(SuccessStr))
            //    {
            //        SetCookie(response.Cookies.ToList());
            //        OpenEditor();
            //    }
            //    else
            //    {
            //        FunMsg.Error(str);
            //        return;
            //    }
            //}
            //else
            //{
            //    FunMsg.Error("服务端数据结构变更，需要更新客户端。");
            //    return;
            //}
        }

        private void SetCookie(List<RestResponseCookie> list)
        {
            Cookie = new JObject();
            var num = 0;
            list.ForEach(c =>
            {
                if (num > 0)
                {
                    Cookie.Add(c.Name, c.Value);
                }
                num++;
            });
        }

        private void OpenEditor()
        {
            groupBox2.Enabled = true;
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
                FunMsg.Warn("请输入要发布的内容");
                return;
            };

            var orders = richTextBox1.Text.Split('\n');

            if (orders.Length < 12)
            {
                FunMsg.Warn("数据填写不全，请补全后重新提交！");
                return;
            }
            DataSource = new JObject();

            if (chkSimple.Checked)
                SimpleModel(orders);
            else
                StandardModel(orders);

            var basrUrl = "http://www.012395.com/e/DoInfo/AddInfo.php?mid=15&enews=MAddInfo&classid=115&Submit=%E6%B7%BB%E5%8A%A0%E4%BF%A1%E6%81%AF";

            var response = HttpHelper.GetApiNew(basrUrl);

            if (response == "添加失败")
            {
                FunMsg.Error("添加失败\r\n服务端异常，请稍后再试.");
                return;
            }

            //TODO:Add faild process

            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(response);

            DataSource.Add("enews", GetNodeVal(html, "enews"));
            DataSource.Add("classid", GetNodeVal(html, "classid"));
            DataSource.Add("id", GetNodeValOfId(html, "id"));
            DataSource.Add("filepass", GetNodeVal(html, "filepass"));
            DataSource.Add("mid", GetNodeValOfId(html, "mid"));
            DataSource.Add("tokenid", GetNodeVal(html, "tokenid"));

            var postUrl = "http://www.012395.com/e/DoInfo/ecms.php";
                                 
            var result = HttpHelper.PostFormApi(postUrl, Cookie, DataSource);

            var strin = JsonConvert.SerializeObject(DataSource);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {

            }
        }

        private void StandardModel(string[] orders)
        {
            throw new NotImplementedException();
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

        private string GetNodeValOfId(HtmlAgilityPack.HtmlDocument html, string args)
        {
            var node = html.GetElementbyId(args);
            var arr = node.OuterHtml.Split('"');
            return arr[3];
        }
    }
}
