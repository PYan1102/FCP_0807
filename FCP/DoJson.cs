using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Diagnostics;

namespace FCP
{
    class DoJson
    {
        //string Path = Environment.CurrentDirectory;
        JsonData Data;
        string Path = Environment.CurrentDirectory;

        public void JudgeJson(string Date)
        {
            var v = JObject.Parse(Get);
            if (v["門診"] == null)
            {
                NewJson(Date);
                v = JObject.Parse(Get);
            }
            Data = null;
            Data = new JsonData() { 門診 = $"{v["門診"]}", 養護 = $"{v["養護"]}", 大寮 = $"{v["大寮"]}", 住院 = $"{v["住院"]}" };
            string[] DataSplit = Data.門診.Split('^');
            foreach (string s in DataSplit)
            {
                if (s.Trim() == "")
                    continue;
                if (s.Contains(Date))
                    break;
                if (DataSplit.ToList().IndexOf(s) == DataSplit.Length - 2)  //遇空白continue，所以長度多減1
                {
                    CreateDate(Date, Data);
                    break;
                }
            }
        }

        public void NewJson(string Date)
        {
            var json = new
            {
                門診 = $"{Date}|0^",
                養護 = $"{Date}|0^",
                大寮 = $"{Date}|0^",
                住院 = $"{Date}|0^"
            };
            Save(JObject.FromObject(json).ToString());
        }

        public void CreateDate(string Date,JsonData Data)
        {
            Debug.WriteLine("Create Date");
            Data.門診 += $"{Date}|0^";
            Data.養護 += $"{Date}|0^";
            Data.大寮 += $"{Date}|0^";
            Data.住院 += $"{Date}|0^";
            var json = new
            {
                門診 = Data.門診,
                養護 = Data.養護,
                大寮 = Data.大寮,
                住院 = Data.住院
            };
            Save(JObject.FromObject(json).ToString());
        }

        public int GetIndex(int MethodID, string Date)
        {
            int Index = 1;
            string Location = MethodID == 0 ? Data.門診 : MethodID == 1 ? Data.養護 : MethodID == 2 ? Data.大寮 : Data.住院;
            string[] DateL = Location.Split('^');
            for (int x = DateL.Length - 1; x >= 0; x--)
            {
                if (DateL[x].Trim() == "")
                    continue;
                if (DateL[x].Contains(Date))
                {
                    Index = DateL.ToList().IndexOf(DateL[x]);
                    break;
                }
                //if (x == 0)
                //{
                //    Debug.WriteLine($"GetIndex() 的Date不包含在內 {Date}");
                //    return -1;
                //}
            }
            switch(MethodID)
            {
                case 0:  //門診
                    return int.Parse(Data.門診.Split('^')[Index].Split('|')[1]);
                case 1:  //養護
                    return int.Parse(Data.養護.Split('^')[Index].Split('|')[1]);
                case 2:  //大寮
                    return int.Parse(Data.大寮.Split('^')[Index].Split('|')[1]);
                case 3:  //住院
                    return int.Parse(Data.住院.Split('^')[Index].Split('|')[1]);
                default:
                    return -1;
            }
        }

        public void UpdateJson(string Date, int MethodID, int Count)
        {
            if (Count == 0)
                return;
            var v = JObject.Parse(Get);
            Data = null;
            Data = new JsonData() { 門診 = $"{v["門診"]}", 養護 = $"{v["養護"]}", 大寮 = $"{v["大寮"]}", 住院 = $"{v["住院"]}" };
            string Location = MethodID == 0 ? Data.門診 : MethodID == 1 ? Data.養護 : MethodID == 2 ? Data.大寮 : Data.住院;
            string[] DataL = Location.Split('^');
            int Index = 0;
            foreach (string s in DataL)
            {
                if (s.Trim() == "")
                    continue;
                if (s.Contains(Date))
                {
                    Index = DataL.ToList().IndexOf(s);
                    break;
                }
            }
            //if (Index == 0 & DataL.Length > 1)
            //{
            //    Debug.WriteLine("DoJson 裡 UpdateJson 函式的 Index = 0");
            //}
            StringBuilder sb = new StringBuilder();
            string[] A;
            int Num;
            switch (MethodID)
            {
                case 0:
                    A = Data.門診.Split('^');
                    //Num = int.Parse(A[Index].Split('|')[1]) + Count;
                    Num = Count;
                    WriteString(A, Index, Date, Num, sb);
                    Data.門診 = sb.ToString();
                    break;
                case 1:
                    A = Data.養護.Split('^');
                    //Num = int.Parse(A[Index].Split('|')[1]) + Count;
                    Num = Count;
                    WriteString(A, Index, Date, Num, sb);
                    Data.養護 = sb.ToString();
                    break;
                case 2:
                    A = Data.大寮.Split('^');
                    //Num = int.Parse(A[Index].Split('|')[1]) + Count;
                    Num = Count;
                    WriteString(A, Index, Date, Num, sb);
                    Data.大寮 = sb.ToString();
                    break;
                case 3:
                    A = Data.住院.Split('^');
                    //Num = int.Parse(A[Index].Split('|')[1]) + Count;
                    Num = Count;
                    WriteString(A, Index, Date, Num, sb);
                    Data.住院 = sb.ToString();
                    break;
            }
            sb = null;
            A = null;
            Save(JObject.FromObject(Data).ToString());
        }

        private void WriteString(string[] A, int Index, string Date, int Num, StringBuilder sb)
        {
            foreach (string s in A)
            {
                if (A.ToList().IndexOf(s) == Index)
                {
                    sb.Append($"{Date}|{Num}^");
                    continue;
                }
                if (s.Trim() == "")
                    continue;
                sb.Append($"{s}^");
            }
        }

        private string Get
        {
            get
            {
                using (StreamReader sr = new StreamReader($@"{Path}\OLEDB_Index.json", Encoding.Default))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private void Save(string Result)
        {
            using (StreamWriter sw = new StreamWriter($@"{Path}\OLEDB_Index.json", false, Encoding.Default))
            {
                sw.WriteLine(Result);
            }
        }

        public class JsonData
        {
            public string 住院 { get; set; }
            public string 養護 { get; set; }
            public string 門診 { get; set; }
            public string 大寮 { get; set; }
        }
    }
}
