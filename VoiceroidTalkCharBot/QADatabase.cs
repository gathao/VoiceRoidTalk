using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NMeCab;
using NMeCab.Core;

namespace VoiceroidCharBot
{
    public class QADatabase
    {
        List<string[]> questionList;
        List<string> answerList;

        public QADatabase(string dbFilePath, MeCabTagger tagger)
        {
            questionList = new List<string[]>();
            answerList = new List<string>();

            if (!File.Exists(dbFilePath))
            {
                this.CreateNewDbFile(dbFilePath);
            }

            this.LoadQAList(dbFilePath, tagger);
        }

        public void CreateNewDbFile(string dbFilePath)
        {
            using (StreamWriter writer = new StreamWriter(dbFilePath, false, Encoding.GetEncoding("UTF-8")))
            {
                writer.Write("");
                writer.Close();
            }
        }

        public void LoadQAList(string dbFilePath, MeCabTagger tagger)
        {
            using (StreamReader qaListFile = new StreamReader(dbFilePath, Encoding.GetEncoding("UTF-8")))
            {
                while (qaListFile.Peek() != -1)
                {
                    string[] qaText = qaListFile.ReadLine().Split(':');

                    questionList.Add(ToArray(tagger.ParseToNode(qaText[0])));
                    answerList.Add(qaText[1]);
                }

                qaListFile.Close();
            }
        }

        public void AddQAList(string dbFilePath, MeCabTagger tagger, Dictionary<string, string> qaDictionary)
        {
            Dictionary<string, string> saveDirectory = new Dictionary<string, string>();

            // 現在のディレクトリをロード
            using (StreamReader qaListFile = new StreamReader(dbFilePath, Encoding.GetEncoding("UTF-8")))
            {
                while (qaListFile.Peek() != -1)
                {
                    string[] qaText = qaListFile.ReadLine().Split(':');
                    saveDirectory[qaText[0]] = qaText[1];
                }

                qaListFile.Close();
            }

            // 新しいペアを追加・更新
            foreach (KeyValuePair<string, string> qaPair in qaDictionary)
            {
                saveDirectory[qaPair.Key] = qaPair.Value;
            }

            // 保存
            using (StreamWriter writer = new StreamWriter(dbFilePath, false, Encoding.GetEncoding("UTF-8")))
            {
                foreach (KeyValuePair<string, string> qaPair in saveDirectory)
                {
                    writer.WriteLine($"{qaPair.Key}:{qaPair.Value}");
                }
   
                writer.Close();
            }

            // 質問と回答のリロード
            this.LoadQAList(dbFilePath, tagger);
        }

        private double Similarity(string[] x, string[] y)
        {
            HashSet<string> words = new HashSet<string>();

            foreach (string str in x)
            {
                words.Add(str);
            }

            int count = 0;

            foreach(string str in y)
            {
                if (words.Contains(str))
                {
                    count++;
                }
            }

            return (double)count / (Math.Sqrt((double)x.Length * y.Length));
        }

        public string NearestAnswer(string[] question)
        {
            double maxSim = -1.0;
            int maxIndex = -1;

            for (int i=0; i < questionList.Count; i++)
            {
                double sim = Similarity(questionList[i], question);

                if (sim > maxSim)
                {
                    maxSim = sim;
                    maxIndex = i;
                }
            }

            string result = "こんにちは";

            if (0 <= maxIndex)
            {
                result = answerList[maxIndex];
            }
            else
            {
                result = RandomAnswer();
            }

            return result;
        }

        public string RandomAnswer()
        {
            return "こんにちは";
        }

        public string[] ToArray(MeCabNode node)
        {
            List<string> words = new List<string>();

            while (node != null)
            {
                if (node.Stat != MeCabNodeStat.Bos && node.Stat != MeCabNodeStat.Eos)
                {
                    words.Add(node.Surface);
                }

                node = node.Next;
            }

            return words.ToArray();
        }
    }
}
