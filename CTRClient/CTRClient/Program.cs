using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CTRClient
{
    internal static class Program
    {
        public static System.Object g_locker = new object();
        public static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static frmLog g_log_frm = null;
        public static bool g_show_log_frm = false;
        public static string g_full_log = "";
        public static Form1 g_main_frm = null;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            g_main_frm = new Form1();
            Application.Run(g_main_frm);
        }

        public static void show_log_window()
        {
            if (!g_show_log_frm)
            {
                Thread thread = new Thread(() =>
                {
                    g_log_frm = new frmLog();

                    g_log_frm.ShowDialog();
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                g_show_log_frm = true;
            }
            else
            {
                if (g_log_frm != null)
                {
                    g_log_frm.Invoke(new Action(() => { g_log_frm.Activate(); }));
                }
            }
        }
        public static void close_log_window()
        {
            if (g_log_frm != null)
            {
                g_log_frm.Invoke(new Action(() => { g_log_frm.Close(); }));
            }
        }
        public static void log(string msg, string logtype, bool msgbox = false)
        {
            lock (g_locker)
            {
                try
                {
                    if (logtype == "error")
                        logger.Error(msg);
                    else
                        logger.Info(msg);

                    if (msgbox)
                        MessageBox.Show(msg);

                    msg = DateTime.Now.ToString("dd.MM.yyyy_hh:mm:ss ") + msg;
                    g_main_frm.log(msg, logtype);
                }
                catch (Exception ex)
                {

                }
            }
        }

        public static void log_info(string msg, bool msgbox = false)
        {
            log(msg, "info", msgbox);
        }

        public static void log_error(string msg, bool msgbox = false)
        {
            log(msg, "error", msgbox);
        }
        public static void log_todo(string msg)
        {
            log(msg, "todo", false);
        }
    }
}
