using Microsoft.Extensions.Options;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTRClient.MainModule
{
    public partial class AutoPro
    {

        public async Task SetUp()
        {
            // await new BrowserFetcher().DownloadAsync();
            // m_clsBrowser = await Puppeteer.LaunchAsync(new LaunchOptions
            // {
            //     Headless = false
            // });
        }

        public async Task TearDown()
        {
            // await m_clsBrowser.CloseAsync();
        }
        public AutoPro()
        {
            // m_varBroOpts = new LaunchOptions
            // {
            //     Headless = false,
            //     ExecutablePath = ""
            // };
        }

        public async Task<bool> workflowWebAuto()
        {
            try
            {
                // var browserFetcher = new BrowserFetcher();
                // await browserFetcher.DownloadAsync();
                // using (var browser = await Puppeteer.LaunchAsync(m_varBroOpts))
                // using (var page = await browser.NewPageAsync())
                // {
                //     await page.GoToAsync("https://ip.me/");
                // 
                //     Console.WriteLine("Generating PDF");
                //     await page.PdfAsync(Path.Combine(Directory.GetCurrentDirectory(), "google.pdf"));
                // 
                //     Console.WriteLine("Export completed");
                // 
                //     // if (!args.Any(arg => arg == "auto-exit"))
                //     // {
                //     //     Console.ReadLine();
                //     // }
                // }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
