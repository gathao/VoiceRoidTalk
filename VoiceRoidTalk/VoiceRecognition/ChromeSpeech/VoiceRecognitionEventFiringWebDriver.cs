using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Events;

namespace VoiceRoidTalk.VoiceRecognition.ChromeSpeech
{
    public class VoiceRecognitionEventFiringWebDriver : EventFiringWebDriver
    {
        public VoiceRecognitionEventFiringWebDriver(IWebDriver driver) : base(driver)
        {
        }

        protected override void OnElementValueChanged(WebElementValueEventArgs e)
        {
            base.OnElementValueChanged(e);
        }
    }
}
