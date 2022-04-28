using FCP.Models;
using FCP.src.Enum;
using FCP.src.Factory;
using FCP.src.Factory.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FCP.Service
{
    internal class FileSearchService
    {
        private static IGetFile _getFile = null;
        private static Dictionary<eDepartment, string> _inputDirectory = null;
        private static SettingModel _settingModel => SettingFactory.GenerateSettingModel();

        public static void Init(eFileSearchMode mode, Dictionary<eDepartment, string> inputDirectory)
        {
            switch (mode)
            {
                case eFileSearchMode.根據檔名開頭:
                    _getFile = new FileNameHead();
                    break;
                case eFileSearchMode.根據輸入路徑:
                    _getFile = new InputDirectory();
                    break;
                default:
                    throw new Exception($"發現未適配的檔案尋找模式 {mode}");
            }
            _inputDirectory = new Dictionary<eDepartment, string>();
            _getFile.InputDirectoryList = inputDirectory;
        }

        public static void GetFileInfo()
        {
            _getFile.GetFile(_settingModel.FileExtensionName);
        }
    }

    class FileNameHead : IGetFile
    {
        public Dictionary<eDepartment, string> InputDirectoryList { get; set; }

        public void GetFile(string extensionName)
        {
            foreach (var v in InputDirectoryList)
            {
                string[] files = Directory.GetFiles(v.Value, $"*.{extensionName}");
                if (files.Length > 0)
                {
                    FileInfoModel.SourceFilePath = files[0];
                    FileInfoModel.InputDirectory = v.Value;
                    FileInfoModel.CurrentDateTime = DateTime.Now;
                    FileInfoModel.Department = v.Key;
                    return;
                }
            }
        }
    }

    class InputDirectory : IGetFile
    {
        public Dictionary<eDepartment, string> InputDirectoryList { get; set; }

        public void GetFile(string extensionName)
        {
        }
    }

    interface IGetFile
    {
        Dictionary<eDepartment, string> InputDirectoryList { get; set; }
        void GetFile(string extensionName);
    }
}
