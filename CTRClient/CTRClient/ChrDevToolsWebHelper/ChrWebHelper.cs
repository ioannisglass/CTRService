using MasterDevs.ChromeDevTools;
// using MasterDevs.ChromeDevTools.Protocol.Chrome.Emulation;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Page;
using MasterDevs.ChromeDevTools.Protocol.Chrome.DOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;

namespace CTRClient.ChrDevToolsWebHelper
{
    public class ChrWebHelper
    {
        public ChrWebHelper()
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
                    Program.log_error($"chrome.exe Not found. Perhaps the Google Chrome browser is not installed on this computer.");
                    return false;
                }

                var screenshotDone = new ManualResetEventSlim();

                // STEP 1 - Run Chrome
                var chromeProcessFactory = new ChromeProcessFactory(new StubbornDirectoryCleaner(), w_strChromePath);
                using (var chromeProcess = chromeProcessFactory.Create(9222, false))
                {
                    // STEP 2 - Create a debugging session
                    var sessionInfo = (await chromeProcess.GetSessionInfo()).LastOrDefault();
                    var chromeSessionFactory = new ChromeSessionFactory();
                    var chromeSession = chromeSessionFactory.Create(sessionInfo.WebSocketDebuggerUrl);

                    // STEP 3 - Send a command
                    //
                    // Here we are sending a commands to tell chrome to set the viewport size 
                    // and navigate to the specified URL
                    await chromeSession.SendAsync(new SetDeviceMetricsOverrideCommand
                    {
                        Width = 1440,
                        Height = 900,
                        Scale = 1
                    });

                    var navigateResponse = await chromeSession.SendAsync(new NavigateCommand
                    {
                        Url = "http://www.google.com"
                    });
                    Program.log_info("NavigateResponse: " + navigateResponse.Id);

                    // STEP 4 - Register for events (in this case, "Page" domain events)
                    // send an command to tell chrome to send us all Page events
                    // but we only subscribe to certain events in this session
                    var pageEnableResult = await chromeSession.SendAsync<MasterDevs.ChromeDevTools.Protocol.Chrome.Page.EnableCommand>();
                    Console.WriteLine("PageEnable: " + pageEnableResult.Id);

                    chromeSession.Subscribe<LoadEventFiredEvent>(loadEventFired =>
                    {
                        // we cannot block in event handler, hence the task
                        Task.Run(async () =>
                        {
                            Console.WriteLine("LoadEventFiredEvent: " + loadEventFired.Timestamp);

                            var documentNodeId = (await chromeSession.SendAsync(new GetDocumentCommand())).Result.Root.NodeId;
                            var bodyNodeId =
                                (await chromeSession.SendAsync(new QuerySelectorCommand
                                {
                                    NodeId = documentNodeId,
                                    Selector = "body"
                                })).Result.NodeId;
                            var height = (await chromeSession.SendAsync(new GetBoxModelCommand { NodeId = bodyNodeId })).Result.Model.Height;

                            await chromeSession.SendAsync(new SetDeviceMetricsOverrideCommand
                            {
                                Width = 1440,
                                Height = height,
                                Scale = 1
                            });

                            Console.WriteLine("Taking screenshot");
                            var screenshot = await chromeSession.SendAsync(new CaptureScreenshotCommand { Format = "png" });

                            var data = Convert.FromBase64String(screenshot.Result.Data);
                            File.WriteAllBytes("output.png", data);
                            Console.WriteLine("Screenshot stored");

                            // tell the main thread we are done
                            screenshotDone.Set();
                        });
                    });

                    // wait for screenshoting thread to (start and) finish
                    screenshotDone.Wait();

                    Console.WriteLine("Exiting ..");
                }
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
