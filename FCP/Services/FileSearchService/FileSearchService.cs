using FCP.Models;
using FCP.src.Enum;
using FCP.src.Factory.Models;
using FCP.src.Interface;
using System;
using System.Collections.Generic;
using System.IO;

namespace FCP.Services.FileSearchService
{
    internal class FileSearchService
    {
        private static eFileSearchMode _fileSearchMode;
        private static IGetFile _getFile = null;
        private static SettingJsonModel _settingModel => ModelsFactory.GenerateSettingModel();

        public static void SetFileSearchMode(eFileSearchMode fileSearchMode)
        {
            _fileSearchMode = fileSearchMode;
        }

        public static void Init()
        {
            switch (_fileSearchMode)
            {
                case eFileSearchMode.根據檔名開頭:
                    _getFile = new FileNameHead();
                    break;
                case eFileSearchMode.根據輸入路徑:
                    _getFile = new InputDirectory();
                    break;
                default:
                    throw new Exception($"發現未適配的檔案尋找模式 {_fileSearchMode}");
            }
        }

        public static void GetFileInfo(List<MatchModel> matchModel)
        {
            try
            {
                string sourceFilePath = _getFile.GetFile(_settingModel.FileExtensionName, matchModel);
                if (string.IsNullOrEmpty(sourceFilePath))
                {
                    return;
                }
                FileInfoModel.InputDirectory = Path.GetDirectoryName(sourceFilePath);
                FileInfoModel.Department = _getFile.GetDepartment;
                FileInfoModel.SourceFilePath = sourceFilePath;
                FileInfoModel.CurrentDateTime = DateTime.Now;
            }
            catch
            {
                throw;
            }
        }
    }
}
