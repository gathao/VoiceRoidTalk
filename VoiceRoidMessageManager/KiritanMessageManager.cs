using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;

namespace VoiceroidManager
{
    /// <summary>
    /// VOICEROID+ 東北きりたん 読み上げライブラリ
    /// </summary>
    public class KiritanMessageManager : VoiceroidMessageManager
    {
        /// <summary>
        /// Win32APIのメッセージを送る関数。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="ptr"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, String lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private IntPtr editorWnd;
        private IntPtr speakhWnd;
        private AutomationElement waitComplete;

        public KiritanMessageManager()
            : base("東北きりたん")
        {
            this.ClearMember();
        }

        /// <summary>
        /// 破棄する
        /// </summary>
        public override void Dispose(bool autoExit)
        {
            base.Dispose(autoExit);
            this.ClearMember();
        }

        private void ClearMember()
        {
            this.editorWnd = IntPtr.Zero;
            this.speakhWnd = IntPtr.Zero;
            this.waitComplete = null;
        }

        /// <summary>
        /// 制御に必要なコンポーネントを取得する
        /// </summary>
        protected override void GetComponent(AutomationElement mainWindow)
        {
            try
            {
                AutomationElement txtMain = mainWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "txtMain"));
                this.editorWnd = new IntPtr(txtMain.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.IsTextPatternAvailableProperty, true)).Current.NativeWindowHandle);
                this.speakhWnd = new IntPtr(mainWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "btnPlay")).Current.NativeWindowHandle);
                this.waitComplete = mainWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "btnSaveWave"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                this.ClearMember();
                throw e;
            }
        }

        /// <summary>
        /// 読み上げ制御を行う
        /// </summary>
        protected override void SpeechControl(string text)
        {
            try
            {
                IntPtr result = IntPtr.Zero;

                // テキストボックスにテキストを送信
                result = SendMessage(this.editorWnd, 0x0c, IntPtr.Zero, text);
                Console.WriteLine("Textbox sendmessage result= :" + result);

                // 再生ボタンクリック
                result = SendMessage(this.speakhWnd, 0xF5, IntPtr.Zero, IntPtr.Zero);
                Console.WriteLine("Textbox sendmessage result= :" + result);

                Thread.Sleep(250);

                // 再生終了を待機する
                while (!this.waitComplete.Current.IsEnabled)
                {
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("sendmessage failed");

                this.ClearMember();

                throw e;
            }
        }

        /// <summary>
        /// 読み上げ可能か否かを返す
        /// </summary>
        protected override Boolean IsCanSpeech()
        {
            if (null == this.waitComplete)
            {
                return false;
            }

            try
            {
                Boolean result = this.waitComplete.Current.IsEnabled;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("voiceroid process is missing");

                this.ClearMember();

                throw e;
            }
        }
    }
}

