using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 追加
using System.Runtime.Serialization;

namespace SpeechRecognitionAPI
{
    [DataContract]
    class SpeechRecognitionResult
    {
        [DataMember]
        public string version { get; set; }
        [DataMember]
        public Header header { get; set; }
        [DataMember]
        public Result[] results { get; set; }
        [DataContract]
        public class Header
        {
            [DataMember]
            public string status { get; set; }
            [DataMember]
            public string scenario { get; set; }
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public string lexical { get; set; }
            [DataMember]
            public Properties properties { get; set; }
        }
        [DataContract]
        public class Properties
        {
            [DataMember]
            public string requestid { get; set; }
            [DataMember]
            public string HIGHCONF { get; set; }
        }
        [DataContract]
        public class Result
        {
            [DataMember]
            public string scenario { get; set; }
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public string lexical { get; set; }
            [DataMember]
            public string confidence { get; set; }
            [DataMember]
            public Properties2 properties { get; set; }
        }
        [DataContract]
        public class Properties2
        {
            [DataMember]
            public string HIGHCONF { get; set; }
        }
    }

}