using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Threading;
using FCP.src.Factory.ViewModel;
using FCP.Models;
using FCP.src.Enum;
using FCP.ViewModels;
using MaterialDesignThemes.Wpf;
using Helper;
using FCP.Services.FileSearchService;
using FCP.src.Factory.Models;
using FCP.Services;
using System.Windows.Forms;
using System.Windows.Threading;

namespace FCP.src
{
    abstract class FormatBase : Window
    {
        public MainWindowViewModel MainWindowVM { get; set; }
        public SettingModel SettingModel { get; set; }
        private UIRefresh _uiRefresh { get; set; }
        private AdvancedSettingsViewModel _advancedSettingsVM;
        private SimpleWindowViewModel _simpleWindowVM;
        protected internal eDepartment CurrentDepartment;
        private string _successDirectory = string.Empty;
        private string _failDirectory = string.Empty;
        private CancellationTokenSource _cts;
        private List<MatchModel> _matchModel = null;

        public FormatBase()
        {
            SettingModel = ModelsFactory.GenerateSettingModel();
            MainWindowVM = MainWindowFactory.GenerateMainWindowViewModel();
            _simpleWindowVM = SimpleWindowFactory.GenerateSimpleWindowViewModel();
        }

        public virtual void Init()
        {
            try
            {
                _uiRefresh = new UIRefresh() { UILayout = SetUILayout(new MainUILayoutModel()) };
                _uiRefresh.StartAsync();
                _matchModel = new List<MatchModel>();

            }
            catch (Exception ex)
            {
                Log.Write(ex);
                MsgCollection.Show(ex);
            }
        }

        public virtual MainUILayoutModel SetUILayout(MainUILayoutModel UI)
        {
            return UI;
        }

        public virtual void Stop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
            }
            _matchModel.Clear();
        }

        public virtual void ConvertPrepare()
        {
            if (CommonModel.CurrentDepartment == eDepartment.OPD)
            {
                if ((SettingModel.InputDirectory1 + SettingModel.InputDirectory2 + SettingModel.InputDirectory3 + SettingModel.InputDirectory4 + SettingModel.InputDirectory5 + SettingModel.InputDirectory6).Trim().Length == 0)
                {
                    MsgCollection.Show("來源路徑為空白", "路徑空白", PackIconKind.Error, KindColors.Error);
                    return;
                }
            }
            else
            {
                if ((SettingModel.InputDirectory5 + SettingModel.InputDirectory6).Trim().Length == 0)
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
            _matchModel.Add(new MatchModel() { Department = eDepartment.OPD, Rule = rule, Enabled = MainWindowVM.OPDToogle1Checked && CommonModel.CurrentDepartment == eDepartment.OPD, InputDirectory = SettingModel.InputDirectory1 });
        }

        public void SetPowderRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.POWDER, Rule = rule, Enabled = MainWindowVM.OPDToogle2Checked && CommonModel.CurrentDepartment == eDepartment.OPD, InputDirectory = SettingModel.InputDirectory2 });
        }

        public void SetCareRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.Care, Rule = rule, Enabled = MainWindowVM.OPDToogle3Checked && CommonModel.CurrentDepartment == eDepartment.OPD, InputDirectory = SettingModel.InputDirectory3 });
        }

        public void SetOtherRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.Other, Rule = rule, Enabled = MainWindowVM.OPDToogle4Checked && CommonModel.CurrentDepartment == eDepartment.OPD, InputDirectory = SettingModel.InputDirectory4 });
        }

        public void SetBatchRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.Batch, Rule = rule, Enabled = SettingModel.UseStatOrBatch && SettingModel.StatOrBatch == eDepartment.Batch && CommonModel.CurrentDepartment == eDepartment.UD, InputDirectory = SettingModel.InputDirectory5 });
        }

        public void SetStatRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.Stat, Rule = rule, Enabled = SettingModel.UseStatOrBatch && SettingModel.StatOrBatch == eDepartment.Stat && CommonModel.CurrentDepartment == eDepartment.UD, InputDirectory = SettingModel.InputDirectory6 });
        }

        public virtual void Start()
        {
            if (_cts == null)
                return;
            FileSearchService.Init();
            Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    ClearFileInfoModel();
                    CheckBackupDirectory();
                    try
                    {
                        FileSearchService.GetFileInfo(_matchModel);
                        if (!string.IsNullOrEmpty(FileInfoModel.SourceFilePath) && File.Exists(FileInfoModel.SourceFilePath))
                        {
                            Converter();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.Exception(ex);
                    }
                    await Task.Delay(SettingModel.Speed);
                }
            });
        }

        public virtual void Converter()
        {

        }

        public void Result(ReturnsResultModel returnsResult, bool remind)
        {
            string message = returnsResult.Message;
            string sourceFilePath = FileInfoModel.SourceFilePath;
            ConvertResult convertResult = new ConvertResult(sourceFilePath, _successDirectory, _failDirectory);
            switch (returnsResult.Result)
            {
                case eConvertResult.成功:
                    convertResult.Success();
                    NoticeService.Notice(returnsResult.Result, message, ToolTipIcon.None);
                    break;
                case eConvertResult.全數過濾:
                    convertResult.Pass();
                    if (remind)
                    {
                        AddNewMessageToProgressBox(message);
                        NoticeService.Notice(returnsResult.Result, message, ToolTipIcon.None);
                    }
                    break;
                case eConvertResult.缺少種包頻率:
                    NoticeService.Notice(returnsResult.Result, message, ToolTipIcon.Error);
                    break;
                case eConvertResult.缺少餐包頻率:
                    NoticeService.Notice(returnsResult.Result, message, ToolTipIcon.Error);
                    break;
                default:
                    convertResult.Fail();
                    NoticeService.Notice(returnsResult.Result, message, ToolTipIcon.Error);
                    break;
            }
            if (returnsResult.Result != eConvertResult.全數過濾)
            {
                AddNewMessageToProgressBox(message);
            }
            if (returnsResult.Result == eConvertResult.缺少種包頻率 || returnsResult.Result == eConvertResult.缺少餐包頻率 || SettingModel.StopWhenDone)
            {
                Dispatcher.Invoke(new Action(() => MainWindowVM.StopFunc()));
            }
        }

        public virtual void StopAll()
        {
            Stop();
            _uiRefresh.Stop();
        }

        public string MergeFilesAndGetNewFilePath(string InputDirectory, string fileName, int start, int length, string content)
        {
            MergeFileService mergeFiles = new MergeFileService(InputDirectory, fileName);
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

        private void ClearFileInfoModel()
        {
            FileInfoModel.Clear();
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
