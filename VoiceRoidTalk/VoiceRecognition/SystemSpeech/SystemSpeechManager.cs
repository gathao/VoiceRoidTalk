using System;
using System.Collections.Generic;
using System.Text;
using System.Speech;
using System.Speech.Recognition;
using VoiceRoidTalk.VoiceRecognition.Core;

namespace VoiceRecognition.SystemSpeech
{
    public class SystemSpeechManager : VoiceRecognitionManager
    {
        private SpeechRecognitionEngine engine;

        /// <summary>
        /// 音声認識エンジンが破棄されているかどうかを取得、設定します。
        /// </summary>
        private bool IsDestroyed;

        /// <summary>
        /// 一時的に音声の一部を認識した場合のイベントを設定、取得します。
        /// </summary>
        public System.Action<SpeechHypothesizedEventArgs> SpeechHypothesizedEvent;

        /// <summary>
        /// 信頼性の高い 1 つ以上の句を認識した場合のイベントを設定、取得します。
        /// </summary>
        public System.Action<SpeechRecognizedEventArgs> SpeechRecognizedEvent;

        /// <summary>
        /// 信頼性の低い候補句のみ認識した場合のイベントを設定、取得します。
        /// </summary>
        public System.Action<SpeechRecognitionRejectedEventArgs> SpeechRecognitionRejectedEvent;

        /// <summary>
        /// 音声認識が終了した場合のイベントを設定、取得します。
        /// </summary>
        public System.Action<RecognizeCompletedEventArgs> SpeechRecognizeCompletedEvent;

        public SystemSpeechManager(string name)
            : base(name)
        {
            this.IsDestroyed = true;
        }

        public override void Create()
        {
            if (this.IsRecognizing())
            {
                return;
            }

            this.engine = new SpeechRecognitionEngine("SR_MS_ja-JP_TELE_11.0");

            IsDestroyed = false;

            this.engine.SetInputToDefaultAudioDevice();

            this.engine.SpeechHypothesized += SpeechHypothesized;
            this.engine.SpeechRecognized += SpeechRecognized;
            this.engine.SpeechRecognitionRejected += SpeechRecognitionRejected;
            this.engine.RecognizeCompleted += SpeechRecognizeCompleted;
        }

        /// <summary>
        /// ルール型による音声認識方法を追加します。
        /// </summary>
        /// <param name="grammarName">文法名</param>
        /// <param name="words">追加する語彙</param>
        public void AddGrammar(string grammarName, params string[] words)
        {
            Choices choices = new Choices();
            choices.Add(words);

            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append(choices);

            AddGrammar(grammarName, grammarBuilder);
        }

        /// <summary>
        /// ルール型による音声認識方法を追加します。
        /// </summary>
        /// <param name="grammarName">文法名</param>
        /// <param name="grammarBuilder">音声認識の文法</param>
        public void AddGrammar(string grammarName, GrammarBuilder grammarBuilder)
        {
            Grammar grammar = new Grammar(grammarBuilder)
            {
                Name = grammarName
            };

            AddGrammar(grammar);
        }

        /// <summary>
        /// ルール型による音声認識方法を追加します。
        /// </summary>
        /// <param name="grammar">音声認識の文法</param>
        public void AddGrammar(Grammar grammar)
        {
            if (!this.IsRecognizing())
            {
                return;
            }

            this.engine.LoadGrammar(grammar);
        }

        /// <summary>
        /// 自由発話のディクテーション型による音声認識方法を追加します。Grammar.Name は Dictation です。
        /// System.Speech.dll のみ使用可能です。
        /// </summary>
        public void AddDictation()
        {
            DictationGrammar dictation = new DictationGrammar()
            {
                Name = "Dictation"
            };

            AddGrammar(dictation);
        }

        /// <summary>
        /// 登録されている音声認識方法を削除します。
        /// </summary>
        /// <param name="grammarName">文法名</param>
        public void ClearGrammar(string grammarName)
        {
            if (!this.IsRecognizing())
            {
                return;
            }

            foreach (Grammar g in this.engine.Grammars)
            {
                if (g.Name == grammarName)
                {
                    this.engine.UnloadGrammar(g);
                    break;
                }
            }
        }

        /// <summary>
        /// 登録されているすべての音声認識方法を削除します。
        /// </summary>
        public void ClearGrammar()
        {
            if (!this.IsRecognizing())
            {
                return;
            }

            this.engine.UnloadAllGrammars();
        }

        /// <summary>
        /// 非同期で音声認識を開始します。
        /// </summary>
        /// <param name="multiple">常に音声を認識する場合は true</param>
        public override void RecognizeStart()
        {
            if (this.IsRecognizing() || this.engine.Grammars.Count <= 0)
            {
                return;
            }

            bool multiple = false;
            RecognizeMode mode = (multiple) ? RecognizeMode.Multiple : RecognizeMode.Single;
            this.engine.RecognizeAsync(mode);
        }

        /// <summary>
        /// 現在の音声認識操作の完了を待たずに非同期認識を終了します。
        /// </summary>
        public void RecognizeAsyncCancel()
        {
            if (!this.IsRecognizing())
            {
                return;
            }

            this.engine.RecognizeAsyncCancel();
        }

        /// <summary>
        /// 現在の音声認識操作の完了後に非同期認識を終了します。
        /// </summary>
        public void RecognizeAsyncStop()
        {
            if (!this.IsRecognizing())
            {
                return;
            }

            this.engine.RecognizeAsyncStop();
        }


        public override bool IsCanRecognition()
        {
            return true;
        }

        public override bool IsRecognizing()
        {
            return false;
        }

        // 一時的に音声の一部を認識した場合のイベント
        private void SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            if (e.Result != null && SpeechHypothesizedEvent != null)
            {
                SpeechHypothesizedEvent(e);
            }
        }

        // 信頼性の高い 1 つ以上の句を認識した場合のイベント
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null && SpeechRecognizedEvent != null)
            {
                SpeechRecognizedEvent(e);
            }
        }

        // 信頼性の低い候補句のみ認識した場合のイベント
        private void SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (e.Result != null && SpeechRecognitionRejectedEvent != null)
            {
                SpeechRecognitionRejectedEvent(e);
            }
        }

        // 音声認識が終了した場合のイベント
        private void SpeechRecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            if (e.Result != null && SpeechRecognizeCompletedEvent != null)
            {
                SpeechRecognizeCompletedEvent(e);
            }
        }

        public override void Dispose()
        {
            if (!this.IsRecognizing())
            {
                return;
            }

            //this.engine.SpeechHypothesized -= SpeechHypothesized;
            //this.engine.SpeechRecognized -= SpeechRecognized;
            //this.engine.SpeechRecognitionRejected -= SpeechRecognitionRejected;
            //this.engine.RecognizeCompleted -= SpeechRecognizeCompleted;
            this.engine.UnloadAllGrammars();
            this.engine.Dispose();
        }
    }
}
