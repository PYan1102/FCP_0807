using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using FCP.MVVM.Models;
using FCP.MVVM.Factory;
using System.Collections.Generic;
using System.IO;

namespace FCP.MVVM.ViewModels
{
    class FindFile
    {
        private CancellationTokenSource _CTS { get; set; }
        private SettingsModel _SettingsModel { get; set; }
        private List<string> _InputPathList { get; set; }
        public StartWithParameters OPD { get; set; }
        public StartWithParameters Powder { get; set; }
        public StartWithParameters UDBatch { get; set; }
        public StartWithParameters UDStat { get; set; }
        public StartWithParameters Other { get; set; }

        public FindFile()
        {
            _SettingsModel = SettingsFactory.GenerateSettingsModels();
        }

        public FindFile SetCTS(CancellationTokenSource cts)
        {
            _CTS = cts;
            return this;
        }

        public FindFile SetInputPathList(List<string> list)
        {
            _InputPathList = list;
            return this;
        }
        
        public string GetFile()
        {
            if (_CTS == null)
                return string.Empty;
            try
            {
                while (!_CTS.IsCancellationRequested)
                {
                    Thread.Sleep(_SettingsModel.Speed);
                    foreach (string path in _InputPathList)
                    {
                        if (path.Trim().Length == 0)
                            continue;
                        int index = _InputPathList.IndexOf(path);
                        foreach (string filePath in Directory.GetFiles(path, _SettingsModel.DeputyFileName))
                        {
                            bool _Exit = false;
                            string fileName = Path.GetFileNameWithoutExtension(filePath);
                            JudgeDepartment(fileName);
                            if (Content != "")
                            {
                                string[] FirstWord = Content.Split('|');
                                foreach (string s in FirstWord)
                                {
                                    if (s == "")
                                        continue;
                                    if (Path.GetFileNameWithoutExtension(filePath).Substring(Start, Length) == s)
                                    {
                                        SetConvertValue(index, Path.GetDirectoryName(path), WD.OP, filePath);
                                        SetConvertInformation();
                                        _Exit = true;
                                        break;
                                    }
                                }
                                if (_Exit)
                                    break;
                            }
                            else
                            {
                                SetConvertValue(index, Path.GetDirectoryName(path), WD.OP, filePath);
                                SetConvertInformation();
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                MessageBox.Show($"{FilePath} {ex}");
            }
        }

        private void JudgeDepartment(string fileName)
        {
            
        }
    }

    public class StartWithParameters
    {
        public string StartWith { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
    }
}
