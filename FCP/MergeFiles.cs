using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FCP
{
    class MergeFiles
    {
        private string _FileName { get; set; }
        private string _InputPath { get; set; }
        public string _NewFilePath { get; set; }
        public MergeFiles SetInputPath(string inputPath)
        {
            _InputPath = inputPath;
            return this;
        }

        public MergeFiles SetFileName(string name)
        {
            _FileName = name;
            return this;
        }

        public MergeFiles Merge(int Start, int Length, string Word)
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
            return this;
        }

        public string GetContent(string Name)
        {
            return File.ReadAllText($@"{_InputPath}\{Name}", Encoding.Default);
        }

        private void CreateFile(string Content)
        {
            _NewFilePath = $@"{_InputPath}\{_FileName}_{DateTime.Now:ss_fff}.txt";
            using (StreamWriter sw = new StreamWriter(_NewFilePath, false, Encoding.Default))
            {
                sw.Write(Content);
            }
        }

        private void MoveFiles(List<string>Files)
        {
            foreach(string s in Files)
            {
                File.Move($@"{_InputPath}\{s}", $@"D:\Converter_Backup\{DateTime.Now:yyyy-MM-dd}\Batch\{Path.GetFileNameWithoutExtension(s)}_{DateTime.Now:ss_fff}.txt");
            }
        }

        private List<string> GetFiles(int Start, int Length, string Word)
        {
            var Files = Directory.GetFiles($@"{_InputPath}\");
            return Files.ToList().Where(x => Path.GetFileName(x).Substring(Start, Length) == Word).Select(x => Path.GetFileName(x)).ToList();
        }

        private void CheckTempFolder()
        {
            if (!Directory.Exists($@"D:\Converter_Backup\{DateTime.Now:yyyy-MM-dd}\Batch"))
                Directory.CreateDirectory($@"D:\Converter_Backup\{DateTime.Now:yyyy-MM-dd}\Batch");
        }
    }
}
