using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace NTable
{
    public class EnglishWord : Flyweight
    {
        public static EnglishWord Instance { get { return instance; } }
        private static EnglishWord instance = new EnglishWord();
        public List<string> Container = new List<string>();

        public override string GetFlyweightType()
        {
            return "!영어단어";
        }

        public override string GetFilePath()
        {
            return Directory.GetCurrentDirectory() + "\\Table\\EnglishWord.csv";
        }

        public override int GetSize()
        {
            int result = 0;
            foreach (var item in Container)
            {
                result += System.Text.ASCIIEncoding.Unicode.GetByteCount(item);
            }
            return result;
        }

        public override int GetCount()
        {
            return Container.Count;
        }

        public override string GetItem()
        {
            string reply = string.Empty;

            DateTime dateTime = DateTime.Now;

            Random rnd = new Random(dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Year);
            int time = rnd.Next(1, Container.Count);

            int index = 1;
            foreach (var item in Container)
            {
                if (index == time || 1 == Container.Count)
                {
                    reply = item;
                    break;
                }
                index++;
            }

            return reply;
        }

        public override bool Execute(string fileName, bool makeClear = false)
        {
            if (false == makeClear)
            {
                Container.Clear();
            }

            try
            {
                string[] lines = System.IO.File.ReadAllLines(fileName);

                foreach (string line in lines)
                {
                    Container.Add(line);
                }

                Console.WriteLine(fileName + ": 테이블 로드 완료.");
            }
            catch (IOException)
            {
                Console.WriteLine(fileName + ": 파일이 존재하지 않습니다.");
                return false;
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(fileName + ": 해당 테이블에서 형식이 잘못 입력된 줄이 발견되었습니다.");
                return false;
            }
            catch (ArgumentException)
            {
                Console.WriteLine(fileName + ": 해당 테이블에서 중복된 키가 있는 것 같습니다.");
                return false;
            }
            return true;
        }
    }
}