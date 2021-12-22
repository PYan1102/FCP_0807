﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Threading;
using FCP.src.Factory.ViewModel;
using FCP.MVVM.Models;
using FCP.src.Factory;
using FCP.src.Enum;
using FCP.MVVM.ViewModels.GetConvertFile;
using FCP.MVVM.ViewModels;
using MaterialDesignThemes.Wpf;
using Helper;
using FCP.src.Factory.Models;

namespace FCP.src
{
    abstract class FunctionCollections : Window
    {
        public MainWindowViewModel MainWindowVM { get; set; }
        public string IP1, IP2, IP3, OP = "";
        public string PackedType;
        public string FilePath { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public string CurrentSeconds { get; set; }
        public FindFile FindFile { get; set; }
        public SettingsModel SettingsModel { get; set; }
        public List<string> IPList = new List<string>();
        public UIRefresh UIRefresh { get; set; }
        public static System.Windows.Forms.NotifyIcon NotifyIcon { get; set; }
        private AdvancedSettingsViewModel _AdvancedSettingsVM { get; set; }
        private ConvertFileInformtaionModel _ConvertFileInformation { get; set; }
        protected internal eConvertLocation CurrentDepartment { get; set; }
        private string _OPD { get; set; } = string.Empty;
        private string _Powder { get; set; } = string.Empty;
        private string _UDBatch { get; set; } = string.Empty;
        private string _UDStat { get; set; } = string.Empty;
        private string _Care { get; set; } = string.Empty;
        private string _Other { get; set; } = string.Empty;
        private CancellationTokenSource _CTS;
        private CancellationTokenSource _CTS1;
        private SimpleWindowViewModel _SimpleWindowVM { get; set; }

        public FunctionCollections()
        {
            SettingsModel = SettingsFactory.GenerateSettingsModel();
            _ConvertFileInformation = ConvertInfoactory.GenerateConvertFileInformation();
            FindFile = new FindFile();
            MainWindowVM = MainWindowFactory.GenerateMainWindowViewModel();
            _SimpleWindowVM = SimpleWindowFactory.GenerateSimpleWindowViewModel();
        }

        public virtual void InitFindFileMode(eFindFileMode mode)
        {
            FindFile.Init(mode);
        }

        //建立MsgBox物件
        public virtual void Init()
        {
            try
            {
                CheckProgramStart();
                CheckFileBackupPath();
                _CTS1 = new CancellationTokenSource();
                UIRefresh = new UIRefresh() { CTS = _CTS1, UILayout = SetUILayout(new UILayout()) };
                UIRefresh.StartAsync();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                Message.Show(ex);
            }
        }

        //檢查程式是否已開啟
        public void CheckProgramStart()
        {
            if (Process.GetProcessesByName("FCP").Length > 1)
            {
                Message.Show("程式已開啟，請確認工具列", "重複開啟", PackIconKind.Error, KindColors.Error);
                Log.Write("程式已開啟，請確認工具列");
                Environment.Exit(0);
            }
        }

        //檢查備份資料夾是否存在
        public void CheckFileBackupPath()
        {
            string BackupPath = $@"{MainWindowVM.FileBackupPath}\{DateTime.Now:yyyy-MM-dd}";
            if (!Directory.Exists($@"{BackupPath}\Success")) Directory.CreateDirectory($@"{BackupPath}\Success");
            if (!Directory.Exists($@"{BackupPath}\Fail")) Directory.CreateDirectory($@"{BackupPath}\Fail");
            MainWindowVM.SuccessPath = $@"{BackupPath}\Success";
            MainWindowVM.FailPath = $@"{BackupPath}\Fail";
        }

        public virtual UILayout SetUILayout(UILayout UI)
        {
            UI.Title = "Normal > OnCube";
            UI.IP1Title = "輸入路徑1";
            UI.IP2Title = "輸入路徑2";
            UI.IP3Title = "輸入路徑3";
            UI.OPDToogle1 = "門診";
            UI.OPDToogle2 = "";
            UI.OPDToogle3 = "";
            UI.OPDToogle4 = "";
            UI.IP1Enabled = true;
            UI.IP2Enabled = true;
            UI.IP3Enabled = true;
            UI.UDVisibility = Visibility.Hidden;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
            return UI;
        }

        public virtual void Stop()
        {
            if (_CTS != null)
            {
                _CTS.Cancel();
            }
        }

        public virtual void ConvertPrepare(bool isOPD)
        {
            if ((MainWindowVM.InputPath1 + MainWindowVM.InputPath2 + MainWindowVM.InputPath3).Trim().Length == 0)
            {
                if (_CTS != null) Stop();
                Message.Show("來源路徑為空白", "路徑空白", PackIconKind.Error, KindColors.Error);
                return;
            }
            if (MainWindowVM.OutputPath.Trim().Length == 0)
            {
                if (_CTS != null) Stop();
                Message.Show("輸出路徑為空白", "路徑空白", PackIconKind.Error, KindColors.Error);
                return;
            }
            RefreshUIPropertyServices.SwitchUIStateForStart(isOPD);
            RefreshUIPropertyServices.SwitchMainWindowControlState(false);
            IPList.Clear();
            if (isOPD)
            {
                IPList.Add(MainWindowVM.InputPath1);
                IPList.Add(MainWindowVM.InputPath2);
            }
            else
            {
                IPList.Add(MainWindowVM.InputPath3);
            }
            _AdvancedSettingsVM = AdvancedSettingsFactory.GenerateAdvancedSettingsViewModel();
            _AdvancedSettingsVM.Visibility = Visibility.Hidden;
            _CTS = null;
            _CTS = new CancellationTokenSource();
            FindFile.ResetRuleToEmpty();
        }

        public void SetOPDRule(string rule)
        {
            _OPD = rule;
        }

        public void SetPowderRule(string rule)
        {
            _Powder = rule;
        }

        public void SetUDBatchRule(string rule)
        {
            _UDBatch = rule;
        }

        public void SetUDStatRule(string rule)
        {
            _UDStat = rule;
        }

        public void SetCareRule(string rule)
        {
            _Care = rule;
        }

        public void SetOtherRule(string rule)
        {
            _Other = rule;
        }

        public void SetIntoProperty(bool isOPD)
        {
            if (isOPD)
            {
                if (MainWindowVM.OPDToogle1Checked)
                    FindFile.SetOPD(_OPD);
                if (MainWindowVM.OPDToogle2Checked)
                    FindFile.SetPowder(_Powder);
                if (MainWindowVM.OPDToogle3Checked)
                    FindFile.SetCare(_Care);
                if (MainWindowVM.OPDToogle4Checked)
                    FindFile.SetOther(_Other);
            }
            else
            {
                if (MainWindowVM.StatChecked)
                    FindFile.SetUDStat(_UDStat);
                else
                    FindFile.SetUDBatch(_UDBatch);

            }
            FindFile.Reset(_CTS, IPList);
        }

        public virtual void GetFileAsync()
        {
            if (_CTS == null)
                return;
            FindFile.SetDepartmentDictionary();
            Task.Run(() =>
            {
                while (!_CTS.IsCancellationRequested)
                {
                    Clear();
                    CheckFileBackupPath();
                    try
                    {
                        Task<FileInformation> fileInformation = FindFile.GetFileInfo();
                        InputPath = fileInformation.Result.InputPath;
                        FilePath = fileInformation.Result.FilePath;
                        CurrentDepartment = fileInformation.Result.Department;
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex);
                    }
                    if (!string.IsNullOrEmpty(FilePath))
                        SetConvertInformation();
                }
            });
        }

