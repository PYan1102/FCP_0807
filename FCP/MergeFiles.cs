using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace FCP
{
    class MergeFiles
    {
        string Name { get; set; }
        string InputPath { get; set; }
        public string FileName { get; set; }
        public void SetValue(string In, string Name)
        {
            InputPath = In;
            this.Name = Name;
        }

        public void Merge(int Start, int Length, string Word)
        {
            CheckTempFolder();
            List<string> Files = GetFiles(Start, Length, Word);
            if (Files.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string s in Files)
                {
                    sb.Append(GetContent(s));
                }
                CreateFile(sb.ToString());
                MoveFiles(Files);
            }
        }

        private void CreateFile(string Content)
        {
            FileName = $@"{InputPath}\{Name}_{DateTime.Now:ss_fff}.txt";
            using (StreamWriter sw = new StreamWriter(FileName, false, Encoding.Default))
            {
                sw.Write(Content);
            }
        }

        private void MoveFiles(List<string>Files)
        {
            foreach(string s in Files)
            {
                File.Move($@"{InputPath}\{s}", $@"D:\Converter_Backup\{DateTime.Now:yyyy-MM-dd}\Batch\{Path.GetFileNameWithoutExtension(s)}_{DateTime.Now:ss_fff}.txt");
            }
        }

        public string GetContent(string Name)
        {
            return File.ReadAllText($@"{InputPath}\{Name}", Encoding.Default);
        }

        private List<string> GetFiles(int Start, int Length, string Word)
        {
            var Files = Directory.GetFiles($@"{InputPath}\");
            return Files.ToList().Where(x => Path.GetFileName(x).Substring(Start, Length) == Word).Select(x => Path.GetFileName(x)).ToList();
        }

        private void CheckTempFolder()
        {
            if (!Directory.Exists($@"D:\Converter_Backup\{DateTime.Now:yyyy-MM-dd}\Batch"))
                Directory.CreateDirectory($@"D:\Converter_Backup\{DateTime.Now:yyyy-MM-dd}\Batch");
        }
    }
}
