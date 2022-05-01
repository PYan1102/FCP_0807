using FCP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCP.Services
{
    public sealed class MergeFileService
    {
        public string GetMergedFilePath { get => _mergedFilePath; }
        private string _fileName { get; set; }
        private string _inputDirectory { get; set; }
        private string _mergedFilePath { get; set; }

        public MergeFileService(string inputDirectory, string fileName)
        {
            _inputDirectory = inputDirectory;
            _fileName = fileName;
        }

        /// <summary>
        /// 合併檔案，如果檔案名稱符合輸入參數的話
        /// </summary>
        /// <param name="start">檔案名稱的起始位置</param>
        /// <param name="length">檔案名稱的長度</param>
        /// <param name="value">存在於檔案名稱內的字串</param>
        public void Merge(int start, int length, string value)
        {
            CheckTempDirectory();
            List<string> files = GetInputDirectoryFiles(start, length, value);
            if (files.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string file in files)
                {
                    sb.Append(GetFileContent(file));
                }
                GenerateMergedFile(sb.ToString());
                MoveFilesToBackupDirectory(files);
            }
        }

        private string GetFileContent(string fileName)
        {
            return File.ReadAllText($@"{_inputDirectory}\{fileName}", Encoding.Default);
        }

        private void GenerateMergedFile(string content)
        {
            _mergedFilePath = $@"{_inputDirectory}\{_fileName}_{FileInfoModel.CurrentDateTime:ss_fff}.txt";
            using (StreamWriter sw = new StreamWriter(_mergedFilePath, false, Encoding.Default))
            {
                sw.Write(content);
            }
        }

        private void MoveFilesToBackupDirectory(List<string> files)
        {
            foreach (string file in files)
            {
                string destFileName = $@"{CommonModel.FileBackupRootDirectory}\{FileInfoModel.CurrentDateTime:yyyy-MM-dd}\Temp\{Path.GetFileNameWithoutExtension(file)}_{FileInfoModel.CurrentDateTime:ss_fff}.txt";
                File.Move($@"{_inputDirectory}\{file}", destFileName);
            }
        }

        private List<string> GetInputDirectoryFiles(int start, int length, string value)
        {
            string[] files = Directory.GetFiles($@"{_inputDirectory}\");
            return (from file in files
                    let fileName = Path.GetFileName(file)
                    where fileName.Substring(start, length) == value
                    select fileName).ToList();
        }

        private void CheckTempDirectory()
        {
            if (!Directory.Exists($@"{CommonModel.FileBackupRootDirectory}\{FileInfoModel.CurrentDateTime:yyyy-MM-dd}\Temp"))
                Directory.CreateDirectory($@"{CommonModel.FileBackupRootDirectory}\{FileInfoModel.CurrentDateTime:yyyy-MM-dd}\Temp");
        }
    }
}
