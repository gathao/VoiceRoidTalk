using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VoiceRoidTalk.VoiceRecognition
{
    public class ResultText
    {
        private ReaderWriterLock rwLock = new ReaderWriterLock();

        private string text = "";

        public string SafeGetText()
        {
            try
            {
                rwLock.AcquireReaderLock(Timeout.Infinite);
                return text;
            }
            finally
            {
                rwLock.ReleaseReaderLock();
            }
        }

        public void SafeSetText(string text)
        {
            try
            {
                rwLock.AcquireWriterLock(Timeout.Infinite);
                this.text = text;
            }
            finally
            {
                rwLock.ReleaseWriterLock();
            }
        }
    }
}
