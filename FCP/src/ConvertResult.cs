using FCP.Models;
using FCP.src.Factory.Models;
using System;
using System.IO;

namespace FCP.src
{
    public class ConvertResult
    {
        public ConvertResult(string sourceFilePath, string successDirectory, string failDirectory)
        {
            _settingModel = ModelsFactory.GenerateSettingModel();

            _sourceFilePath = sourceFilePath;
            _fileName = $"{Path.GetFileName(sourceFilePath)}_{ DateTime.Now:ss_fff}";
            _successDirectory = successDirectory;
            _failDirectory = failDirectory;
        }

        private SettingModel _settingModel;
        private string _sourceFilePath;
        private string _fileName;
        private string _successDirectory;
        private string _failDirectory;

        public void Success()
        {
            if (_settingModel.MoveSourceFileToBackupDirectoryWhenDone)
            {
                File.Move(_sourceFilePath, $@"{_successDirectory}\{_fileName}.ok");
            }
        }

        public void Pass()
        {
            if (_settingModel.MoveSourceFileToBackupDirectoryWhenDone)
            {
                File.Move(_sourceFilePath, $@"{_successDirectory}\{_fileName}.pass");
            }
        }

        public void LostCombiDoseAdminCode()
        {

        }

        public void LostMultiDoseAdminCode()
        {

        }

        public void Fail()
        {
            if (_settingModel.MoveSourceFileToBackupDirectoryWhenDone)
            {
                File.Move(_sourceFilePath, $@"{_failDirectory}\{_fileName}.fail");
            }
        }
    }
}
