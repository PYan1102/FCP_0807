using FCP.Models;
using FCP.Providers;
using FCP.Services;
using FCP.Services.FileSearchService;
using FCP.src.Enum;
using FCP.src.Factory.Models;
using FCP.src.MessageManager;
using FCP.src.MessageManager.Change;
using FCP.ViewModels;
using Helper;
using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace FCP.src
{
    abstract class ConvertBase : Window
    {
        public ConvertBase()
        {
            SettingModel = ModelsFactory.GenerateSettingModel();
        }

        public bool IsStart { get => _isStart; private set => _isStart = value; }
        public MainWindowViewModel MainWindowVM { get; set; }
        public SettingJsonModel SettingModel { get; set; }
        private UIRefresh _uiRefresh { get; set; }
        protected internal eDepartment CurrentDepartment;
        private string _successDirectory = string.Empty;
        private string _failDirectory = string.Empty;
        private CancellationTokenSource _cts;
        private List<MatchModel> _matchModel = null;
        private MainWindowModel.ToggleModel _toggleModel;
        private bool _isStart = false;

        public virtual void Init()
        {
            try
            {
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

        public virtual void SetMainWindowToogleChecked()
        {
            _toggleModel = WeakReferenceMessenger.Default.Send(new GetMainWindowToogleCheckedRequestMessage());
        }

        public virtual ActionResult PrepareStart()
        {
            ActionResult checkResult = CheckMatchModel();
            if (!checkResult.Success)
            {
                Stop();
                WeakReferenceMessenger.Default.Send(new OpreationMessage(), nameof(eOpreation.Stop));
                MsgCollection.ShowDialog(checkResult.Message, "設定發生錯誤", PackIconKind.Error, ColorProvider.GetSolidColorBrush(eColor.Red));
                return checkResult;
            }
            Start();
            return new ActionResult();
        }

        public void SetFileSearchMode(eFileSearchMode fileSearchMode)
        {
            FileSearchService.SetFileSearchMode(fileSearchMode);
            _matchModel.Clear();
        }

        public void SetOPDRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.OPD, Rule = rule, Enabled = _toggleModel.Toggle1 && CommonModel.CurrentDepartment == eDepartment.OPD, InputDirectory = SettingModel.InputDirectory1 });
        }

        public void SetPowderRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.POWDER, Rule = rule, Enabled = _toggleModel.Toggle2 && CommonModel.CurrentDepartment == eDepartment.OPD, InputDirectory = SettingModel.InputDirectory2 });
        }

        public void SetCareRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.Care, Rule = rule, Enabled = _toggleModel.Toggle3 && CommonModel.CurrentDepartment == eDepartment.OPD, InputDirectory = SettingModel.InputDirectory3 });
        }

        public void SetOtherRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.Other, Rule = rule, Enabled = _toggleModel.Toggle4 && CommonModel.CurrentDepartment == eDepartment.OPD, InputDirectory = SettingModel.InputDirectory4 });
        }

        public void SetBatchRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.Batch, Rule = rule, Enabled = ((SettingModel.UseStatAndBatchOption && SettingModel.StatOrBatch == eDepartment.Batch) || !SettingModel.UseStatAndBatchOption) && CommonModel.CurrentDepartment == eDepartment.UD, InputDirectory = SettingModel.InputDirectory5 });
        }

        public void SetStatRule(string rule = null)
        {
            _matchModel.Add(new MatchModel() { Department = eDepartment.Stat, Rule = rule, Enabled = SettingModel.UseStatAndBatchOption && SettingModel.StatOrBatch == eDepartment.Stat && CommonModel.CurrentDepartment == eDepartment.UD, InputDirectory = SettingModel.InputDirectory6 });
        }

        public virtual void Start()
        {
            _cts = new CancellationTokenSource();
            FileSearchService.Init();
            _isStart = true;
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
                            await Task.Delay(20);
                            Converter();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.Exception(ex);
                    }
                    await Task.Delay(SettingModel.Speed);
                    if (_cts == null)
                    {
                        break;
                    }
                }
                _cts = null;
                _matchModel.Clear();
            });
        }

        public virtual void Stop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _isStart = false;
            }
        }

        public abstract void Converter();

        public void Result(ReturnsResultModel returnsResult, bool remind)
        {
            string message = returnsResult.Message;
            string sourceFilePath = FileInfoModel.SourceFilePath;
            MoveFile convertResult = new MoveFile(sourceFilePath, _successDirectory, _failDirectory);
            Console.WriteLine(message.Length);
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
                    convertResult.LostAdminCode();
                    NoticeService.Notice(returnsResult.Result, message, ToolTipIcon.Error);
                    break;
                case eConvertResult.缺少餐包頻率:
                    convertResult.LostAdminCode();
                    NoticeService.Notice(returnsResult.Result, message, ToolTipIcon.Error);
                    break;
                case eConvertResult.產生OCS失敗:
                    convertResult.Fail();
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
            if (((returnsResult.Result == eConvertResult.缺少種包頻率 || returnsResult.Result == eConvertResult.缺少餐包頻率) && !SettingModel.IgnoreAdminCodeIfNotInOnCube) || SettingModel.WhenCompeletedStop)
            {
                WeakReferenceMessenger.Default.Send(new OpreationMessage(), nameof(eOpreation.Stop));
            }
        }

        public virtual void StopAll()
        {
            Stop();
            _uiRefresh.StopRefresh();
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
            int index = result.IndexOf(" ");
            string newResult = $"{Path.GetFileName(result.Substring(0, index))} {result.Substring(index + 1, result.Length - index - 1)}";
            WeakReferenceMessenger.Default.Send(new LogChangeMessage($"{DateTime.Now:HH:mm:ss:fff} {newResult}"));
        }

        private ActionResult CheckMatchModel()
        {
            ActionResult result = new ActionResult();
            StringBuilder sb = new StringBuilder();

            if (CommonModel.CurrentDepartment == eDepartment.OPD)
            {
                if ((_toggleModel.Toggle1 || _toggleModel.Toggle2 || _toggleModel.Toggle3 || _toggleModel.Toggle4) == false)
                {
                    result.Success = false;
                    sb.AppendLine("沒有勾選任一個模式");
                }
                if (_toggleModel.Toggle1 && SettingModel.InputDirectory1.Length == 0)
                {
                    result.Success = false;
                    sb.AppendLine("使用輸入路徑1，但路徑沒有被設定");
                }
                if (_toggleModel.Toggle2 && SettingModel.InputDirectory2.Length == 0)
                {
                    result.Success = false;
                    sb.AppendLine("使用輸入路徑2，但路徑沒有被設定");
                }
                if (_toggleModel.Toggle3 && SettingModel.InputDirectory3.Length == 0)
                {
                    result.Success = false;
                    sb.AppendLine("使用輸入路徑3，但路徑沒有被設定");
                }
                if (_toggleModel.Toggle4 && SettingModel.InputDirectory4.Length == 0)
                {
                    result.Success = false;
                    sb.AppendLine("使用輸入路徑4，但路徑沒有被設定");
                }
            }
            else
            {
                if (SettingModel.UseStatAndBatchOption && SettingModel.StatOrBatch == eDepartment.Stat && SettingModel.InputDirectory6.Length == 0)
                {
                    result.Success = false;
                    sb.AppendLine("使用即時包藥，但路徑沒有被設定");
                }
                else if ((SettingModel.UseStatAndBatchOption && SettingModel.StatOrBatch == eDepartment.Batch && SettingModel.InputDirectory5.Length == 0) || (!SettingModel.UseStatAndBatchOption && SettingModel.InputDirectory5.Length == 0))
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