        private void Clear()
        {
            InputPath = string.Empty;
            OutputPath = MainWindowVM.OutputPath;
            FilePath = string.Empty;
            CurrentSeconds = string.Empty;
            CurrentDepartment = eConvertLocation.OPD;
        }

        public virtual void Loop_OPD(int Start, int Length, string Content)
        {
            if (_CTS == null)
                return;
            Task.Run(async () =>
            {
                //GetFileNameTaskAsync();
                try
                {
                    while (!_CTS.IsCancellationRequested)
                    {
                        await Task.Delay(SettingsModel.Speed);
                        foreach (string IP in IPList)
                        {
                            int Index = IPList.IndexOf(IP);
                            if (IP.Trim() == "")
                                continue;
                            foreach (string Name in Directory.GetFiles(IP, SettingsModel.FileExtensionName))
                            {
                                bool _Exit = false;
                                if (Content != "")
                                {
                                    string[] FirstWord = Content.Split('|');
                                    foreach (string s in FirstWord)
                                    {
                                        if (s == "")
                                            continue;
                                        if (Path.GetFileNameWithoutExtension(Name).Substring(Start, Length) == s)
                                        {
                                            SetConvertValue(Index, Path.GetDirectoryName(IP), MainWindowVM.OutputPath, Name);
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
                                    SetConvertValue(Index, Path.GetDirectoryName(IP), MainWindowVM.OutputPath, Name);
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
            });
        }

        public virtual void Loop_OPD_小港()
        {
            if (_CTS == null)
                return;
            Task.Run(async () =>
            {
                try
                {
                    while (!_CTS.IsCancellationRequested)
                    {
                        await Task.Delay(SettingsModel.Speed);
                        foreach (string IP in IPList)
                        {
                            int Index = IPList.IndexOf(IP);
                            if (IP.Trim() == "")
                                continue;
                            foreach (string Name in Directory.GetFiles(IP, SettingsModel.FileExtensionName))
                            {
                                string FileName = Path.GetFileNameWithoutExtension(Name);
                                if (FileName.Contains(" "))  //門診、急診、慢籤
                                {
                                    string[] Split = FileName.Split(' ');
                                    int No = Convert.ToInt32(Split[1]);

                                    if (MainWindowVM.OPDToogle1Checked & 1 <= No & No <= 4999)
                                    {
                                        SetConvertValue(Index, Path.GetDirectoryName(IP), MainWindowVM.OutputPath, Name);
                                        SetConvertInformation();
                                        break;
                                    }
                                    else if (MainWindowVM.OPDToogle2Checked & 6001 <= No & No <= 7999)
                                    {
                                        SetConvertValue(Index, Path.GetDirectoryName(IP), MainWindowVM.OutputPath, Name);
                                        SetConvertInformation();
                                        break;
                                    }
                                    else if (MainWindowVM.OPDToogle3Checked & 9001 <= No & No < 19999)
                                    {
                                        SetConvertValue(Index, Path.GetDirectoryName(IP), MainWindowVM.OutputPath, Name);
                                        SetConvertInformation();
                                        break;
                                    }
                                }
                                else if (MainWindowVM.OPDToogle4Checked) //磨粉
                                {
                                    SetConvertValue(Index, Path.GetDirectoryName(IP), MainWindowVM.OutputPath, Name);
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
            });
        }

        public virtual void Loop_UD(int Start, int Length, string Content)
        {
            if (_CTS == null)
                return;
            Task.Run(async () =>
            {
                try
                {
                    while (!_CTS.IsCancellationRequested)
                    {
                        await Task.Delay(SettingsModel.Speed);
                        string IP = MainWindowVM.InputPath3;
                        if (IP.Trim() == "")
                            continue;
                        foreach (string Name in Directory.GetFiles(IP, SettingsModel.FileExtensionName))
                        {
                            if (SettingsModel.EN_StatOrBatch)
                            {
                                if (Path.GetFileNameWithoutExtension(Name).Substring(Start, Length) == Content)
                                {
                                    SetConvertValue(2, Path.GetDirectoryName(IP), MainWindowVM.OutputPath, Name);
                                    SetConvertInformation();
                                    break;
                                };
                            }
                            else
                            {
                                SetConvertValue(2, Path.GetDirectoryName(IP), MainWindowVM.OutputPath, Name);
                                SetConvertInformation();
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    MessageBox.Show($"{FilePath} {ex}");
                }
            });
        }

        private void SetConvertValue(int MethodID, string IP, string OP, string File)
        {
            int MethodID1 = MethodID;
            InputPath = IP;
            OutputPath = OP;
            FilePath = File;
        }

        public virtual void SetConvertInformation()
        {
            CurrentSeconds = DateTime.Now.ToString("ss.ffff");
            _ConvertFileInformation.SetInputPath(InputPath)
                 .SetOutputPath(OutputPath)
                 .SetFilePath(FilePath)
                 .SetDepartment(CurrentDepartment)
                 .SetCurrentSeconds(CurrentSeconds);
        }

        public void Result(ReturnsResultFormat returnsResult, bool isReminder)
        {
            string message = returnsResult.Message;
            string fileName = $"{Path.GetFileName(FilePath)}_{ DateTime.Now:ss_fff}";
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(FilePath);
            string tip;
            switch (returnsResult.Result)
            {
                case eConvertResult.成功:
                    if (SettingsModel.EN_WhenCompeletedMoveFile)
                    {
                        File.Move(FilePath, $@"{MainWindowVM.SuccessPath}\{fileName}.ok");
                        Dispatcher.InvokeAsync(new Action(() => { MainWindowVM.SuccessCount = (Convert.ToInt32(MainWindowVM.SuccessCount) + 1).ToString(); }));
                    }
                    tip = $"{fileNameWithoutExtension} {nameof(eConvertResult.成功)}";
                    Log.Write($"{fileNameWithoutExtension} {nameof(eConvertResult.成功)}");
                    AddNewMessageToProgressBox(tip);
                    NotifyIcon.ShowBalloonTip(850, nameof(eConvertResult.成功), tip, System.Windows.Forms.ToolTipIcon.None);
                    break;
                case eConvertResult.全數過濾:
                    if (SettingsModel.EN_WhenCompeletedMoveFile)
                    {
                        File.Move(FilePath, $@"{MainWindowVM.SuccessPath}\{fileName}.ok");
                        Dispatcher.InvokeAsync(new Action(() => { MainWindowVM.SuccessCount = (Convert.ToInt32(MainWindowVM.SuccessCount) + 1).ToString(); }));
                    }
                    if (isReminder)
                    {
                        tip = $"{fileNameWithoutExtension} {nameof(eConvertResult.全數過濾)}";
                        AddNewMessageToProgressBox(tip);
                        NotifyIcon.ShowBalloonTip(850, nameof(eConvertResult.全數過濾), tip, System.Windows.Forms.ToolTipIcon.None);
                    }
                    Log.Write($"{fileNameWithoutExtension} {nameof(eConvertResult.全數過濾)}");
                    break;
                case eConvertResult.沒有種包頻率:
                    Dispatcher.Invoke(new Action(() => MainWindowVM.StopFunc()));
                    AddNewMessageToProgressBox(message);
                    NotifyIcon.ShowBalloonTip(850, $"缺少頻率", $"{Path.GetFileName(FilePath)} OnCube中缺少該檔案 {message} 的種包頻率", System.Windows.Forms.ToolTipIcon.Error);
                    break;
                case eConvertResult.沒有餐包頻率:
                    Dispatcher.Invoke(new Action(() => MainWindowVM.StopFunc()));
                    AddNewMessageToProgressBox(message);
                    NotifyIcon.ShowBalloonTip(850, $"缺少頻率", $"{Path.GetFileName(FilePath)} OnCube中缺少該檔案 {message} 的餐包頻率", System.Windows.Forms.ToolTipIcon.Error);
                    break;
                default:
                    if (SettingsModel.EN_WhenCompeletedMoveFile)
                    {
                        File.Move(FilePath, $@"{MainWindowVM.FailPath}\{fileName}.fail");
                        Dispatcher.InvokeAsync(new Action(() => { MainWindowVM.FailCount = (Convert.ToInt32(MainWindowVM.FailCount) + 1).ToString(); }));
                    }
                    AddNewMessageToProgressBox($"{returnsResult.Result} {message}");
                    NotifyIcon.ShowBalloonTip(850, "轉檔錯誤", message, System.Windows.Forms.ToolTipIcon.Error);
                    break;
            }
            if (SettingsModel.EN_WhenCompeletedStop)
            {
                Dispatcher.Invoke(new Action(() => MainWindowVM.StopFunc()));
            }
        }

        private void AddNewMessageToProgressBox(string result)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                MainWindowVM.AddLog($"{DateTime.Now:HH:mm:ss:fff} {result}");
                //MainWindow.Txt_ProgressBox.ScrollToEnd();
                _SimpleWindowVM.AddLog($"{DateTime.Now:HH:mm:ss:fff} {result}");
            }));
        }

        public virtual void CloseSelf()
        {
            Stop();
            UIRefresh.Stop();
        }

        public string MergeFilesAndGetNewFilePath(string inputPath, string fileName, int start, int length, string content)
        {
            MergeFiles mf = new MergeFiles()
                .SetInputPath(inputPath)
                .SetFileName(fileName)
                .Merge(start, length, content);
            return mf._NewFilePath;
        }

        //移動檔案
        public void MoveFilesIncludeResult(bool isSuccess)
        {
            string folderName = isSuccess ? "Success" : "Fail";
            string extension = isSuccess ? "ok" : "fail";
            string[] files = Directory.GetFiles($@"D:\Converter_Backup\{DateTime.Now:yyyy-MM-dd}\Batch");
            foreach (string s in files)
            {
                File.Move(s, $@"D:\Converter_Backup\{DateTime.Now:yyyy-MM-dd}\{folderName}\{Path.GetFileNameWithoutExtension(s)}.{extension}");
            }
            if (_SimpleWindowVM.Visibility == Visibility.Visible)
                _SimpleWindowVM.StopFunc();
            else
                Stop();
        }
    }
}