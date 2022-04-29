using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Threading;
using FCP.src.Factory.ViewModel;
using FCP.Models;
using FCP.src.Factory;
using FCP.src.Enum;
using FCP.ViewModels;
using MaterialDesignThemes.Wpf;
using Helper;
using FCP.src.Factory.Models;
using FCP.Services.FileSearchService;
using System.Linq;

namespace FCP.src
{
    abstract class FormatBase : Window
    {
        public MainWindowViewModel MainWindowVM { get; set; }
        public string IP1, IP2, IP3, OP = "";
        public string PackedType;
        public string SourceFilePath { get; set; }
        public string InputDirectory { get; set; }
        public string OutputDirectory { get; set; }
        public string CurrentSeconds { get; set; }
        public SettingModel SettingModel { get; set; }
        public UIRefresh UIRefresh { get; set; }
        public static System.Windows.Forms.NotifyIcon NotifyIcon { get; set; }
        private AdvancedSettingsViewModel _advancedSettingsVM;
        private SimpleWindowViewModel _simpleWindowVM;
        protected internal eDepartment CurrentDepartment;
        private string _successDirectory = string.Empty;
        private string _failDirectory = string.Empty;
        private CancellationTokenSource _cts;
        private List<MatchModel> _matchModel = null;
        private List<string> _opdDirectory = null;
        private List<string> _udDirectory = null;

        public FormatBase()
        {
            SettingModel = SettingFactory.GenerateSettingModel();
            MainWindowVM = MainWindowFactory.GenerateMainWindowViewModel();
            _simpleWindowVM = SimpleWindowFactory.GenerateSimpleWindowViewModel();
        }

        public virtual void Init()
        {
            try
            {
                UIRefresh = new UIRefresh() { UILayout = SetUILayout(new UILayout()) };
                UIRefresh.StartAsync();
                _matchModel = new List<MatchModel>();
                _opdDirectory = new List<string>();
                _udDirectory = new List<string>();

            }
            catch (Exception ex)
            {
                Log.Write(ex);
                MsgCollection.Show(ex);
            }
        }

        public virtual UILayout SetUILayout(UILayout UI)
        {
            UI.Title = "通用格式";
            UI.IP1Title = "輸入路徑1";
            UI.IP2Title = "輸入路徑2";
            UI.IP3Title = "輸入路徑3";
            UI.IP4Title = "輸入路徑4";
            UI.IP5Title = "Batch";
            UI.IP6Title = "Stat";
            UI.OPDToogle1 = "門診";
            UI.OPDToogle2 = "";
            UI.OPDToogle3 = "";
            UI.OPDToogle4 = "";
            UI.IP1Enabled = true;
            UI.IP2Enabled = true;
            UI.IP3Enabled = true;
            UI.IP4Enabled = true;
            UI.IP5Enabled = false;
            UI.IP6Enabled = false;
            UI.UDVisibility = Visibility.Hidden;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
            return UI;
        }

