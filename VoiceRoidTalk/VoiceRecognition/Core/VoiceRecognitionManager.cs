using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceRoidTalk.VoiceRecognition.Core
{
    public delegate void RecognitionComplete();

    public abstract class VoiceRecognitionManager
    {
        protected string speechManagerName;

        private ResultText _resultText = null;

        internal string ResultText
        {
            get
            {
                return this._resultText.SafeGetText();
            }
            set
            {
                this._resultText.SafeSetText(value);
            }
        }


        protected VoiceRecognitionManager(string name)
        {
            speechManagerName = name;
            this._resultText = new ResultText();
        }

        public abstract void RecognizeStart();

        public abstract bool IsCanRecognition();

        public abstract bool IsRecognizing();

        public abstract void Create();

        public abstract void Dispose();
    }
}
