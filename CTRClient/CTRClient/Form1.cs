using CTRClient.ChrDevToolsWebHelper;
using CTRClient.MainModule;
using CTRClient.PWebHelper;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CTRClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            // AutoPro w_clsAutoPro = new AutoPro();
            PuWebHelper w_clsPuWebHelper = new PuWebHelper();
            ChrWebHelper w_clsChrWebHelper = new ChrWebHelper();
            ChrRemoteWebHelper w_clsChrRemoteHelper = new ChrRemoteWebHelper();

            new Thread((ThreadStart)(async () =>
            {
                await Task.Delay(100);
                // bool w_bRet = await w_clsPuWebHelper.start();
                // bool w_bRet = await w_clsChrWebHelper.start();
                bool w_bRet = await w_clsChrRemoteHelper.start();

            })).Start();

        }
        public void show_log_window()
        {
            Program.g_log_frm.Show();
            Program.g_log_frm.Activate();
        }
        public void log(string msg, string logtype)
        {
            if (logtype != "todo")
            {
                Program.g_full_log += "\n" + msg;
                if (Program.g_log_frm != null)
                    Program.g_log_frm.update_log();
            }
            else
            {
                // m_ucTask.update_log(msg);
            }
            update_status(msg);
        }
        public void update_status(string msg)
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                // lblInstantLog.Text = msg;
            });
        }
    }
}
