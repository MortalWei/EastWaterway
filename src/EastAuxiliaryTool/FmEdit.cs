using EastWaterway;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EastAuxiliaryTool
{
    public partial class FmEdit : Form
    {
        EnumMode Mode { get; set; }

        const string DefaultTitle = "信息采集：";

        public Goods DataSource { get; set; }

        public FmEdit()
        {
            InitializeComponent();
        }

        public FmEdit(EnumMode mode) : this()
        {
            Mode = mode;
            switch (Mode)
            {
                case EnumMode.货源一:
                    this.Text = DefaultTitle + "货源信息模式一";
                    break;
                case EnumMode.货源二:
                    this.Text = DefaultTitle + "货源信息模式二";
                    break;
                case EnumMode.船源一:
                    this.Text = DefaultTitle + "船源信息模式一";
                    break;
                case EnumMode.船源二:
                    this.Text = DefaultTitle + "船源信息模式二";
                    break;
                default:
                    this.Text = "异常模式";
                    button1.Enabled = false;
                    button1.Visible = false;
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(richTextBox1.Text))
            {
                FuncMsg.Warn("请先填写内容");
                return;
            }
            ProcessData(richTextBox1.Text);
        }

        private void ProcessData(string args)
        {
            DataSource = new Goods();
            try
            {
                switch (Mode)
                {
                    case EnumMode.货源一:
                        ProMode1(args);
                        break;
                    case EnumMode.货源二:
                        ProMode2(args);
                        break;
                    case EnumMode.船源一:
                        ProMode3(args);
                        break;
                    case EnumMode.船源二:
                        ProMode4(args);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ProMode2(string args)
        {
            var idx1 = args.IndexOf("名称");
            var idx2 = args.IndexOf("运价");
            var plac = 0;
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[货源信息]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
            }
            plac = 2;
            DataSource.Title = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("吨位");
            idx2 = args.IndexOf("发出港");
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[吨位]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
            }
            plac = 2;
            DataSource.Weight = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("发出港");
            idx2 = args.IndexOf("所属省市");
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[发出港]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
            }
            plac = 3;
            DataSource.Depart = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("到达港");
            idx2 = args.IndexOf("到达省市");
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[到达港]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
            }
            plac = 3;
            DataSource.Arrive = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("联系人");
            idx2 = args.IndexOf("手机");
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[联系人]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
            }
            plac = 4;
            DataSource.Person = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("手机");
            idx2 = args.IndexOf("QQ号");
            if (idx2 == idx1 || (idx2 > -1 && idx1 > idx2))
            {
                throw new Exception("[联系电话]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式二”中的内容示例");
            }
            plac = 3;
            DataSource.Phone = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            this.DialogResult = DialogResult.OK;
        }

        private void ProMode1(string args)
        {
            var idx1 = args.IndexOf("货源名称");
            var idx2 = args.IndexOf("航线类别");
            var plac = 0;
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[货源信息]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }
            plac = 4;
            DataSource.Title = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("吨位");
            idx2 = args.IndexOf("发出港");
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[吨位]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }
            plac = 2;
            DataSource.Weight = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("发出港");
            idx2 = args.IndexOf("到达港");
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[发出港]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }
            plac = 3;
            DataSource.Depart = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("到达港");
            idx2 = args.IndexOf("发布时间");
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[到达港]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }
            plac = 3;
            DataSource.Arrive = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("联系人");
            idx2 = args.IndexOf("联系电话");
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[联系人]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }
            plac = 4;
            DataSource.Person = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("联系电话");
            idx2 = args.IndexOf("备注");
            if (idx2 == idx1 || (idx2 > -1 && idx1 > idx2))
            {
                throw new Exception("[联系电话]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }

            string phone;
            if (idx2 < 0)
            {
                phone = args.Substring(idx1 + 5).Trim();
            }
            else
            {
                plac = 4;
                phone = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();
            }
            DataSource.Phone = phone;

            this.DialogResult = DialogResult.OK;
        }

        private void ProMode3(string args)
        {
            var idx1 = args.IndexOf("船舶吨位");
            var idx2 = args.IndexOf("发出港");
            var plac = 0;
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[船舶吨位]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }
            plac = 4;
            DataSource.Weight = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("发出港");
            idx2 = args.IndexOf("到达港");
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[发出港]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }
            plac = 3;
            DataSource.Depart = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("到达港");
            idx2 = args.IndexOf("发布时间");
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[到达港]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }
            plac = 3;
            DataSource.Arrive = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("联系人");
            idx2 = args.IndexOf("联系电话");
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[联系人]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }
            plac = 4;
            DataSource.Person = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("联系电话");
            idx2 = args.IndexOf("备注");
            if (idx2 == idx1 || (idx2 > -1 && idx1 > idx2))
            {
                throw new Exception("[联系电话]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }

            string phone;
            if (idx2 < 0)
            {
                phone = args.Substring(idx1 + 5).Trim();
            }
            else
            {
                plac = 4;
                phone = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();
            }
            DataSource.Phone = phone;

            this.DialogResult = DialogResult.OK;
        }

        private void ProMode4(string args)
        {
            var idx1 = args.IndexOf("吨位");
            var idx2 = args.IndexOf("类型");
            var plac = 0;
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[船舶吨位]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }
            plac = 4;
            DataSource.Weight = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("所在港");
            idx2 = args.IndexOf("到达港");
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[发出港]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }
            plac = 3;
            DataSource.Depart = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("到达港");
            idx2 = args.IndexOf("吨位");
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[到达港]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }
            plac = 3;
            DataSource.Arrive = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();

            idx1 = args.IndexOf("联系人");
            idx2 = args.IndexOf("手机");
            if (idx2 == idx1 || idx1 > idx2)
            {
                throw new Exception("[联系人]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            }
            plac = 4;
            DataSource.Person = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();
            DataSource.Phone = args.Substring(idx2 + 3).Trim();

            //idx1 = args.IndexOf("手机");
            //idx2 = args.IndexOf("备注");
            //if (idx2 == idx1 || (idx2 > -1 && idx1 > idx2))
            //{
            //    throw new Exception("[联系电话]检测失败，请检查待发布内容！\r\n可参考帮助界面中“货源信息模式一”中的内容示例");
            //}

            //string phone;
            //if (idx2 < 0)
            //{
            //    phone = args.Substring(idx1 + 3).Trim();
            //}
            //else
            //{
            //    plac = 4;
            //    phone = args.Substring(idx1 + plac, idx2 - idx1 - plac).Trim();
            //}
            //DataSource.Phone = phone;

            this.DialogResult = DialogResult.OK;
        }
    }
}
