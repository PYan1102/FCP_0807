using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Threading;
using System.Windows.Media;
using FCP.MVVM.Factory.ViewModel;
using FCP.MVVM.Control;
using FCP.MVVM.Models;
using FCP.MVVM.Factory;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.ViewModels.GetConvertFile;
using FCP.MVVM.ViewModels;
using MaterialDesignThemes.Wpf;
using FCP.MVVM.View;
using Helper;

namespace FCP
{
    abstract class FunctionCollections
    {
        public MainWindowViewModel MainWindowVM { get; set; }
        public string IP1, IP2, IP3, OP = "";
        public string PackedType;
        public string FilePath { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public string CurrentSeconds { get; set; }
        public FindFile FindFile { get; set; }
        public MainWindow MainWindow { get; set; }
        public SettingsModel SettingsModel { get; set; }
        private Settings _Settings { get; set; }
        public SimpleWindowView SimpleWindow { get; set; }
        public AdvancedSettings AS;
        List<string> IPList = new List<string>();
        public WindowsDo WD { get; set; }
        private AdvancedSettingsViewModel _AdvancedSettingsVM { get; set; }
        private System.Windows.Forms.NotifyIcon _NotifyIcon;
        private ConvertFileInformtaionModel _ConvertFileInformation { get; set; }
        protected internal DepartmentEnum CurrentDepartment { get; set; }
        private string _OPD { get; set; } = string.Empty;
        private string _Powder { get; set; } = string.Empty;
        private string _UDBatch { get; set; } = string.Empty;
        private string _UDStat { get; set; } = string.Empty;
        private string _Care { get; set; } = string.Empty;
        private string _Other { get; set; } = string.Empty;
        private MsgBViewModel _MsgBVM { get; set; }
        private CancellationTokenSource _CTS;
        private CancellationTokenSource _CTS1;
        private SimpleWindowViewModel _SimpleWindowVM { get; set; }

        public FunctionCollections()
        {
            _Settings = SettingsFactory.GenerateSettingsControl();
            SettingsModel = SettingsFactory.GenerateSettingsModel();
            _ConvertFileInformation = ConvertInfoactory.GenerateConvertFileInformation();
            FindFile = new FindFile();
            _MsgBVM = MsgBFactory.GenerateMsgBViewModel();
            MainWindowVM = MainWindowFactory.GenerateMainWindowViewModel();
            _SimpleWindowVM = SimpleWindowFactory.GenerateSimpleWindowViewModel();
        }

        public void SetNotifyIcon(System.Windows.Forms.NotifyIcon icon)
        {
            _NotifyIcon = icon;
        }

        public virtual void InitFindFileMode(FindFileModeEnum mode)
        {
            FindFile.Init(mode);
        }

        public enum ModeEnum
        {
            OPD = 0, UD = 1
        }

        //建立MsgBox物件
        public virtual void Init()
        {
            try
            {
                WD = new WindowsDo() { AS = AS };
                CheckProgramStart();
                CheckFileBackupPath();
                _CTS1 = new CancellationTokenSource();
                WD.UI_Refresh(_CTS1);
            }
            catch (Exception ex)
            {
                Log.Write($"{ex}");
                _MsgBVM.Show(ex.ToString());
            }
        }

        //檢查程式是否已開啟
        public void CheckProgramStart()
        {
            if (Process.GetProcessesByName("FCP").Length >= 2)
            {
                _MsgBVM.Show("程式已開啟，請確認工具列", "重複開啟", PackIconKind.Error, KindColors.Error);
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

        public virtual void Stop()
        {
            if (_CTS != null)
            {
                _CTS.Cancel();
            }
        }

        public virtual void Save()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Log.Write($"{ex}");
            }
        }

        public virtual void ConvertPrepare(bool isOPD)
        {
            if ((MainWindowVM.InputPath1 + MainWindowVM.InputPath2 + MainWindowVM.InputPath3).Trim().Length == 0)
            {
                if (_CTS != null) Stop();
                _MsgBVM.Show("來源路徑為空白", "路徑空白", PackIconKind.Error, KindColors.Error);
                return;
            }
            if (MainWindowVM.OutputPath.Trim().Length == 0)
            {
                if (_CTS != null) Stop();
                _MsgBVM.Show("輸出路徑為空白", "路徑空白", PackIconKind.Error, KindColors.Error);
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
                if (MainWindowVM.OPDToogle1Enabled)
                    FindFile.SetOPD(_OPD);
                if (MainWindowVM.OPDToogle2Enabled)
                    FindFile.SetPowder(_Powder);
                if (MainWindowVM.OPDToogle3Enabled)
                    FindFile.SetCare(_Care);
                if (MainWindowVM.OPDToogle4Enabled)
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
                        Log.Write(ex.ToString());
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
            CurrentDepartment = DepartmentEnum.OPD;
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
                            foreach (string Name in Directory.GetFiles(IP, SettingsModel.DeputyFileName))
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
                            foreach (string Name in Directory.GetFiles(IP, SettingsModel.DeputyFileName))
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
                        foreach (string Name in Directory.GetFiles(IP, SettingsModel.DeputyFileName))
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
                    Log.Write($"{ex}");
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

        public void Result(ReturnsResultFormat returnsResult, bool isMoveFile, bool isReminder)
        {
            string message = returnsResult.Message;
            string fileName = $"{Path.GetFileName(FilePath)}_{ DateTime.Now:ss_fff}";
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(FilePath);
            string tip;
            switch (returnsResult.Result)
            {
                case ConvertResult.成功:
                    if (isMoveFile)
                    {
                        File.Move(FilePath, $@"{MainWindowVM.SuccessPath}\{fileName}.ok");
                        WD.SuccessCountAdd();
                    }
                    tip = $"{fileNameWithoutExtension} {nameof(ConvertResult.成功)}";
                    AddNewMessageToProgressBox(tip);
                    _NotifyIcon.ShowBalloonTip(850, nameof(ConvertResult.成功), tip, System.Windows.Forms.ToolTipIcon.None);
                    break;
                case ConvertResult.全數過濾:
                    if (isMoveFile)
                    {
                        File.Move(FilePath, $@"{MainWindowVM.SuccessPath}\{fileName}.ok");
                        WD.SuccessCountAdd();
                    }
                    if (isReminder)
                    {
                        tip = $"{fileNameWithoutExtension} {nameof(ConvertResult.全數過濾)}";
                        AddNewMessageToProgressBox(tip);
                        _NotifyIcon.ShowBalloonTip(850, nameof(ConvertResult.全數過濾), tip, System.Windows.Forms.ToolTipIcon.None);
                    }
                    break;
                case ConvertResult.沒有種包頻率:
                    Stop();
                    AddNewMessageToProgressBox(message);
                    _NotifyIcon.ShowBalloonTip(850, $"缺少頻率", $"{Path.GetFileName(FilePath)} OnCube中缺少該檔案 {message} 的種包頻率", System.Windows.Forms.ToolTipIcon.Error);
                    break;
                case ConvertResult.沒有餐包頻率:
                    Stop();
                    AddNewMessageToProgressBox(message);
                    _NotifyIcon.ShowBalloonTip(850, $"缺少頻率", $"{Path.GetFileName(FilePath)} OnCube中缺少該檔案 {message} 的餐包頻率", System.Windows.Forms.ToolTipIcon.Error);
                    break;
                default:
                    if (isMoveFile)
                    {
                        File.Move(FilePath, $@"{MainWindowVM.FailPath}\{fileName}.fail");
                        WD.FailCountAdd();
                    }
                    AddNewMessageToProgressBox($"{returnsResult.Result} {message}");
                    _NotifyIcon.ShowBalloonTip(850, "轉檔錯誤", message, System.Windows.Forms.ToolTipIcon.Error);
                    break;
            }
            //Stop();
        }

        public virtual void ProgressBoxClear()
        {
            WD.ProgressBoxClear();
        }

        private void AddNewMessageToProgressBox(string Result)
        {
            WD.ProgressBoxAdd($"{DateTime.Now:HH:mm:ss:fff} {Result}");
        }

        public virtual void CloseSelf()
        {
            Stop();
            _CTS1.Cancel();
            WD = null;
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

    public class WindowsDo : Window
    {
        public MainWindow MainWindow { get; set; }
        private SettingsModel _SettingsModel { get; set; }
        private Settings _Settings { get; set; }
        private MsgBViewModel _MsgBVM { get; set; }
        public AdvancedSettings AS { get; set; }
        private SimpleWindowViewModel _SimpleWindowVM { get; set; }
        private MainWindowViewModel _MainWindowVM { get => MainWindowFactory.GenerateMainWindowViewModel(); }

        public WindowsDo()
        {
            _Settings = SettingsFactory.GenerateSettingsControl();
            _SettingsModel = SettingsFactory.GenerateSettingsModel();
            _MsgBVM = MsgBFactory.GenerateMsgBViewModel();
            _SimpleWindowVM = SimpleWindowFactory.GenerateSimpleWindowViewModel();
        }

        public async void UI_Refresh(CancellationTokenSource cts)
        {
            while (!cts.IsCancellationRequested)
            {
                await Task.Delay(500);
                try
                {
                    _SimpleWindowVM.SetWindowPosition(Properties.Settings.Default.X, Properties.Settings.Default.Y);
                    if (!_MainWindowVM.StopEnabled)
                    {
                        UILayout UI = new UILayout();
                        switch (_SettingsModel.Mode)
                        {
                            case Format.小港醫院TOC:
                                小港ToOnCube(UI);
                                break;
                            case Format.光田醫院TOC:
                                光田ToOnCube(UI);
                                break;
                            case Format.光田醫院TJVS:
                                光田ToJVServer(UI);
                                break;
                            case Format.民生醫院TOC:
                                民生ToOnCube(UI);
                                break;
                            case Format.義大醫院TOC:
                                義大ToOnCube(UI);
                                break;
                            case Format.長庚磨粉TJVS:
                                長庚磨粉ToJVServer(UI);
                                break;
                            case Format.長庚醫院TOC:
                                長庚醫院ToOnCube(UI);
                                break;
                            case Format.仁康醫院TOC:
                                仁康醫院ToOnCube(UI);
                                break;
                            default:
                                JVServerToOnCube(UI);
                                break;
                        }
                        RefreshUIPropertyServices.RefrehMainWindowUI(UI);
                        _MainWindowVM.DoseType = _SettingsModel.DoseType.ToString();
                    }
                    _MainWindowVM.StatVisibility = _SettingsModel.EN_StatOrBatch ? Visibility.Visible : Visibility.Hidden;
                    _MainWindowVM.BatchVisibility = _SettingsModel.EN_StatOrBatch ? Visibility.Visible : Visibility.Hidden;
                    _MainWindowVM.MinimumAndCloseVisibility = _SettingsModel.EN_ShowControlButton ? Visibility.Visible : Visibility.Hidden;
                    _MainWindowVM.WindowXVisibility = _SettingsModel.EN_ShowXY ? Visibility.Visible : Visibility.Hidden;
                    _MainWindowVM.WindowYVisibility = _SettingsModel.EN_ShowXY ? Visibility.Visible : Visibility.Hidden;
                }
                catch (Exception a)
                {
                    _MsgBVM.Show(a.ToString(), "錯誤", PackIconKind.Error, KindColors.Error);
                    Log.Write(a.ToString());
                }
            }
        }

        private void JVServerToOnCube(UILayout UI)
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
        }

        private void 小港ToOnCube(UILayout UI)
        {
            UI.Title = "小港醫院 > OnCube";
            UI.IP1Title = "門   診";
            UI.IP2Title = "磨   粉";
            UI.IP3Title = "住   院";
            UI.OPDToogle1 = "門診";
            UI.OPDToogle2 = "磨粉";
            UI.OPDToogle3 = "慢籤";
            UI.OPDToogle4 = "急診";
            UI.IP1Enabled = true;
            UI.IP2Enabled = true;
            UI.IP3Enabled = true;
            UI.UDVisibility = Visibility.Visible;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Visible;
            UI.OPD3Visibility = Visibility.Visible;
            UI.OPD4Visibility = Visibility.Visible;
        }

        private void 光田ToOnCube(UILayout UI)
        {
            UI.Title = "光田醫院 > OnCube";
            UI.IP1Title = "門   診";
            UI.IP2Title = "輸入路徑2";
            UI.IP3Title = "住   院";
            UI.OPDToogle1 = "門診";
            UI.OPDToogle2 = "";
            UI.OPDToogle3 = "";
            UI.OPDToogle4 = "";
            UI.IP1Enabled = true;
            UI.IP2Enabled = false;
            UI.IP3Enabled = true;
            UI.UDVisibility = Visibility.Visible;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
        }

        private void 光田ToJVServer(UILayout UI)
        {
            UI.Title = "光田醫院 > JVServer";
            UI.IP1Title = "輸入路徑1";
            UI.IP2Title = "磨   粉";
            UI.IP3Title = "輸入路徑3";
            UI.OPDToogle1 = "";
            UI.OPDToogle2 = "磨粉";
            UI.OPDToogle3 = "";
            UI.OPDToogle4 = "";
            UI.IP1Enabled = false;
            UI.IP2Enabled = true;
            UI.IP3Enabled = false;
            UI.UDVisibility = Visibility.Hidden;
            UI.OPD1Visibility = Visibility.Hidden;
            UI.OPD2Visibility = Visibility.Visible;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
        }

        private void 民生ToOnCube(UILayout UI)
        {
            UI.Title = "民生醫院 > OnCube";
            UI.IP1Title = "輸入路徑1";
            UI.IP2Title = "輸入路徑1";
            UI.IP3Title = "住   院";
            UI.OPDToogle1 = "門診";
            UI.OPDToogle2 = "";
            UI.OPDToogle3 = "養護";
            UI.OPDToogle4 = "大寮";
            UI.IP1Enabled = true;
            UI.IP2Enabled = true;
            UI.IP3Enabled = true;
            UI.UDVisibility = Visibility.Visible;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Visible;
            UI.OPD4Visibility = Visibility.Visible;
        }

        private void 義大ToOnCube(UILayout UI)
        {
            UI.Title = "義大醫院 > OnCube";
            UI.IP1Title = "輸入路徑1";
            UI.IP2Title = "輸入路徑2";
            UI.IP3Title = "住   院";
            UI.OPDToogle1 = "";
            UI.OPDToogle2 = "";
            UI.OPDToogle3 = "";
            UI.OPDToogle4 = "";
            UI.IP1Enabled = false;
            UI.IP2Enabled = false;
            UI.IP3Enabled = true;
            UI.UDVisibility = Visibility.Visible;
            UI.OPD1Visibility = Visibility.Hidden;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
        }

        private void 長庚磨粉ToJVServer(UILayout UI)
        {
            UI.Title = "長庚磨粉 > JVServer";
            UI.IP1Title = "磨粉";
            UI.IP2Title = "輸入路徑2";
            UI.IP3Title = "輸入路徑3";
            UI.OPDToogle1 = "";
            UI.OPDToogle2 = "磨粉";
            UI.OPDToogle3 = "";
            UI.OPDToogle4 = "";
            UI.IP1Enabled = true;
            UI.IP2Enabled = false;
            UI.IP3Enabled = false;
            UI.UDVisibility = Visibility.Hidden;
            UI.OPD1Visibility = Visibility.Hidden;
            UI.OPD2Visibility = Visibility.Visible;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
        }

        private void 長庚醫院ToOnCube(UILayout UI)
        {
            UI.Title = "長庚醫院 > OnCube";
            UI.IP1Title = "門診";
            UI.IP2Title = "藥來速";
            UI.IP3Title = "住院";
            UI.OPDToogle1 = "門診";
            UI.OPDToogle2 = "";
            UI.OPDToogle3 = "";
            UI.OPDToogle4 = "藥來速";
            UI.IP1Enabled = true;
            UI.IP2Enabled = true;
            UI.IP3Enabled = true;
            UI.UDVisibility = Visibility.Visible;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Visible;
        }

        private void 仁康醫院ToOnCube(UILayout UI)
        {
            UI.Title = "仁康醫院 > OnCube";
            UI.IP1Title = "門診";
            UI.IP2Title = "輸入路徑2";
            UI.IP3Title = "住院";
            UI.OPDToogle1 = "門診";
            UI.OPDToogle2 = "";
            UI.OPDToogle3 = "";
            UI.OPDToogle4 = "";
            UI.IP1Enabled = true;
            UI.IP2Enabled = false;
            UI.IP3Enabled = true;
            UI.UDVisibility = Visibility.Visible;
            UI.OPD1Visibility = Visibility.Visible;
            UI.OPD2Visibility = Visibility.Hidden;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
        }

        public void ProgressBoxClear()
        {
            MainWindow.Txt_ProgressBox.Clear();
            
        }

        public void ProgressBoxAdd(string result)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                _MainWindowVM.AddLog(result);
                //MainWindow.Txt_ProgressBox.ScrollToEnd();
                _SimpleWindowVM.AddLog(result);
            }));
        }

        public void SuccessCountAdd()
        {
            Dispatcher.InvokeAsync(new Action(() => { _MainWindowVM.SuccessCount = (Convert.ToInt32(_MainWindowVM.SuccessCount) + 1).ToString(); }));
        }

        public void FailCountAdd()
        {
            Dispatcher.InvokeAsync(new Action(() => { _MainWindowVM.FailCount = (Convert.ToInt32(_MainWindowVM.FailCount) + 1).ToString(); }));
        }
    }

    public class UILayout
    {
        public string Title { get; set; }
        public string IP1Title { get; set; }
        public string IP2Title { get; set; }
        public string IP3Title { get; set; }
        public string OPDToogle1 { get; set; }
        public string OPDToogle2 { get; set; }
        public string OPDToogle3 { get; set; }
        public string OPDToogle4 { get; set; }
        public bool IP1Enabled { get; set; }
        public bool IP2Enabled { get; set; }
        public bool IP3Enabled { get; set; }
        public Visibility UDVisibility { get; set; }
        public Visibility OPD1Visibility { get; set; }
        public Visibility OPD2Visibility { get; set; }
        public Visibility OPD3Visibility { get; set; }
        public Visibility OPD4Visibility { get; set; }
    }
}
