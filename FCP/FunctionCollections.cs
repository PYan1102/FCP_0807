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
using FCP.MVVM.Helper;
using FCP.MVVM.ViewModels;
using MaterialDesignThemes.Wpf;

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
        public readonly static string FileBackupPath = @"D:\Converter_Backup";
        public SettingsModel SettingsModel { get; set; }
        private Settings _Settings { get; set; }
        public SmallForm SF;
        public AdvancedSettings AS;
        List<string> IPList = new List<string>();
        public WindowsDo WD { get; set; }
        private System.Windows.Forms.NotifyIcon _NotifyIcon = new System.Windows.Forms.NotifyIcon();
        private AdvancedSettingsViewModel _AdvancedSettingsVM { get; set; }
        private string SuccessPath { get; set; }
        private string FailPath { get; set; }
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

        public FunctionCollections()
        {
            _Settings = SettingsFactory.GenerateSettingsControl();
            SettingsModel = SettingsFactory.GenerateSettingsModel();
            _ConvertFileInformation = ConvertInfoactory.GenerateConvertFileInformation();
            FindFile = new FindFile();
            _MsgBVM = MsgBFactory.GenerateMsgBViewModel();
            MainWindowVM = MainWindowFacotry.GenerateMainWindowViewModel();
        }

        public void SetWindow(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
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
                SF = new SmallForm(MainWindow) { Owner = MainWindow };
                WD = new WindowsDo() { MainWindow = MainWindow, SF = SF, AS = AS };
                CheckProgramStart();
                CheckFileBackupPath();
                CreateNotifyIcon();
                RefreshUIPropertyServices.InitMainWindowUI();
                RefreshUIPropertyServices.SwitchMainWindowControlState(true);
                _CTS1 = new CancellationTokenSource();
                WD.UI_Refresh(_CTS1);
                //MainWindow.Dispatcher.InvokeAsync(new Action(() => ));
            }
            catch (Exception ex)
            {
                Log.Write($"{ex}");
                MessageBox.Show($"{ex}");
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
        public  void CheckFileBackupPath()
        {
            string BackupPath = $@"{FileBackupPath}\{DateTime.Now:yyyy-MM-dd}";
            if (!Directory.Exists($@"{BackupPath}\Success")) Directory.CreateDirectory($@"{BackupPath}\Success");
            if (!Directory.Exists($@"{BackupPath}\Fail")) Directory.CreateDirectory($@"{BackupPath}\Fail");
            SuccessPath = $@"{BackupPath}\Success";
            FailPath = $@"{BackupPath}\Fail";
        }

        public void AllWindowShowOrHide(bool b1, bool b2, bool b3)
        {
            WD.AllWindowShowOrHide(b1, b2, b3);
        }

        private void CreateNotifyIcon()
        {
            _NotifyIcon.Icon = Properties.Resources.FCP;
            _NotifyIcon.Visible = true;
            _NotifyIcon.Text = "轉檔";
            _NotifyIcon.DoubleClick += NotifyIconDBClick;
        }

        public void NotifyIconDBClick(object sender, EventArgs e)
        {
            WD.NotifyIconDBClick();
        }

        public virtual void ShowAdvancedSettings()
        {
            WD.ShowAdvancedSettings();
        }

        public virtual void Stop()
        {
            if (_CTS != null) _CTS.Cancel();
            WD.Stop_Control();
            WD.SwitchControlEnabled(true);
            _CTS = null;
        }

        public virtual void Save()
        {
            try
            {
                Properties.Settings.Default.X = WD.X;
                Properties.Settings.Default.Y = WD.Y;
                Properties.Settings.Default.Save();

                _Settings.SaveMainWidow(MainWindowVM.InputPath1, MainWindowVM.InputPath2, MainWindowVM.InputPath3, MainWindowVM.OutputPath, MainWindowVM.IsAutoStartChecked, MainWindowVM.StatChecked ? "S" : "B");
                AddNewMessageToProgressBox("儲存成功");
            }
            catch (Exception ex)
            {
                Log.Write($"{ex}");
            }
        }

        public void ChangeWindow()
        {
            WD.ChangeWindow();
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
            WD.SwitchConverterButtonColor(isOPD);
            RefreshUIPropertyServices.SwitchMainWindowControlState(false);
            IPList.Clear();
            if (isOPD)
            {
                IPList.Add(WD.IP1);
                IPList.Add(WD.IP2);
            }
            else
            {
                IPList.Add(WD.IP3);
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
                if (WD._OPD1)
                    FindFile.SetOPD(_OPD);
                if (WD._OPD2)
                    FindFile.SetPowder(_Powder);
                if (WD._OPD3)
                    FindFile.SetCare(_Care);
                if (WD._OPD4)
                    FindFile.SetOther(_Other);
            }
            else
            {
                if (WD._IsStat)
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
            OutputPath = WD.OP;
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
                                            SetConvertValue(Index, Path.GetDirectoryName(IP), WD.OP, Name);
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
                                    SetConvertValue(Index, Path.GetDirectoryName(IP), WD.OP, Name);
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

                                    if (WD._OPD1 & 1 <= No & No <= 4999)
                                    {
                                        SetConvertValue(Index, Path.GetDirectoryName(IP), WD.OP, Name);
                                        SetConvertInformation();
                                        break;
                                    }
                                    else if (WD._OPD2 & 6001 <= No & No <= 7999)
                                    {
                                        SetConvertValue(Index, Path.GetDirectoryName(IP), WD.OP, Name);
                                        SetConvertInformation();
                                        break;
                                    }
                                    else if (WD._OPD3 & 9001 <= No & No < 19999)
                                    {
                                        SetConvertValue(Index, Path.GetDirectoryName(IP), WD.OP, Name);
                                        SetConvertInformation();
                                        break;
                                    }
                                }
                                else if (WD._OPD4) //磨粉
                                {
                                    SetConvertValue(Index, Path.GetDirectoryName(IP), WD.OP, Name);
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
                        string IP = WD.IP3;
                        if (IP.Trim() == "")
                            continue;
                        foreach (string Name in Directory.GetFiles(IP, SettingsModel.DeputyFileName))
                        {
                            if (SettingsModel.EN_StatOrBatch)
                            {
                                if (Path.GetFileNameWithoutExtension(Name).Substring(Start, Length) == Content)
                                {
                                    SetConvertValue(2, Path.GetDirectoryName(IP), WD.OP, Name);
                                    SetConvertInformation();
                                    break;
                                };
                            }
                            else
                            {
                                SetConvertValue(2, Path.GetDirectoryName(IP), WD.OP, Name);
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
                        File.Move(FilePath, $@"{SuccessPath}\{fileName}.ok");
                        WD.SuccessCountAdd();
                    }
                    tip = $"{fileNameWithoutExtension} {nameof(ConvertResult.成功)}";
                    AddNewMessageToProgressBox(tip);
                    _NotifyIcon.ShowBalloonTip(850, nameof(ConvertResult.成功), tip, System.Windows.Forms.ToolTipIcon.None);
                    break;
                case ConvertResult.全數過濾:
                    if (isMoveFile)
                    {
                        File.Move(FilePath, $@"{SuccessPath}\{fileName}.ok");
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
                        File.Move(FilePath, $@"{FailPath}\{fileName}.fail");
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

        public virtual async void AutoStart()
        {
            await Task.Delay(1000);
            WD.AutoStart();
        }

        public void CMD(string Command)
        {
            Process p = new Process();
            p.StartInfo.FileName = "CMD.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.Start();
            p.StandardInput.WriteLine(Command);
            p.StandardInput.WriteLine("exit");
            p.Close();
        }

        public string[] CMD(List<string> L)
        {
            Process p = new Process();
            p.StartInfo.FileName = "CMD.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            L.ForEach(x => p.StandardInput.WriteLine(x));
            p.StandardInput.WriteLine("exit");
            return p.StandardOutput.ReadToEnd().Split('\n');
        }

        public virtual void CloseSelf()
        {
            Stop();
            _CTS1.Cancel();
            _NotifyIcon.Dispose();
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
                Console.WriteLine(s);
                File.Move(s, $@"D:\Converter_Backup\{DateTime.Now:yyyy-MM-dd}\{folderName}\{Path.GetFileNameWithoutExtension(s)}.{extension}");
            }
            if (SF.Visibility == Visibility.Visible)
                SF.Btn_StopConverter_Click(null, null);
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
        public SmallForm SF { get; set; }
        public string IP1 { get { string A = ""; Dispatcher.Invoke(new Action(() => { A = MainWindow.Txt_InputPath1.Text; })); return A; } }
        public string IP2 { get { string A = ""; Dispatcher.Invoke(new Action(() => { A = MainWindow.Txt_InputPath2.Text; })); return A; } }
        public string IP3 { get { string A = ""; Dispatcher.Invoke(new Action(() => { A = MainWindow.Txt_InputPath3.Text; })); return A; } }
        public string OP { get { string A = ""; Dispatcher.Invoke(new Action(() => { A = MainWindow.Txt_OutputPath.Text; })); return A; } }
        public int X { get { int xPoint = 0; Dispatcher.Invoke(new Action(() => { xPoint = Convert.ToInt32(MainWindow.Txt_X.Text.Trim()); })); return xPoint; } }
        public int Y { get { int yPoint = 0; Dispatcher.Invoke(new Action(() => { yPoint = Convert.ToInt32(MainWindow.Txt_Y.Text.Trim()); })); return yPoint; } }
        public bool _AutoStart { get { bool B = false; Dispatcher.Invoke(new Action(() => { B = (bool)MainWindow.Tgl_AutoStart.IsChecked; })); return B; } }
        public bool _IsStat { get { bool B = false; Dispatcher.Invoke(new Action(() => { B = (bool)MainWindow.Rdo_Stat.IsChecked; })); return B; } }  //true > Stat, false > Batch
        public bool _OPD1 { get { bool B = false; Dispatcher.Invoke(new Action(() => { B = (bool)MainWindow.Tgl_OPD1.IsChecked; })); return B; } }
        public bool _OPD2 { get { bool B = false; Dispatcher.Invoke(new Action(() => { B = (bool)MainWindow.Tgl_OPD2.IsChecked; })); return B; } }
        public bool _OPD3 { get { bool B = false; Dispatcher.Invoke(new Action(() => { B = (bool)MainWindow.Tgl_OPD3.IsChecked; })); return B; } }
        public bool _OPD4 { get { bool B = false; Dispatcher.Invoke(new Action(() => { B = (bool)MainWindow.Tgl_OPD4.IsChecked; })); return B; } }
        private MainWindowViewModel _MainWindowVM { get => MainWindowFacotry.GenerateMainWindowViewModel(); }
        bool isMainWindow = true;  //true > mainwindow, false > smallform

        SolidColorBrush Red = new SolidColorBrush((Color)Color.FromRgb(255, 82, 85));
        SolidColorBrush White = new SolidColorBrush((Color)Color.FromRgb(255, 255, 255));

        public WindowsDo()
        {
            _Settings = SettingsFactory.GenerateSettingsControl();
            _SettingsModel = SettingsFactory.GenerateSettingsModel();
            _MsgBVM = MsgBFactory.GenerateMsgBViewModel();
        }

        public void ProcessAction()
        {
            try
            {
                string BackupPath = $@"{FunctionCollections.FileBackupPath}\{DateTime.Now:yyyy-MM-dd}";
                MainWindow.Txtb_Success.Text = $"{Directory.GetFiles($@"{BackupPath}\Success").Length}";
                MainWindow.Txtb_Fail.Text = $"{Directory.GetFiles($@"{BackupPath}\Fail").Length}";
                MainWindow.Txt_InputPath1.Text = _SettingsModel.InputPath1;
                MainWindow.Txt_InputPath2.Text = _SettingsModel.InputPath2;
                MainWindow.Txt_InputPath3.Text = _SettingsModel.InputPath3;
                MainWindow.Txt_OutputPath.Text = _SettingsModel.OutputPath;
                MainWindow.Tgl_AutoStart.IsChecked = _SettingsModel.EN_AutoStart;
                MainWindow.Txt_X.Text = Properties.Settings.Default.X.ToString();
                MainWindow.Txt_Y.Text = Properties.Settings.Default.Y.ToString();
                MainWindow.Btn_Stop.IsEnabled = false;
                MainWindow.Tgl_OPD1.IsChecked = false;
                MainWindow.Tgl_OPD2.IsChecked = false;
                MainWindow.Tgl_OPD3.IsChecked = false;
                MainWindow.Tgl_OPD4.IsChecked = false;
                MainWindow.Rdo_Stat.IsChecked = false;
                if (_SettingsModel.StatOrBatch == "S") MainWindow.Rdo_Stat.IsChecked = true; else MainWindow.Rdo_Batch.IsChecked = true;
                if (_SettingsModel.EN_WindowMinimumWhenOpen)
                    ChangeWindow();
            }
            catch (Exception a)
            {
                _MsgBVM.Show(a.ToString(), "錯誤", PackIconKind.Error, KindColors.Error);
                Log.Write(a.ToString());
            }
        }  //佈署控鍵

        public async void UI_Refresh(CancellationTokenSource cts)
        {
            while (!cts.IsCancellationRequested)
            {
                await Task.Delay(500);
                try
                {
                    //string FileVersion = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion.ToString();  //版本
                    SF.SetFormLocation(Properties.Settings.Default.X, Properties.Settings.Default.Y);
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
                    _MainWindowVM.StatVisibility= _SettingsModel.EN_StatOrBatch ? Visibility.Visible : Visibility.Hidden;
                    _MainWindowVM.BatchVisibility= _SettingsModel.EN_StatOrBatch ? Visibility.Visible : Visibility.Hidden;
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

        private void UI_LayoutSet(UILayout UI)
        {
            MainWindow.Txtb_Title.Text = UI.Title;
            MainWindow.Txtb_InputPath1.Text = UI.IP1Title;
            MainWindow.Txtb_InputPath2.Text = UI.IP2Title;
            MainWindow.Txtb_InputPath3.Text = UI.IP3Title;
            MainWindow.Txt_OPD1.Text = UI.OPDToogle1;
            MainWindow.Txt_OPD2.Text = UI.OPDToogle2;
            MainWindow.Txt_OPD3.Text = UI.OPDToogle3;
            MainWindow.Txt_OPD4.Text = UI.OPDToogle4;
            MainWindow.Btn_InputPath1.IsEnabled = UI.IP1Enabled;
            MainWindow.Btn_InputPath2.IsEnabled = UI.IP2Enabled;
            MainWindow.Btn_InputPath3.IsEnabled = UI.IP3Enabled;
            MainWindow.Btn_UD.Visibility = UI.UDVisibility;
            MainWindow.Tgl_OPD1.Visibility = UI.OPD1Visibility;
            MainWindow.Tgl_OPD2.Visibility = UI.OPD2Visibility;
            MainWindow.Tgl_OPD3.Visibility = UI.OPD3Visibility;
            MainWindow.Tgl_OPD4.Visibility = UI.OPD4Visibility;
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

        //視窗顯示控制
        public void AllWindowShowOrHide(bool? b1, bool? b2, bool? b3)
        {
            if (b1 != null)
                MainWindow.Visibility = (bool)b1 ? Visibility.Visible : Visibility.Hidden;
            if (b2 != null)
                SF.Visibility = (bool)b2 ? Visibility.Visible : Visibility.Hidden;
            if (b3 != null)
            {
                //AS.Visibility = (bool)b3 ? Visibility.Visible : Visibility.Hidden;
            }
        }

        //工具列圖示雙擊
        public void NotifyIconDBClick()
        {
            if (isMainWindow)
            {
                MainWindow.Visibility = Visibility.Visible;
                MainWindow.Activate();
            }
            else
            {
                SF = SF ?? new SmallForm(MainWindow) { Owner = MainWindow };
                SF.Visibility = Visibility.Visible;
                SF.Visibility = Visibility.Visible;
                SF.Topmost = true;
            }
        }

        public void ChangeWindow()
        {
            if (isMainWindow)
            {
                SF = SF ?? new SmallForm(MainWindow) { Owner = MainWindow };
                SF.Initialize();
                SF.ChangeLayout();
                SF.Visibility = Visibility.Visible;
                SF.Topmost = true;
                MainWindow.Visibility = Visibility.Hidden;
                isMainWindow = false;
            }
            else
            {
                SF.Topmost = false;
                SF.Visibility = Visibility.Hidden;
                MainWindow.Visibility = Visibility.Visible;
                MainWindow.Activate();
                isMainWindow = true;
            }
        }

        //MainWindow控建Enabled切換
        public void SwitchControlEnabled(bool b)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.Btn_InputPath1.IsEnabled = b;
                MainWindow.Btn_InputPath2.IsEnabled = b;
                MainWindow.Btn_InputPath3.IsEnabled = b;
                MainWindow.Btn_OutputPath.IsEnabled = b;
                MainWindow.Tgl_AutoStart.IsEnabled = b;
                MainWindow.Rdo_Stat.IsEnabled = b;
                MainWindow.Rdo_Batch.IsEnabled = b;
                MainWindow.Btn_OPD.IsEnabled = b;
                MainWindow.Btn_UD.IsEnabled = b;
                MainWindow.Btn_Stop.IsEnabled = !b;
                MainWindow.Btn_Save.IsEnabled = b;
                MainWindow.Bod_X.IsEnabled = b;
                MainWindow.Bod_Y.IsEnabled = b;
                MainWindow.Tgl_OPD1.IsEnabled = b;
                MainWindow.Tgl_OPD2.IsEnabled = b;
                MainWindow.Tgl_OPD3.IsEnabled = b;
                MainWindow.Tgl_OPD4.IsEnabled = b;
            }));
        }

        //OPD或UD切換
        public void SwitchConverterButtonColor(bool isOPD)
        {
            if (isOPD)
            {
                MainWindow.Btn_OPD.Background = Red;
                MainWindow.Btn_UD.Opacity = 0.2;
            }
            else
            {
                MainWindow.Btn_UD.Background = Red;
                MainWindow.Btn_OPD.Opacity = 0.2;
            }
        }

        public void Stop_Control()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                SF.Stop();
                MainWindow.Btn_OPD.Opacity = 1;
                MainWindow.Btn_UD.Opacity = 1;
                MainWindow.Btn_OPD.Background = White;
                MainWindow.Btn_UD.Background = White;
            }));
        }

        public void ShowAdvancedSettings()
        {
            try
            {
                AS = AdvancedSettingsFactory.GenerateAdvancedSettings();
                AS.SetMainWindow(MainWindow);
                AS.Window_Loaded(null,null);
                AS.ShowDialog();
                AS = null;
            }
            catch (Exception ex)
            {
                _MsgBVM.Show(ex.ToString(), "錯誤", PackIconKind.Error, KindColors.Error);
            }
        }

        public void ProgressBoxClear()
        {
            MainWindow.Txt_ProgressBox.Clear();
            SF.ProgressBoxClear();
        }

        public void ProgressBoxAdd(string Result)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.Txt_ProgressBox.AppendText($"{Result}\n");
                MainWindow.Txt_ProgressBox.ScrollToEnd();
                SF.ProgressBoxAdd(Result);
            }));
        }

        public void SuccessCountAdd()
        {
            Dispatcher.InvokeAsync(new Action(() => { MainWindow.Txtb_Success.Text = (Convert.ToInt32(MainWindow.Txtb_Success.Text) + 1).ToString(); }));
        }

        public void FailCountAdd()
        {
            Dispatcher.InvokeAsync(new Action(() => { MainWindow.Txtb_Fail.Text = (Convert.ToInt32(MainWindow.Txtb_Fail.Text) + 1).ToString(); }));
        }

        public void AutoStart()
        {
            if (_SettingsModel.EN_AutoStart)
                _MainWindowVM.OPDFunc();
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
