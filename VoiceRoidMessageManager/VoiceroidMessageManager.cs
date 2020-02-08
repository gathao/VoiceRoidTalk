using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace VoiceroidManager
{
    public abstract class VoiceroidMessageManager
    {
        /// <summary>
        /// VOICEROIDプロセス
        /// </summary>
        protected Process voiceroideProcess = null;

        private string voiceroidName;

        private Boolean isMonitoring = false;
        private Task monitoringTask = null;

        public VoiceroidMessageManager(string voiceroidName)
        {
            this.voiceroidName = voiceroidName;
        }

        /// <summary>
        /// VOICEROIDのプロセスを取得する
        /// </summary>
        public void Create(string voiceroidPath)
        {
            if (String.IsNullOrEmpty(voiceroidPath))
            {
                Console.WriteLine(this.voiceroidName + " のexeファイルのパスが設定されてませんぞｗｗｗ");
                throw new ArgumentException(this.voiceroidName + " のexeファイルのパスが設定されてませんぞｗｗｗ");
            }

            if (!File.Exists(voiceroidPath))
            {
                Console.WriteLine(this.voiceroidName + " のexeファイルが見つかりませんでしたぞｗパスの再設定以外ありえないｗｗｗ");
                throw new IOException(this.voiceroidName + " のexeファイルのパスが設定されてませんぞｗｗｗ");
            }

            try
            {
                // プロセスを取得する
                AutomationElement voiceroidWindow = this.GetVoiceroidApp(voiceroidPath);

                // 取得できないときはプロセスを起動する
                if (voiceroidWindow is null)
                {
                    voiceroidWindow = this.StartVoiceroidApp(voiceroidPath);
                }
                this.GetComponent(voiceroidWindow);

                this.MonitoringVoiceroid(voiceroidPath);
                
            } 
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(this.voiceroidName + " の認識に失敗しました" + e.Message);
            }

        }

        /// <summary>
        /// VOICEROIDアプリを起動する。
        /// </summary>
        protected AutomationElement StartVoiceroidApp(string voiceroidPath)
        {
            Console.WriteLine(this.voiceroidName + " 起動開始：" + voiceroidPath);

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = voiceroidPath;
            processStartInfo.WorkingDirectory = Path.GetDirectoryName(voiceroidPath);
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;

            this.voiceroideProcess = Process.Start(processStartInfo);

            if (!this.VoiceroidProcessStartWait(15000))
            {
                throw new TimeoutException(this.voiceroidName + " のプロセス取得がタイムアウトしましたぞｗｗｗ");
            }

            return AutomationElement.FromHandle(this.voiceroideProcess.MainWindowHandle);
        }

        /// <summary>
        /// VOICEROIDアプリが動作していたら取得する。
        /// </summary>
        protected AutomationElement GetVoiceroidApp(string voiceroidPath, Boolean isWait = true)
        {
            string path = Path.GetFileNameWithoutExtension(voiceroidPath);
            var processes = Process.GetProcessesByName(path);

            foreach (Process process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(voiceroidPath)))
            {
                if (voiceroidPath == process.MainModule.FileName)
                {
                    if (isWait)
                    {
                        if (!this.VoiceroidProcessStartWait(15000))
                        {
                            throw new TimeoutException(this.voiceroidName + " のプロセス取得がタイムアウトしましたぞｗｗｗ");
                        }
                    }
                    else
                    {
                        if (IntPtr.Zero == process.MainWindowHandle)
                        {
                            return null;
                        }
                    }

                    AutomationElement voiceroidElement = AutomationElement.FromHandle(this.voiceroideProcess.MainWindowHandle);

                    WindowPattern voiceroidWindowPattern = (WindowPattern)voiceroidElement.GetCurrentPattern(WindowPattern.Pattern);
                    voiceroidWindowPattern.SetWindowVisualState(WindowVisualState.Normal);

                    return voiceroidElement;
                }
            }

            return null;
        }

        /// <summary>
        /// VOICEROIDアプリのプロセスが取得できるまで待機する
        /// </summary>
        protected bool VoiceroidProcessStartWait(long timeOutMillisec)
        {
            // ウィンドウハンドルが取得できるまで待機（15秒でタイムアウト）
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (IntPtr.Zero == this.voiceroideProcess.MainWindowHandle)
            {
                Thread.Sleep(250);

                if (15000 <= stopwatch.ElapsedMilliseconds)
                {
                    // タイムアウト
                    Console.WriteLine(this.voiceroidName + " のプロセス取得がタイムアウトしましたぞｗｗｗ");
                    stopwatch.Stop();
                    return false;
                }
            }

            stopwatch.Stop();

            Console.WriteLine(this.voiceroidName + " のプロセスが取得できましたなｗｗｗ");

            return true;
        }

        /// <summary>
        /// VOICEROIDをモニタリングする。
        /// </summary>
        private void MonitoringVoiceroid(string voiceroidPath)
        {
            this.isMonitoring = true;

            this.monitoringTask = Task.Run(() =>
            {
                Console.WriteLine(this.voiceroidName + "モニタリング開始");

                while(this.isMonitoring)
                {
                    try
                    {
                        Thread.Sleep(250);

                        if (this.IsCanSpeech())
                        {
                            continue;
                        }

                        AutomationElement voiceroidWindow = this.GetVoiceroidApp(voiceroidPath, false);
                        if (null == voiceroidWindow)
                        {
                            continue;
                        }

                        this.GetComponent(voiceroidWindow);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                        if (null != this.voiceroideProcess)
                        {
                            this.voiceroideProcess.Dispose();
                            this.voiceroideProcess = null;
                        }
                    }
                }

                Console.WriteLine(this.voiceroidName + "モニタリング終了");
            });
        }

        /// <summary>
        /// 破棄する
        /// </summary>
        public virtual void Dispose(Boolean autoExit)
        {
            if (null != this.monitoringTask)
            {
                this.isMonitoring = false;
                this.monitoringTask.Wait();
                this.monitoringTask = null;
            }

            if (null != this.voiceroideProcess)
            {
                try
                {
                    if (autoExit)
                    {
                        this.voiceroideProcess.Kill();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                this.voiceroideProcess.Dispose();
                this.voiceroideProcess = null;
            }

            Console.WriteLine(this.voiceroidName + " 破棄完了");
        }

        /// <summary>
        /// 読み上げ開始
        /// </summary>
        /// <param name=""></param>
        public void Speach(String text)
        {
            try
            {
                if (!this.IsCanSpeech())
                {
                    Console.WriteLine( this.voiceroidName + ": speach skip");
                    return;
                }

                Console.WriteLine(this.voiceroidName + " speach start: " + text);
                this.SpeechControl(text);
                Console.WriteLine(this.voiceroidName + " speach end");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                if (null != this.voiceroideProcess)
                {
                    this.voiceroideProcess.Dispose();
                    this.voiceroideProcess = null;
                }
            }
        }

        /// <summary>
        /// 制御に必要なコンポーネントを取得する
        /// </summary>
        protected abstract void GetComponent(AutomationElement form);

        /// <summary>
        /// 読み上げ制御を行う
        /// </summary>
        protected abstract void SpeechControl(string text);

        /// <summary>
        /// 読み上げ可能か否かを返す
        /// </summary>
        protected abstract Boolean IsCanSpeech();
    }
}
