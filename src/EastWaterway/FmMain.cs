using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace EastWaterway
{
    public partial class FmMain : Form
    {
        List<RestResponseCookie> Cookie;

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
            groupBox2.Enabled = true;
            richTextBox1.Focus();
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

            var response = HttpHelper.PostApi(baseUrl, "", list);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                FunMsg.Error(response.ErrorMessage);
                return;
            }

            var xmlDoc = new System.Xml.XmlDocument();

            var start = response.Content.IndexOf("<table");
            var end = response.Content.IndexOf("</table>");

            var num = end - start + 8;
            var xmlStr = response.Content.Substring(start, num);
            xmlStr = xmlStr.Replace("<br>", "");
            xmlDoc.LoadXml(xmlStr);
            var node = xmlDoc.SelectSingleNode("//b");

            if (node != null)
            {
                var str = node.InnerText;
                if (str.Equals(SuccessStr))
                {
                    Cookie = response.Cookies.ToList();
                    OpenEditor();
                }
                else
                {
                    FunMsg.Error(str);
                    return;
                }
            }
            else
            {
                FunMsg.Error("服务端数据结构变更，需要更新客户端。");
                return;
            }
        }

        private void OpenEditor()
        {
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
            if (string.IsNullOrEmpty(richTextBox1.Text))
            {
                return;
            }

            var list = richTextBox1.Text.Split('\n');
            if (list.Length >= 12)
            {

            }
            else
            {
                return;
            }
        }
    }
}
