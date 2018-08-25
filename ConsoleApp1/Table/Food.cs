using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace NTable
{
    public class Food
    {
        public static Food Instance { get { return instance; } }

        private static Food instance = new Food();

        public static List<string> Flyweight = new List<string>();

        public static string Keyword = "뭐먹지";

        public void AddObject(string key)
        {
            Flyweight.Add(key);
        }

        public string GetObject(string key)
        {
            if (!Flyweight.Contains(key)) return null;
            else return key;
        }

        public int GetSize()
        {
            int result = 0;
            foreach ( var item in Flyweight )
            {
                result += System.Text.ASCIIEncoding.Unicode.GetByteCount(item);
            }
            return result;
        }

        public string GetRandomObject()
        {
            string reply = string.Empty;
            
            DateTime dateTime = DateTime.Now;

            Random rnd = new Random(dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Year);
            int time = rnd.Next(1, Flyweight.Count);

            int index = 1;
            foreach (var item in Flyweight)
            {
                if (index == time || 1 == Flyweight.Count)
                {
                    reply = item;
                    break;
                }
                index++;
            }

            return reply;
        }

        public bool Execute(string fileName, bool makeClear = false)
        {
            if (false == makeClear)
            {
                Food.Flyweight.Clear();
            }

            try
            {
                string[] lines = System.IO.File.ReadAllLines(fileName);

                foreach (string line in lines)
                {
                    AddObject(line);
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