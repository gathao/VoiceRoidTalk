using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using CefSharp;
using CefSharp.Wpf;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;
using VoiceroidManager;
using VoiceRoidTalk.VoiceRecognition.Core;
using VoiceRoidTalk.VoiceRecognition.ChromeSpeech;
using VoiceroidCharBot;
using System.Threading;
using System.Diagnostics;

namespace VoiceRoidTalk
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        VoiceroidMessageManager kiritanManager = new KiritanMessageManager();
        VoiceRecognitionManager voiceRecognitionManager = new ChromeSpeechManager();
        RuleTypeCharBot chartBot = null;

        private Task resultTextMonitoringTask = null;



        public string StartupPath
        {
            get
            {
                return System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(Environment.GetCommandLineArgs()[0]));
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            ChatBotInitialize();
        }

        private void ChatBotInitialize()
        {
            chartBot = new RuleTypeCharBot();
        }

        private void VoiceroidExecute()
        {
            kiritanManager.Create("C:\\Program Files (x86)\\AHS\\VOICEROID+\\KiritanEX\\VOICEROID.exe");
        }

        private void VoiceRecognigExecute()
        {
            if (this.voiceRecognitionManager.IsCanRecognition())
            {
                return;
            }

            this.voiceRecognitionManager.Create();

            this.resultTextMonitoringTask = Task.Run(() =>
            {
                string recondingResult = "";
                string prevRecondingResult = "";

                while (true)
                {
                    if (this.voiceRecognitionManager.IsCanRecognition())
                    {
                        try
                        {
                            Thread.Sleep(250);

                            recondingResult = this.voiceRecognitionManager.ResultText;

                            if (prevRecondingResult.CompareTo(recondingResult) != 0)
                            {
                                prevRecondingResult = recondingResult;

                                string answer = chartBot.Talk(prevRecondingResult);

                                kiritanManager.Speach(answer);

                                ProcessStart(answer);
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            });
        }

        class ProcessInfo
        {
            public string exePath;
            public Process process;
        }

        private readonly Dictionary<string, ProcessInfo> processDictionary = new Dictionary<string, ProcessInfo>()
        {
            {
                "サクラエディタを起動するよ",
                new ProcessInfo()
                {
                    exePath = @"C:\Program Files (x86)\sakura\sakura.exe",
                    process = null,
                }
            },
        };

        public void ProcessStart(string keyword)
        {
            ProcessInfo info = processDictionary[keyword];

            if (info == null)
            {
                return;
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = info.exePath;
            processStartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(info.exePath);
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;

            info.process = Process.Start(processStartInfo);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //this.TalkEditor.Text = chartBot.Talk("名前は？");
            VoiceroidExecute();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string talkString = this.TalkEditor.Text;
            kiritanManager.Speach(talkString);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            VoiceRecognigExecute();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            kiritanManager.Dispose(true);
            voiceRecognitionManager.Dispose();

            if (this.resultTextMonitoringTask != null)
            {
                this.resultTextMonitoringTask.Wait();
            }
        }
    }

    public class Echo : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Sessions.Broadcast(e.Data);
        }
    }
}
