using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Diagnostics;
using FCP.src.Enum;

namespace FCP.MVVM.Control
{
    static class JsonService
    {
        private static JsonData _Json { get; set; }
        private static string _CurrentPath = Environment.CurrentDirectory;
        private static string GetContent
        {
            get
            {
                using (StreamReader sr = new StreamReader($@"{_CurrentPath}\OLEDB_Index.json", Encoding.Default))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static void JudgeJsonHasBeenCreated(string date)
        {
            var v = JObject.Parse(GetContent);
            if (v["門診"] == null)
            {
                CreateJson(date);
                v = JObject.Parse(GetContent);
            }
            _Json = null;
            _Json = new JsonData() { 門診 = $"{v["門診"]}", 養護 = $"{v["養護"]}", 大寮 = $"{v["大寮"]}", 住院 = $"{v["住院"]}" };
            string[] list = _Json.門診.Split('^');
            foreach (string s in list)
            {
                if (s.Trim() == "")
                    continue;
                if (s.Contains(date))
                    break;
                if (list.ToList().IndexOf(s) == list.Length - 2)  //遇空白continue，所以長度多減1
                {
                    CreateDate(date, _Json);
                    break;
                }
            }
        }

        public static int GetOLEDBIndex(eConvertLocation department, string date)
        {
            int index = GetCurrentIndexReverse(date, department);
            switch (department)
            {
                case eConvertLocation.OPD:
                    return int.Parse(_Json.門診.Split('^')[index].Split('|')[1]);
                case eConvertLocation.Care:
                    return int.Parse(_Json.養護.Split('^')[index].Split('|')[1]);
                case eConvertLocation.Other:
                    return int.Parse(_Json.大寮.Split('^')[index].Split('|')[1]);
                case eConvertLocation.UDBatch:
                    return int.Parse(_Json.住院.Split('^')[index].Split('|')[1]);
                default:
                    return -1;
            }
        }

        public static void UpdateJson(string date, eConvertLocation department, int count)
        {
            if (count == 0)
                return;
            var v = JObject.Parse(GetContent);
            _Json = null;
            _Json = new JsonData() { 門診 = $"{v["門診"]}", 養護 = $"{v["養護"]}", 大寮 = $"{v["大寮"]}", 住院 = $"{v["住院"]}" };
            int index = GetCurrentIndex(date, department);
            StringBuilder sb = new StringBuilder();
            string[] A;
            int num;
            switch (department)
            {
                case eConvertLocation.OPD:
                    A = _Json.門診.Split('^');
                    num = count;
                    WriteString(A, index, date, num, sb);
                    _Json.門診 = sb.ToString();
                    break;
                case eConvertLocation.Care:
                    A = _Json.養護.Split('^');
                    num = count;
                    WriteString(A, index, date, num, sb);
                    _Json.養護 = sb.ToString();
                    break;
                case eConvertLocation.Other:
                    A = _Json.大寮.Split('^');
                    num = count;
                    WriteString(A, index, date, num, sb);
                    _Json.大寮 = sb.ToString();
                    break;
                case eConvertLocation.UDBatch:
                    A = _Json.住院.Split('^');
                    num = count;
                    WriteString(A, index, date, num, sb);
                    _Json.住院 = sb.ToString();
                    break;
            }
            sb = null;
            A = null;
            Save(JObject.FromObject(_Json).ToString());
        }

        private static int GetCurrentIndex(string date, eConvertLocation department)
        {
            string location = department == eConvertLocation.OPD ? _Json.門診 : department == eConvertLocation.Care ? _Json.養護 : department == eConvertLocation.Other ? _Json.大寮 : _Json.住院;
            string[] list = location.Split('^');
            int index = 0;
            foreach (string s in list)
            {
                if (s.Trim() == "")
                    continue;
                if (s.Contains(date))
                {
                    index = list.ToList().IndexOf(s);
                    break;
                }
            }
            return index;
        }

        private static int GetCurrentIndexReverse(string date, eConvertLocation department)
        {
            int index = 1;
            string location = department == eConvertLocation.OPD ? _Json.門診 : department == eConvertLocation.Care ? _Json.養護 : department == eConvertLocation.Other ? _Json.大寮 : _Json.住院;
            string[] list = location.Split('^');
            for (int x = list.Length - 1; x >= 0; x--)
            {
                if (list[x].Trim() == "")
                    continue;
                if (list[x].Contains(date))
                {
                    index = list.ToList().IndexOf(list[x]);
                    break;
                }
            }
            return index;
        }

        private static void WriteString(string[] A, int index, string date, int num, StringBuilder sb)
        {
            foreach (string s in A)
            {
                if (A.ToList().IndexOf(s) == index)
                {
                    sb.Append($"{date}|{num}^");
                    continue;
                }
                if (s.Trim() == "")
                    continue;
                sb.Append($"{s}^");
            }
        }

        private static void CreateJson(string date)
        {
            var json = new
            {
                門診 = $"{date}|0^",
                養護 = $"{date}|0^",
                大寮 = $"{date}|0^",
                住院 = $"{date}|0^"
            };
            Save(JObject.FromObject(json).ToString());
        }

        private static void CreateDate(string date, JsonData data)
        {
            Debug.WriteLine("Create Date");
            data.門診 += $"{date}|0^";
            data.養護 += $"{date}|0^";
            data.大寮 += $"{date}|0^";
            data.住院 += $"{date}|0^";
            var json = new
            {
                門診 = data.門診,
                養護 = data.養護,
                大寮 = data.大寮,
                住院 = data.住院
            };
            Save(JObject.FromObject(json).ToString());
        }

        private static void Save(string result)
        {
            using (StreamWriter sw = new StreamWriter($@"{_CurrentPath}\OLEDB_Index.json", false, Encoding.Default))
            {
                sw.WriteLine(result);
            }
        }
    }

    internal class JsonData
    {
        public string 住院 { get; set; }
        public string 養護 { get; set; }
        public string 門診 { get; set; }
        public string 大寮 { get; set; }
    }
}
