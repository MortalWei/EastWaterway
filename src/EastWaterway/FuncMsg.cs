using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EastWaterway
{
    public static class FuncMsg
    {
        public static void Error(string msg, string title = "错误")
        {
            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void Warn(string msg, string title = "提示")
        {
            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static bool YesNo(string msg, string title = "提示")
        {
            if (MessageBox.Show(msg, "title", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                return true;
            }
            return false;
        }
    }
}
