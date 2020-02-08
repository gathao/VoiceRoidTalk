using System;
using System.Collections.Generic;
using System.Text;
using NMeCab;

namespace VoiceroidCharBot
{
    public class RuleTypeCharBot
    {
        string qaListFileName = "QAList.txt";

        MeCabTagger tagger = null;
        QADatabase qaDatabase = null;

        public string StartupPath
        {
            get
            {
                return System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(Environment.GetCommandLineArgs()[0])) + "//";
            }
        }

        public RuleTypeCharBot()
        {
            this.tagger = MeCabTagger.Create();

            string filePath = this.StartupPath + this.qaListFileName;
            this.qaDatabase = new QADatabase(filePath, this.tagger);
        }

        public void CreateQAList(Dictionary<string, string> qaDictionary)
        {
            string filePath = this.StartupPath + this.qaListFileName;
            qaDatabase.CreateNewDbFile(filePath);
            qaDatabase.AddQAList(filePath, this.tagger, qaDictionary);
        }

        public void AddQAList(Dictionary<string, string> qaDictionary)
        {
            string filePath = this.StartupPath + this.qaListFileName;
            qaDatabase.AddQAList(filePath, this.tagger, qaDictionary);
        }

        public string Talk(string text)
        {
            string answer = qaDatabase.NearestAnswer(qaDatabase.ToArray(tagger.ParseToNode(text)));
            return answer;
        }
    }
}
