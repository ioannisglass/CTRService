using Microsoft.Win32;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CTRClient.PWebHelper
{
    public partial class PuWebHelper
    {
        private Browser m_clsBrowser { get; set; }
        // private LaunchOptions m_varBroOpts { get; set; }

        public PuWebHelper()
        {
            
        }

        public string getChromePath()
        {
            string w_strChromePath = string.Empty;
            string w_strChromeReg = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";
            RegistryKey registryKey;
            using (registryKey = Registry.LocalMachine.OpenSubKey(w_strChromeReg))
            {
                if (registryKey != null)
                    w_strChromePath = registryKey.GetValue("Path").ToString() + @"\chrome.exe";
            }
            if (w_strChromePath == "")
            {
                if (Environment.Is64BitOperatingSystem)
                    w_strChromePath = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
                else
                    w_strChromePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";

                if (!System.IO.File.Exists(w_strChromePath))
                {
                    w_strChromePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\Application\chrome.exe";
                }
            }
            return w_strChromePath;
        }

        public async Task<bool> start()
        {
            try
            {
                string w_strChromePath = getChromePath();
                if (!System.IO.File.Exists(w_strChromePath))
                {
                    // MainApp.log_error($"#{m_ID} - chrome.exe Not found. Perhaps the Google Chrome browser is not installed on this computer.");
                    return false;
                }

                LaunchOptions w_varBroOpts = new LaunchOptions
                {
                    Headless = false,
                    ExecutablePath = w_strChromePath
                };

                var browser = await Puppeteer.LaunchAsync(w_varBroOpts);
                var page = await browser.NewPageAsync();
                await page.GoToAsync("https://ip.me/");
                // 
                //     Console.WriteLine("Generating PDF");
                //     await page.PdfAsync(Path.Combine(Directory.GetCurrentDirectory(), "google.pdf"));
                // 
                //     Console.WriteLine("Export completed");
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
