using ChromeRemoteSharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTRClient.ChrDevToolsWebHelper
{
    public class ChrRemoteWebHelper
    {
        public LowLevelDriver m_drvChrome = null;
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
                Helper.KillAllChromeInstances(); // kill all opened instances of google chrome
                Helper.StartChromeDevTools(); // start new chrome browser instance point to http://localhost:9222

                if (!await Helper.CheckWebSocketAsync()) // check if remote debugging url it's available
                    Helper.StartChromeHeadless(); // start a headless instance of google chrome

                var url = await Helper.FirstWebSocketDebuggerUrlAsync(); // get the first ws remote url
                m_drvChrome = new LowLevelDriver(url); // instance driver

                // navigate
                await m_drvChrome.Page.NavigateAsync("https://www.google.com"); // navigate
                System.Threading.Thread.Sleep(2000);

                // screenshot
                var screenShotJson = await m_drvChrome.Page.CaptureScreenshotAsync(); // capture image and return base64 json image
                Console.WriteLine(screenShotJson);

                // get version
                Console.WriteLine(await m_drvChrome.Browser.GetVersionAsync()); // get browser version

                // get browser command line
                Console.WriteLine(await m_drvChrome.Browser.GetBrowserCommandLineAsync()); // get all command lines of headless instance

                // page cookies
                Console.WriteLine(await m_drvChrome.Page.GetCookiesAsync()); // get all cookies

                // get document
                var docJson = await m_drvChrome.Dom.GetDocumentAsync(); // get document in raw format devtools protocol
                var nodeId = docJson["root"]["nodeId"].ToObject<int>();

                // get html
                var htmlJson = await m_drvChrome.Dom.GetOuterHTMLAsync(nodeId); // get source code of page
                var html = htmlJson["outerHTML"].ToString();
                Program.log_info(html);


                Console.ReadKey();
                m_drvChrome.CloseConnection();
            }
            catch (Exception ex)
            {
                Program.log_error($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name}): {ex.Message}");
                return false;
            }
            return true;
        }
    }

}
