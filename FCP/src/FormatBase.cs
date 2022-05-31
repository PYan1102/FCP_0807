using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Threading;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using FCP.src.MessageManager;
using System.Text;
using System.Windows.Media;
using FCP.src.Dictionary;

namespace FCP.src
{
    abstract class FormatBase : Window
    {
        public FormatBase()
        {
            SettingModel = ModelsFactory.GenerateSettingModel();
        }

        public MainWindowViewModel MainWindowVM { get; set; }
        public SettingJsonModel SettingModel { get; set; }
        private UIRefresh _uiRefresh { get; set; }
        protected internal eDepartment CurrentDepartment;
        private string _successDirectory = string.Empty;
        private string _failDirectory = string.Empty;
        private CancellationTokenSource _cts;
        private List<MatchModel> _matchModel = null;
        private MainWindowModel.ToogleModel _toogleModel;
        private StringBuilder _log = null;

        public virtual void Init()
        {
            try
            {
                _log = new StringBuilder();
                _uiRefresh = new UIRefresh() { UILayout = SetUILayout(new MainUILayoutModel()) };
                _uiRefresh.StartRefreshAsync();
                _matchModel = new List<MatchModel>();

            }
            catch (Exception ex)
            {
                LogService.Exception(ex);
                MsgCollection.ShowDialog(ex);
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
        }

        public virtual void SetMainWindowToogleChecked()
        {
            _toogleModel = WeakReferenceMessenger.Default.Send(new GetMainWindowToogleCheckedRequestMessage());
        }

        public virtual ActionResult PrepareStart()
        {
            ActionResult checkResult = CheckMatchModel();
            if (!checkResult.Success)
            {
                Stop();
                WeakReferenceMessenger.Default.Send(new OpreationMessage(), nameof(eOpreation.Stop));
                MsgCollection.ShowDialog(checkResult.Message, "設定發生錯誤", PackIconKind.Error, dColor.GetSolidColorBrush(eColor.Red));
                return checkResult;
            }
            Start();
            return new ActionResult();
        }

        public void SetFileSearchMode(eFileSearchMode fileSearchMode)
        {
            FileSearchService.SetFileSearchMode(fileSearchMode);
        }

        public void SetOPDRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.OPD, Rule = rule, Enabled = _toogleModel.Toogle1 && CommonModel.CurrentDepartment == eDepartment.OPD, InputDirectory = SettingModel.InputDirectory1 });
        }

        public void SetPowderRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.POWDER, Rule = rule, Enabled = _toogleModel.Toogle2 && CommonModel.CurrentDepartment == eDepartment.OPD, InputDirectory = SettingModel.InputDirectory2 });
        }

        public void SetCareRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.Care, Rule = rule, Enabled = _toogleModel.Toogle3 && CommonModel.CurrentDepartment == eDepartment.OPD, InputDirectory = SettingModel.InputDirectory3 });
        }

        public void SetOtherRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.Other, Rule = rule, Enabled = _toogleModel.Toogle4 && CommonModel.CurrentDepartment == eDepartment.OPD, InputDirectory = SettingModel.InputDirectory4 });
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
            _cts = new CancellationTokenSource();
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
                _cts = null;
                _matchModel.Clear();
            });
        }

        public abstract void Converter();

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
            _uiRefresh.StopRefresh();
        }

        public void ClearLog()
        {
            _log.Clear();
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
            //if (_simpleWindowVM.Visibility == Visibility.Visible)
            //    _simpleWindowVM.StopFunc();
            //else
            //    Stop();
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
            _log.AppendLine($"{DateTime.Now:HH:mm:ss:fff} {result}");
            WeakReferenceMessenger.Default.Send(new LogChangeMessage(_log));
        }

        private ActionResult CheckMatchModel()
        {
            ActionResult result = new ActionResult();
            StringBuilder sb = new StringBuilder();

            if (CommonModel.CurrentDepartment == eDepartment.OPD)
            {
                if ((_toogleModel.Toogle1 || _toogleModel.Toogle2 || _toogleModel.Toogle3 || _toogleModel.Toogle4) == false)
                {
                    result.Success = false;
                    sb.AppendLine("沒有勾選任一個模式");
                }
                if (_toogleModel.Toogle1 && SettingModel.InputDirectory1.Length == 0)
                {
                    result.Success = false;
                    sb.AppendLine("使用輸入路徑1，但路徑沒有被設定");
                }
                if (_toogleModel.Toogle2 && SettingModel.InputDirectory2.Length == 0)
                {
                    result.Success = false;
                    sb.AppendLine("使用輸入路徑2，但路徑沒有被設定");
                }
                if (_toogleModel.Toogle3 && SettingModel.InputDirectory3.Length == 0)
                {
                    result.Success = false;
                    sb.AppendLine("使用輸入路徑3，但路徑沒有被設定");
                }
                if (_toogleModel.Toogle4 && SettingModel.InputDirectory4.Length == 0)
                {
                    result.Success = false;
                    sb.AppendLine("使用輸入路徑4，但路徑沒有被設定");
                }
            }
            else
            {
                if (SettingModel.UseStatOrBatch && SettingModel.StatOrBatch == eDepartment.Stat && SettingModel.InputDirectory6.Length == 0)
                {
                    result.Success = false;
                    sb.AppendLine("使用即時包藥，但路徑沒有被設定");
                }
                else if ((SettingModel.UseStatOrBatch && SettingModel.StatOrBatch == eDepartment.Batch) || SettingModel.InputDirectory5.Length == 0)
                {
                    result.Success = false;
                    sb.AppendLine("使用住院包藥，但路徑沒有被設定");
                }
            }
            if (SettingModel.OutputDirectory.Length == 0)
            {
                result.Success = false;
                sb.AppendLine("輸出路徑沒有被設定");
            }
            result.Message = sb.ToString();
            sb = null;
            return result;
        }
    }
}
