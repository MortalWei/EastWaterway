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
    public partial class FmMain : Form
    {
        public FmMain()
        {
            InitializeComponent();
            Browser.Url = new Uri("http://www.012395.com/e/member/login/");
        }

        private void FmMain_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 货源一
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            EditGoods(EnumMode.货源一);
        }

        /// <summary>
        /// 货源二
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            EditGoods(EnumMode.货源二);
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            EditGoods(EnumMode.货源三);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            EditGoods(EnumMode.货源四);
        }

        /// <summary>
        /// 船源一
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //EditShips(EnumMode.船源一);
            EditShips(EnumMode.船源一);
        }

        /// <summary>
        /// 船源二
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            //EditShips(EnumMode.船源二);
            EditShips(EnumMode.船源二);
        }

        /// <summary>
        /// 货源信息处理
        /// </summary>
        /// <param name="mode"></param>
        private void EditGoods(EnumMode mode)
        {
            if (!IsCanOpenEdit(mode)) { return; }

            using (var fm = new FmEdit(mode))
            {
                if (fm.ShowDialog() == DialogResult.OK)
                {
                    var good = fm.DataSource;
                    EditHtmlGoodData(good);
                }
            }
        }

        /// <summary>
        /// 船源信息处理
        /// </summary>
        /// <param name="mode"></param>
        private void EditShips(EnumMode mode)
        {
            if (!IsCanOpenEdit(mode)) { return; }

            using (var fm = new FmEdit(mode))
            {
                if (fm.ShowDialog() == DialogResult.OK)
                {
                    var good = fm.DataSource;
                    EditHtmlShipData(good);
                }
            }
        }

        private void EditHtmlShipData(Goods args)
        {
            //y_dunwei
            Browser.Document.GetElementById("y_dunwei").SetAttribute("value", args.Weight);
            //y_suozai
            Browser.Document.GetElementById("y_suozai").SetAttribute("value", args.Depart);
            //y_daoga
            Browser.Document.GetElementById("y_daoga").SetAttribute("value", args.Arrive);
            //y_lxr
            Browser.Document.GetElementById("y_lxr").SetAttribute("value", args.Person);
            //y_lxdh
            Browser.Document.GetElementById("y_lxdh").SetAttribute("value", args.Phone);
        }

        /// <summary>
        /// 修改页面值
        /// </summary>
        /// <param name="args"></param>
        private void EditHtmlGoodData(Goods args)
        {
            var ele = Browser.Document.GetElementsByTagName("input").GetElementsByName("title");
            if (ele.Count > 0)
            {
                ele[0].SetAttribute("value", args.Title);
            }

            //h_dunwei
            Browser.Document.GetElementById("h_dunwei").SetAttribute("value", args.Weight);
            //h_fachu
            Browser.Document.GetElementById("h_fachu").SetAttribute("value", args.Depart);
            //h_daoda
            Browser.Document.GetElementById("h_daoda").SetAttribute("value", args.Arrive);
            //hy_lxr
            Browser.Document.GetElementById("hy_lxr").SetAttribute("value", args.Person);
            //hy_lxdh
            Browser.Document.GetElementById("hy_lxdh").SetAttribute("value", args.Phone);
        }

        private bool IsCanOpenEdit(EnumMode mode)
        {
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            var str = Browser.DocumentText;
            document.LoadHtml(str);

            var table = document.DocumentNode.SelectSingleNode("//table");
            var content = table.InnerText;

            var flag = false;
            switch (mode)
            {
                case EnumMode.货源一:
                case EnumMode.货源二:
                case EnumMode.货源三:
                case EnumMode.货源四:
                    flag = content.Contains("栏目：货源信息");
                    if (!flag)
                    {
                        FuncMsg.Warn("请先进入[货源信息]编辑页面后再使用本功能");
                    }
                    break;
                case EnumMode.船源一:
                case EnumMode.船源二:
                    flag = content.Contains("栏目：船源信息");
                    if (!flag)
                    {
                        FuncMsg.Warn("请先进入[船源信息]编辑页面后再使用本功能");
                    }
                    break;
                default:
                    flag = false;
                    break;
            }
            return flag;
        }
    }
}
