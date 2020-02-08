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
using System.Reactive.Linq;
using System.Threading;

namespace VoiceRoidTalk
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        VoiceroidMessageManager kiritanManager = new KiritanMessageManager();
        VoiceRecognitionManager voiceRecognitionManager = new ChromeSpeechManager();

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

            VoiceroidExecute();

            VoiceRecognigExecute();
        }

        private void VoiceroidExecute()
        {
            kiritanManager.Create("C:\\Program Files (x86)\\AHS\\VOICEROID+\\KiritanEX\\VOICEROID.exe");
        }

        private void VoiceRecognigExecute()
        {
            this.voiceRecognitionManager.Create();

            this.resultTextMonitoringTask = Task.Run(() =>
            {
                string recondingResult = "";
                string prevRecondingResult = "";

                while (true)
                {
                    try
                    {
                        Thread.Sleep(250);

                        recondingResult = this.voiceRecognitionManager.ResultText;

                        if (prevRecondingResult.CompareTo(recondingResult) != 0)
                        {
                            prevRecondingResult = recondingResult;
                            kiritanManager.Speach(recondingResult);
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                }
            });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            VoiceroidExecute();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string talkString = this.TalkEditor.Text;

            talkString = this.voiceRecognitionManager.ResultText;

            kiritanManager.Speach(talkString);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            voiceRecognitionManager.RecognizeStart();
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