        public virtual void Stop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
            }
        }

        public virtual void ConvertPrepare()
        {
            _opdDirectory.Add(SettingModel.InputDirectory1);
            _opdDirectory.Add(SettingModel.InputDirectory2);
            _opdDirectory.Add(SettingModel.InputDirectory3);
            _opdDirectory.Add(SettingModel.InputDirectory4);
            _udDirectory.Add(SettingModel.InputDirectory5);
            _udDirectory.Add(SettingModel.InputDirectory6);
            if (CommonModel.CurrentDepartment == eDepartment.OPD)
            {
                if (_opdDirectory.Where(x => x.Trim().Length == 0).Count() == _opdDirectory.Count)
                {
                    MsgCollection.Show("來源路徑為空白", "路徑空白", PackIconKind.Error, KindColors.Error);
                    return;
                }
            }
            else
            {
                if (_udDirectory.Where(x => x.Trim().Length == 0).Count() == _udDirectory.Count)
                {
                    MsgCollection.Show("住院路徑為空白", "路徑空白", PackIconKind.Error, KindColors.Error);
                    return;
                }
            }
            if (SettingModel.OutputDirectory.Trim().Length == 0)
            {
                MsgCollection.Show("輸出路徑為空白", "路徑空白", PackIconKind.Error, KindColors.Error);
                return;
            }
            RefreshUIPropertyServices.SwitchUIStateForStart();
            RefreshUIPropertyServices.SwitchMainWindowControlState(false);
            _advancedSettingsVM = AdvancedSettingFactory.GenerateAdvancedSettingsViewModel();
            _advancedSettingsVM.Visibility = Visibility.Hidden;
            _cts = new CancellationTokenSource();
        }

        public void SetFileSearchMode(eFileSearchMode fileSearchMode)
        {
            FileSearchService.SetFileSearchMode(fileSearchMode);
        }

        public void SetOPDRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.OPD, Rule = rule, Enabled = MainWindowVM.OPDToogle1Checked });
        }

        public void SetPowderRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.POWDER, Rule = rule, Enabled = MainWindowVM.OPDToogle2Checked });
        }

        public void SetCareRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.Care, Rule = rule, Enabled = MainWindowVM.OPDToogle3Checked });
        }

        public void SetOtherRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.Other, Rule = rule, Enabled = MainWindowVM.OPDToogle4Checked });
        }

        public void SetBatchRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.Batch, Rule = rule, Enabled = SettingModel.UseStatOrBatch && SettingModel.StatOrBatch == eDepartment.Batch });
        }

        public void SetStatRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.Stat, Rule = rule, Enabled = SettingModel.UseStatOrBatch && SettingModel.StatOrBatch == eDepartment.Stat });
        }

        public virtual void Start()
        {
            if (_cts == null)
                return;
            FileSearchService.Init();
            Task.Run(() =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    Clear();
                    CheckBackupDirectory();
                    try
                    {
                        FileSearchService.GetFileInfo(_matchModel, CommonModel.CurrentDepartment == eDepartment.OPD ? _opdDirectory : _udDirectory);
                        if (!string.IsNullOrEmpty(FileInfoModel.SourceFilePath))
                        {
                            Converter();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex);
                    }
                }
            });
        }

        public virtual void Converter()
        {

        }

        public void Result(ReturnsResultFormat returnsResult, bool isReminder)
        {
            string message = returnsResult.Message;
            string fileName = $"{Path.GetFileName(SourceFilePath)}_{ DateTime.Now:ss_fff}";
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(SourceFilePath);
            string tip;
            switch (returnsResult.Result)
            {
                case eConvertResult.成功:
                    if (SettingModel.MoveSourceFileToBackupDirectoryWhenDone)
                    {
                        File.Move(SourceFilePath, $@"{_successDirectory}\{fileName}.ok");
                    }
                    tip = $"{fileNameWithoutExtension} {nameof(eConvertResult.成功)}";
                    Log.Write($"{fileNameWithoutExtension} {nameof(eConvertResult.成功)}");
                    AddNewMessageToProgressBox(tip);
                    NotifyIcon.ShowBalloonTip(850, nameof(eConvertResult.成功), tip, System.Windows.Forms.ToolTipIcon.None);
                    break;
                case eConvertResult.全數過濾:
                    if (SettingModel.MoveSourceFileToBackupDirectoryWhenDone)
                    {
                        File.Move(SourceFilePath, $@"{_successDirectory}\{fileName}.ok");
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
                    NotifyIcon.ShowBalloonTip(850, $"缺少頻率", $"{Path.GetFileName(SourceFilePath)} OnCube中缺少該檔案 {message} 的種包頻率", System.Windows.Forms.ToolTipIcon.Error);
                    break;
                case eConvertResult.沒有餐包頻率:
                    Dispatcher.Invoke(new Action(() => MainWindowVM.StopFunc()));
                    AddNewMessageToProgressBox(message);
                    NotifyIcon.ShowBalloonTip(850, $"缺少頻率", $"{Path.GetFileName(SourceFilePath)} OnCube中缺少該檔案 {message} 的餐包頻率", System.Windows.Forms.ToolTipIcon.Error);
                    break;
                default:
                    if (SettingModel.MoveSourceFileToBackupDirectoryWhenDone)
                    {
                        File.Move(SourceFilePath, $@"{_failDirectory}\{fileName}.fail");
                    }
                    AddNewMessageToProgressBox($"{returnsResult.Result} {message}");
                    NotifyIcon.ShowBalloonTip(850, "轉檔錯誤", message, System.Windows.Forms.ToolTipIcon.Error);
                    break;
            }
            if (SettingModel.StopWhenDone)
            {
                Dispatcher.Invoke(new Action(() => MainWindowVM.StopFunc()));
            }
        }

        public virtual void StopAll()
        {
            Stop();
            UIRefresh.Stop();
        }

        public string MergeFilesAndGetNewFilePath(string InputDirectory, string fileName, int start, int length, string content)
        {
            MergeFiles mergeFiles = new MergeFiles(InputDirectory, fileName);
            mergeFiles.Merge(start, length, content);
            return mergeFiles.GetMergedFilePath;
        }

        //移動檔案
        public void MoveFilesIncludeResult(bool isSuccess)
        {
            string folderName = isSuccess ? "Success" : "Fail";
            string extension = isSuccess ? "ok" : "fail";
            string[] files = Directory.GetFiles($@"{CommonModel.FileBackupRootDirectory}\{DateTime.Now:yyyy-MM-dd}\Temp");
            foreach (string s in files)
            {
                File.Move(s, $@"{CommonModel.FileBackupRootDirectory}\{DateTime.Now:yyyy-MM-dd}\{folderName}\{Path.GetFileNameWithoutExtension(s)}.{extension}");
            }
            if (_simpleWindowVM.Visibility == Visibility.Visible)
                _simpleWindowVM.StopFunc();
            else
                Stop();
        }

        private void Clear()
        {
            InputDirectory = string.Empty;
            OutputDirectory = MainWindowVM.OutputDirectory;
            SourceFilePath = string.Empty;
            CurrentSeconds = string.Empty;
            CurrentDepartment = eDepartment.OPD;
        }

        //檢查備份資料夾是否存在
        private void CheckBackupDirectory()
        {
            string backupDirectory = $@"{CommonModel.FileBackupRootDirectory}\{DateTime.Now:yyyy-MM-dd}";
            if (!Directory.Exists($@"{backupDirectory}\Success"))
            {
                Directory.CreateDirectory($@"{backupDirectory}\Success");
            }
            if (!Directory.Exists($@"{backupDirectory}\Fail"))
            {
                Directory.CreateDirectory($@"{backupDirectory}\Fail");
            }
            _successDirectory = $@"{backupDirectory}\Success";
            _failDirectory = $@"{backupDirectory}\Fail";
        }



        private void SetConvertValue(int MethodID, string IP, string OP, string File)
        {
            int MethodID1 = MethodID;
            InputDirectory = IP;
            OutputDirectory = OP;
            SourceFilePath = File;
        }

        private void AddNewMessageToProgressBox(string result)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                MainWindowVM.AddLog($"{DateTime.Now:HH:mm:ss:fff} {result}");
                //MainWindow.Txt_ProgressBox.ScrollToEnd();
                _simpleWindowVM.AddLog($"{DateTime.Now:HH:mm:ss:fff} {result}");
            }));
        }
    }
}
