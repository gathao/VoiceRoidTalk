using System;
using System.Collections.Generic;
using System.Text;
using System.Speech;
using System.Speech.Recognition;

namespace VoiceRecognition.SystemSpeech
{
    public class SystemSpeechManager : VoiceRecognitionManager
    {
        public SystemSpeechManager()
        {

        }




        public override bool IsCanRecognition()
        {
            return true;
        }

        public override bool IsRecognizing()
        {
            return false;
        }

        public override void Create(string name)
        {

        }

        public override void Dispose()
        {

        }
    }
}
