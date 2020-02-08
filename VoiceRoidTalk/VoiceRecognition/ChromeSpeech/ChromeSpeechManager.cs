using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceRoidTalk.VoiceRecognition.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Events;
using System.Reactive.Linq;

namespace VoiceRoidTalk.VoiceRecognition.ChromeSpeech
{   
    public class ChromeSpeechManager : VoiceRecognitionManager
    {
        private IWebDriver driver = null;

        private int windowWidth = 450;
        private int windowHeight = 120;

        private bool _isRecognizing = false;
        private Task ChromeMonitoringTask = null;

        public string StartupPath
        {
            get
            {
                return System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(Environment.GetCommandLineArgs()[0]));
            }
        }

        public ChromeSpeechManager()
            : base("ChromeSpeechManager")
        {
            this._isRecognizing = false;
        }


        public override void Create()
        {
            if (this.IsRecognizing())
            {
                return;
            }

            String page = string.Format(@"{0}\GoogleSpeechApiResources\Browser.html", StartupPath);

            ChromeOptions options = new ChromeOptions();
            options.AddArguments(new string[] {
                "--app=" + page,
                "--window-size=" + windowWidth + "," + windowHeight,
                "--use-fake-ui-for-media-stream",
            });

            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            this.driver = new ChromeDriver(driverService, options);

            this.RecognizeStart();
        }

        public void RegisterVoiceRecognitionEvent() {

            this._isRecognizing = true;

            this.ChromeMonitoringTask = Task.Run(() =>
            {
                Console.WriteLine(this.speechManagerName + "モニタリング開始");

                IWebElement recondingResultElement = null;
                string recondingResult = "";

                while (this._isRecognizing)
                {
                    try
                    {
                        Thread.Sleep(250);

                        recondingResultElement = this.driver.FindElement(By.Id("RecondingResult"));
                        recondingResult = recondingResultElement.Text;

                        ResultText = recondingResult;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Dispose();
                    }
                }

                Console.WriteLine(this.speechManagerName + "モニタリング終了");
            });
        }

        /// <summary>
        /// 音声認識開始
        /// </summary>
        public override void RecognizeStart()
        {
            if (this.IsRecognizing())
            {
                return;
            }

            this.RegisterVoiceRecognitionEvent();
        }

        public void RecognizeStop()
        {
            this._isRecognizing = false;
            this.ChromeMonitoringTask.Wait();
        }

        public override bool IsCanRecognition()
        {
            return this.IsRecognizing();
        }

        public override bool IsRecognizing()
        {
            return this._isRecognizing;
        }

        public override void Dispose()
        {
            this._isRecognizing = false;

            if (this.ChromeMonitoringTask != null)
            {
                this.ChromeMonitoringTask.Wait();
                this.ChromeMonitoringTask = null;
            }

            if (this.driver != null)
            {
                this.driver.Close();
                this.driver.Quit();
            }
        }
    }
}
