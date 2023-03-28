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

            new Thread((ThreadStart)(async () =>
            {
                await Task.Delay(100);
                bool w_bRet = await w_clsPuWebHelper.start();

            })).Start();

        }
    }
}
