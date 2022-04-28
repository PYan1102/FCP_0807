using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
using FCP.src.Enum;
using System.Collections.Generic;

namespace FCP.Service
{
    static class JsonService
    {
        private static JsonData _json { get; set; }
        private static string _currentPath = Environment.CurrentDirectory;
        private static string _getJsonContent
        {
            get
            {
                using (StreamReader sr = new StreamReader($@"{_currentPath}\OLEDB_Index.json", Encoding.Default))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static void JudgeJsonFileIsAlreadyCreated(string date)
        {
            string jsonContent = _getJsonContent;
            var jObject = JObject.Parse(jsonContent);
            if (jObject["門診"] == null)
            {
                GenerateJsonFile(date);
                JudgeJsonFileIsAlreadyCreated(date);
                return;
            }
            _json = null;
            _json = new JsonData() { 門診 = $"{jObject["門診"]}", 養護 = $"{jObject["養護"]}", 大寮 = $"{jObject["大寮"]}", 住院 = $"{jObject["住院"]}" };
            string[] list = _json.門診.Split('^');
            foreach (string s in list)
            {
                if (s.Trim() == "")
                    continue;
                if (s.Contains(date))
                    break;
                if (list.ToList().IndexOf(s) == list.Length - 2)  //遇空白continue，所以長度多減1
                {
                    CreateDate(date, _json);
                    break;
                }
            }
        }

        public static int GetOLEDBIndex(eDepartment department, string date)
        {
            int index = GetCurrentIndexReverse(date, department);
            switch (department)
            {
                case eDepartment.OPD:
                    return int.Parse(_json.門診.Split('^')[index].Split('|')[1]);
                case eDepartment.Care:
                    return int.Parse(_json.養護.Split('^')[index].Split('|')[1]);
                case eDepartment.Other:
                    return int.Parse(_json.大寮.Split('^')[index].Split('|')[1]);
                case eDepartment.UDBatch:
                    return int.Parse(_json.住院.Split('^')[index].Split('|')[1]);
                default:
                    return -1;
            }
        }

        public static void UpdateJson(string date, eDepartment department, int count)
        {
            if (count == 0)
                return;
            var jObject = JObject.Parse(_getJsonContent);
            _json = null;
            _json = new JsonData() { 門診 = $"{jObject["門診"]}", 養護 = $"{jObject["養護"]}", 大寮 = $"{jObject["大寮"]}", 住院 = $"{jObject["住院"]}" };
            int index = GetCurrentIndex(date, department);
            List<string> list = new List<string>();
            switch (department)
            {
                case eDepartment.OPD:
                    list = _json.門診.Split('^').ToList();
                    _json.門診 = AppendDepartmentInfo(list, index, date, count);
                    break;
                case eDepartment.Care:
                    list = _json.養護.Split('^').ToList();
                    _json.養護 = AppendDepartmentInfo(list, index, date, count);
                    break;
                case eDepartment.Other:
                    list = _json.大寮.Split('^').ToList();
                    _json.大寮 = AppendDepartmentInfo(list, index, date, count);
                    break;
                case eDepartment.UDBatch:
                    list = _json.住院.Split('^').ToList();
                    _json.住院 = AppendDepartmentInfo(list, index, date, count);
                    break;
            }
            Save(JObject.FromObject(_json));
        }

        private static int GetCurrentIndex(string date, eDepartment department)
        {
            string location = department == eDepartment.OPD ? _json.門診 : department == eDepartment.Care ? _json.養護 : department == eDepartment.Other ? _json.大寮 : _json.住院;
            List<string> list = location.Split('^').ToList();
            foreach (string s in list)
            {
                if (s.Trim() == "")
                    continue;
                if (s.Contains(date))
                {
                    return list.IndexOf(s);
                }
            }
            return 0;
        }

        private static int GetCurrentIndexReverse(string date, eDepartment department)
        {
            string location = department == eDepartment.OPD ? _json.門診 : department == eDepartment.Care ? _json.養護 : department == eDepartment.Other ? _json.大寮 : _json.住院;
            List<string> list = location.Split('^').ToList();
            list.Reverse();
            foreach (string s in list)
            {
                if (s.Trim() == "")
                    continue;
                if (s.Contains(date))
                {
                    return list.IndexOf(s);
                }
            }
            return 1;
        }

        private static string AppendDepartmentInfo(List<string> list, int index, string date, int num)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in list)
            {
                if (list.IndexOf(s) == index)
                {
                    sb.Append($"{date}|{num}^");
                    continue;
                }
                if (s.Trim() == "")
                    continue;
                sb.Append($"{s}^");
            }
            return sb.ToString();
        }

        private static void GenerateJsonFile(string date)
        {
            var json = new
            {
                門診 = $"{date}|0^",
                養護 = $"{date}|0^",
                大寮 = $"{date}|0^",
                住院 = $"{date}|0^"
            };
            Save(JObject.FromObject(json));
        }

        private static void CreateDate(string date, JsonData data)
        {
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
            Save(JObject.FromObject(json));
        }

        private static void Save(JObject jObject)
        {
            using (StreamWriter sw = new StreamWriter($@"{_currentPath}\OLEDB_Index.json", false, Encoding.Default))
            {
                sw.WriteLine(jObject.ToString());
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
