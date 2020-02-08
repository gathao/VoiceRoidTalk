using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace VoiceRoidTalk.VoiceRecognition.ChromeSpeech
{
    public class SeleniumManager
    {
        private IWebDriver driver = null;

        SeleniumManager()
        {
            CreateChromeDriver();
        }

        public void CreateChromeDriver()
        {
            this.driver = new ChromeDriver();
        }

        public void GotoUrl(string url)
        {
            this.driver.Navigate().GoToUrl(url);
        }


    }
}
