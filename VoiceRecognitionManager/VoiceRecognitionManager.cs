using System;

namespace VoiceRecognition
{
    public abstract class VoiceRecognitionManager
    {
        public VoiceRecognitionManager()
        {

        }

        public abstract bool IsCanRecognition();

        public abstract bool IsRecognizing();

        public abstract void Create(string name);

        public abstract void Dispose();
    }
}
