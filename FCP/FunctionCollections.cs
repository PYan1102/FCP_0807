using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Data.SqlClient;
using FCP.MVVM.ViewModels.MainWindow;
using FCP.MVVM.Factory.ViewModels;
using FCP.MVVM.Control;
using FCP.MVVM.Models;
using FCP.MVVM.Factory;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.ViewModels.GetConvertFile;

namespace FCP
{
    public abstract class FunctionCollections
    {
        public string SQLInfo = "server=.;database=OnCube;user id=sa;password=jvm5822511";
        public string IP1, IP2, IP3, OP = "";
        public string PackedType;
        public const string FileBackupPath = @"D:\Converter_Backup";
        System.Windows.Forms.NotifyIcon NF = new System.Windows.Forms.NotifyIcon();
        System.Windows.Forms.ContextMenuStrip CM = new System.Windows.Forms.ContextMenuStrip();
        public Log Log = new Log();
        public SettingsModel SettingsModel { get; set; }
        private Settings _Settings { get; set; }
        public SmallForm SF;
        public AdvancedSettings AS;
        MsgB Msg;
        MergeFiles Merge = new MergeFiles();
        CancellationTokenSource cts;
        CancellationTokenSource cts1;
        List<string> IPList = new List<string>();
        public WindowsDo WD;
        private static string SuccessPath, FailPath;
        protected internal string FilePath, InputPath, OutputPath, CurrentSeconds;
        public int MethodID;
        public IFindNeedToConvertFile IFindFile { get; set; }
        public MainWindow MainWindow { get; set; }
        private ConvertFileInformtaionModel _ConvertFileInformation { get; set; }
        protected internal DepartmentEnum CurrentDepartment { get; set; }
        private string _OPD { get; set; }
        private string _Powder { get; set; }
        private string _UDBatch { get; set; }
        private string _UDStat { get; set; }
        private string _Other { get; set; }
        private string _Care { get; set; }

        public FunctionCollections()
        {
            _Settings = SettingsFactory.GenerateSettingsControl();
            SettingsModel = SettingsFactory.GenerateSettingsModels();
            _ConvertFileInformation = ConvertInfoactory.GenerateConvertFileInformation();
        }

        public void SetWindow(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        public virtual void SetFindFileMode(FindFileModeEnum mode)
        {
            switch(mode)
            {
                case FindFileModeEnum.根據檔名開頭:
                    IFindFile = new FindFileAccordingToStartWith();
                    break;
                case FindFileModeEnum.根據輸入路徑:
                    IFindFile = new FindFileAccordingToInputPath();
                    break;
                case FindFileModeEnum.根據檔案內容:
                    IFindFile = new FindFileAccordingToFileContent();
                    break;
            }
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
                Log.Check();
                SF = new SmallForm(MainWindow) { Owner = MainWindow };
                AS = new AdvancedSettings(MainWindow, Log) { Owner = MainWindow };
                Msg = new MsgB() { Owner = MainWindow };
                WD = new WindowsDo() { MainWindow = MainWindow, Msg = Msg, Log = Log, SF = SF, AS = AS };
                CheckProgramStart();
                CheckFileBackupPath();
                CreateIcon();
                WD.ProcessAction();
                NF.Text = (bool)MainWindow.Tgl_AutoStart.IsChecked ? "轉檔程式：轉檔進行中" : "轉檔程式：停止";
                cts1 = new CancellationTokenSource();
                MainWindow.Dispatcher.InvokeAsync(new Action(() => WD.UI_Refresh(cts1)));
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
                Msg.Show("程式已開啟，請確認工具列", "重複開啟", "Error", Msg.Color.Error);
                Log.Write("程式已開啟，請確認工具列");
                Environment.Exit(0);
            }
        }

        //檢查備份資料夾是否存在
        public static void CheckFileBackupPath()
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

        private void CreateIcon()
        {
            CM = new System.Windows.Forms.ContextMenuStrip();
            CM.Items.Add("離開");
            CM.ItemClicked += CMS_ItemClicked;
            NF.Icon = new System.Drawing.Icon(new Uri("FCP.ico", UriKind.Relative).ToString());
            NF.Visible = true;
            NF.Text = "轉檔狀態：停止";
            NF.ContextMenuStrip = CM;
            NF.DoubleClick += IconDBClick;
        }

        private void CMS_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.ToString().Equals("離開"))
                Environment.Exit(0);
        }

        public void IconDBClick(object sender, EventArgs e)
        {
            WD.IconDBClick();
        }

        public virtual void AdvancedSettingsShow()
        {
            WD.AdvancedSettingsShow();
        }

        public virtual void Stop()
        {
            if (cts != null) cts.Cancel();
            WD.Stop_Control();
            WD.SwitchControlEnabled(true);
            NF.Text = "停止";
        }

        public virtual void Save()
        {
            try
            {
                Properties.Settings.Default.X = WD.X;
                Properties.Settings.Default.Y = WD.Y;
                Properties.Settings.Default.Save();

                _Settings.SaveMainWidow(WD.IP1, WD.IP2, WD.IP3, WD.OP, WD._AutoStart, WD._IsStat ? "S" : "B");
                ProgressBoxAdd("儲存成功");
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
            if ((WD.IP1 + WD.IP2 + WD.IP3).Trim() == "")
            {
                if (cts != null) Stop();
                Msg.Show("來源路徑為空白", "路徑空白", "Error", Msg.Color.Error);
                return;
            }
            if (WD.OP.Trim() == "")
            {
                if (cts != null) Stop();
                Msg.Show("輸出路徑為空白", "路徑空白", "Error", Msg.Color.Error);
                return;
            }
            WD.Start_Control(isOPD);
            WD.SwitchControlEnabled(false);
            NF.Text = "開始";
            IPList.Clear();
            IPList.Add(WD.IP1);
            IPList.Add(WD.IP2);
            WD.AllWindowShowOrHide(null, null, false);
            cts = null;
            cts = new CancellationTokenSource();
            IFindFile.ResetDictionary();
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
        public void SetOtherRule(string rule)
        {
            _Other = rule;
        }
        public void SetCareRule(string rule)
        {
            _Care = rule;
        }

        public void ReSet(bool isOPD)
        {
            if (isOPD)
            {
                if (WD._OPD1)
                    IFindFile.SetOPD(_OPD);
                if (WD._OPD2)
                    IFindFile.SetPowder(_Powder);
                if (WD._OPD3)
                    IFindFile.SetOther(_Other);
                if (WD._OPD4)
                    IFindFile.SetCare(_Care);
            }
            else
            {
                if (WD._IsStat)
                    IFindFile.SetUDStat(_UDStat);
                else
                    IFindFile.SetUDBatch(_UDBatch);

            }
            IFindFile.Reset(cts, IPList);
        }

        public virtual void GetFileAsync()
        {
            Task.Run(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    Clear();
                    Task<FileInformation> fileInformation = Task.Run(() => IFindFile.GetFilePathTaskAsync());
                    InputPath = fileInformation.Result.InputPath;
                    FilePath = fileInformation.Result.FilePath;
                    CurrentDepartment = fileInformation.Result.Department;
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
            if (cts == null)
                return;
            Task.Run(async () =>
            {
                //GetFileNameTaskAsync();
                try
                {
                    while (!cts.IsCancellationRequested)
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
            if (cts == null)
                return;
            Task.Run(async () =>
            {
                try
                {
                    while (!cts.IsCancellationRequested)
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
            if (cts == null)
                return;
            Task.Run(async () =>
            {
                try
                {
                    while (!cts.IsCancellationRequested)
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
            this.MethodID = MethodID;
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

        private DepartmentEnum JudgeCurrentDepartment()
        {
            return DepartmentEnum.OPD;
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
                    tip = $"{fileNameWithoutExtension}  轉檔成功";
                    ProgressBoxAdd(tip);
                    NF.ShowBalloonTip(850, "轉檔成功", tip, System.Windows.Forms.ToolTipIcon.None);
                    break;
                case ConvertResult.全數過濾:
                    if (isMoveFile)
                    {
                        File.Move(FilePath, $@"{SuccessPath}\{fileName}.ok");
                        WD.SuccessCountAdd();
                    }
                    if (isReminder)
                    {
                        tip = $"{fileNameWithoutExtension} 全數過濾";
                        ProgressBoxAdd(tip);
                        NF.ShowBalloonTip(850, "全數過濾", tip, System.Windows.Forms.ToolTipIcon.None);
                    }
                    break;
                case ConvertResult.沒有種包頻率:
                    Stop();
                    ProgressBoxAdd(message);
                    NF.ShowBalloonTip(850, $"缺少頻率", $"{Path.GetFileName(FilePath)} OnCube中缺少該檔案 {message} 的種包頻率", System.Windows.Forms.ToolTipIcon.Error);
                    break;
                case ConvertResult.沒有餐包頻率:
                    Stop();
                    ProgressBoxAdd(message);
                    NF.ShowBalloonTip(850, $"缺少頻率", $"{Path.GetFileName(FilePath)} OnCube中缺少該檔案 {message} 的餐包頻率", System.Windows.Forms.ToolTipIcon.Error);
                    break;
                default:
                    if (isMoveFile)
                    {
                        File.Move(FilePath, $@"{FailPath}\{fileName}.fail");
                        WD.FailCountAdd();
                    }
                    ProgressBoxAdd(message);
                    NF.ShowBalloonTip(850, "轉檔錯誤", message, System.Windows.Forms.ToolTipIcon.Error);
                    break;
            }
            //Stop();
        }

        public virtual void ProgressBoxClear()
        {
            WD.ProgressBoxClear();
        }

        private void ProgressBoxAdd(string Result)
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

        public void Query(string Command)
        {
            SqlConnection con = new SqlConnection(SQLInfo);
            con.Open();
            SqlCommand com = new SqlCommand(Command, con);
            com.ExecuteNonQuery();
            com.Dispose();
            con.Close();
        }

        public virtual void CloseSelf()
        {
            Stop();
            cts1.Cancel();
            NF.Dispose();
            CM.Dispose();
            WD = null;
        }

        public string MergeFiles(string In, string FileName, int Start, int Length, string Word)
        {
            Merge.SetValue(In, FileName);
            Merge.Merge(Start, Length, Word);
            return Merge.FileName;
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
        public MsgB Msg { get; set; }
        public Log Log { get; set; }
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
        bool isMainWindow = true;  //true > mainwindow, false > smallform

        SolidColorBrush Red = new SolidColorBrush((Color)Color.FromRgb(255, 82, 85));
        SolidColorBrush White = new SolidColorBrush((Color)Color.FromRgb(255, 255, 255));

        public WindowsDo()
        {
            _Settings = SettingsFactory.GenerateSettingsControl();
            _SettingsModel = SettingsFactory.GenerateSettingsModels();
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
                MainWindow.Txt_OutputPath.Text = _SettingsModel.OutputPath1;
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
                Msg.Show(a.ToString(), "錯誤", "Error", Msg.Color.Error);
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
                    if (!(bool)MainWindow.Btn_Stop.IsEnabled)
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
                        UI_LayoutSet(UI);
                        MainWindow.Txtb_PackType.Text = _SettingsModel.DoseMode.ToString();
                    }
                    MainWindow.Rdo_Stat.Visibility = _SettingsModel.EN_StatOrBatch ? Visibility.Visible : Visibility.Hidden;
                    MainWindow.Rdo_Batch.Visibility = _SettingsModel.EN_StatOrBatch ? Visibility.Visible : Visibility.Hidden;
                    MainWindow.Btn_Close.Visibility = _SettingsModel.EN_ShowControlButton ? Visibility.Visible : Visibility.Hidden;
                    MainWindow.Btn_Minimum.Visibility = _SettingsModel.EN_ShowControlButton ? Visibility.Visible : Visibility.Hidden;
                    MainWindow.Bod_X.Visibility = _SettingsModel.EN_ShowXY ? Visibility.Visible : Visibility.Hidden;
                    MainWindow.Bod_Y.Visibility = _SettingsModel.EN_ShowXY ? Visibility.Visible : Visibility.Hidden;
                    FunctionCollections.CheckFileBackupPath();
                }
                catch (Exception a)
                {
                    Msg.Show(a.ToString(), "錯誤", "Error", Msg.Color.Error);
                    Log.Write(a.ToString());
                }
            }
        }

        private void UI_LayoutSet(UILayout UI)
        {
            MainWindow.Txtb_Title.Text = UI.Title;
            MainWindow.Txtb_InputPath1.Text = UI.IP1s;
            MainWindow.Txtb_InputPath2.Text = UI.IP2s;
            MainWindow.Txtb_InputPath3.Text = UI.IP3s;
            MainWindow.Txt_OPD1.Text = UI.OPD1s;
            MainWindow.Txt_OPD2.Text = UI.OPD2s;
            MainWindow.Txt_OPD3.Text = UI.OPD3s;
            MainWindow.Txt_OPD4.Text = UI.OPD4s;
            MainWindow.Btn_InputPath1.IsEnabled = UI.IP1b;
            MainWindow.Btn_InputPath2.IsEnabled = UI.IP2b;
            MainWindow.Btn_InputPath3.IsEnabled = UI.IP3b;
            MainWindow.Btn_UD.Visibility = UI.UDv;
            MainWindow.Tgl_OPD1.Visibility = UI.OPD1v;
            MainWindow.Tgl_OPD2.Visibility = UI.OPD2v;
            MainWindow.Tgl_OPD3.Visibility = UI.OPD3v;
            MainWindow.Tgl_OPD4.Visibility = UI.OPD4v;
        }

        private void JVServerToOnCube(UILayout UI)
        {
            UI.Title = "Normal > OnCube";
            UI.IP1s = "輸入路徑1";
            UI.IP2s = "輸入路徑2";
            UI.IP3s = "輸入路徑3";
            UI.OPD1s = "門診";
            UI.OPD2s = "";
            UI.OPD3s = "";
            UI.OPD4s = "";
            UI.IP1b = true;
            UI.IP2b = true;
            UI.IP3b = true;
            UI.UDv = Visibility.Hidden;
            UI.OPD1v = Visibility.Visible;
            UI.OPD2v = Visibility.Hidden;
            UI.OPD3v = Visibility.Hidden;
            UI.OPD4v = Visibility.Hidden;
        }

        private void 小港ToOnCube(UILayout UI)
        {
            UI.Title = "小港醫院 > OnCube";
            UI.IP1s = "門   診";
            UI.IP2s = "磨   粉";
            UI.IP3s = "住   院";
            UI.OPD1s = "門診";
            UI.OPD2s = "磨粉";
            UI.OPD3s = "慢籤";
            UI.OPD4s = "急診";
            UI.IP1b = true;
            UI.IP2b = true;
            UI.IP3b = true;
            UI.UDv = Visibility.Visible;
            UI.OPD1v = Visibility.Visible;
            UI.OPD2v = Visibility.Visible;
            UI.OPD3v = Visibility.Visible;
            UI.OPD4v = Visibility.Visible;
        }

        private void 光田ToOnCube(UILayout UI)
        {
            UI.Title = "光田醫院 > OnCube";
            UI.IP1s = "門   診";
            UI.IP2s = "輸入路徑2";
            UI.IP3s = "住   院";
            UI.OPD1s = "門診";
            UI.OPD2s = "";
            UI.OPD3s = "";
            UI.OPD4s = "";
            UI.IP1b = true;
            UI.IP2b = false;
            UI.IP3b = true;
            UI.UDv = Visibility.Visible;
            UI.OPD1v = Visibility.Visible;
            UI.OPD2v = Visibility.Hidden;
            UI.OPD3v = Visibility.Hidden;
            UI.OPD4v = Visibility.Hidden;
        }

        private void 光田ToJVServer(UILayout UI)
        {
            UI.Title = "光田醫院 > JVServer";
            UI.IP1s = "輸入路徑1";
            UI.IP2s = "磨   粉";
            UI.IP3s = "輸入路徑3";
            UI.OPD1s = "";
            UI.OPD2s = "磨粉";
            UI.OPD3s = "";
            UI.OPD4s = "";
            UI.IP1b = false;
            UI.IP2b = true;
            UI.IP3b = false;
            UI.UDv = Visibility.Hidden;
            UI.OPD1v = Visibility.Hidden;
            UI.OPD2v = Visibility.Visible;
            UI.OPD3v = Visibility.Hidden;
            UI.OPD4v = Visibility.Hidden;
        }

        private void 民生ToOnCube(UILayout UI)
        {
            UI.Title = "民生醫院 > OnCube";
            UI.IP1s = "門診養護";
            UI.IP2s = "大寮百合";
            UI.IP3s = "住   院";
            UI.OPD1s = "門診";
            UI.OPD2s = "";
            UI.OPD3s = "養護";
            UI.OPD4s = "大寮";
            UI.IP1b = true;
            UI.IP2b = true;
            UI.IP3b = true;
            UI.UDv = Visibility.Visible;
            UI.OPD1v = Visibility.Visible;
            UI.OPD2v = Visibility.Hidden;
            UI.OPD3v = Visibility.Visible;
            UI.OPD4v = Visibility.Visible;
        }

        private void 義大ToOnCube(UILayout UI)
        {
            UI.Title = "義大醫院 > OnCube";
            UI.IP1s = "輸入路徑1";
            UI.IP2s = "輸入路徑2";
            UI.IP3s = "住   院";
            UI.OPD1s = "";
            UI.OPD2s = "";
            UI.OPD3s = "";
            UI.OPD4s = "";
            UI.IP1b = false;
            UI.IP2b = false;
            UI.IP3b = true;
            UI.UDv = Visibility.Visible;
            UI.OPD1v = Visibility.Hidden;
            UI.OPD2v = Visibility.Hidden;
            UI.OPD3v = Visibility.Hidden;
            UI.OPD4v = Visibility.Hidden;
        }

        private void 長庚磨粉ToJVServer(UILayout UI)
        {
            UI.Title = "長庚磨粉 > JVServer";
            UI.IP1s = "磨粉";
            UI.IP2s = "輸入路徑2";
            UI.IP3s = "輸入路徑3";
            UI.OPD1s = "";
            UI.OPD2s = "磨粉";
            UI.OPD3s = "";
            UI.OPD4s = "";
            UI.IP1b = true;
            UI.IP2b = false;
            UI.IP3b = false;
            UI.UDv = Visibility.Hidden;
            UI.OPD1v = Visibility.Hidden;
            UI.OPD2v = Visibility.Visible;
            UI.OPD3v = Visibility.Hidden;
            UI.OPD4v = Visibility.Hidden;
        }

        private void 長庚醫院ToOnCube(UILayout UI)
        {
            UI.Title = "長庚醫院 > OnCube";
            UI.IP1s = "門診";
            UI.IP2s = "藥來速";
            UI.IP3s = "住院";
            UI.OPD1s = "門診";
            UI.OPD2s = "";
            UI.OPD3s = "";
            UI.OPD4s = "藥來速";
            UI.IP1b = true;
            UI.IP2b = true;
            UI.IP3b = true;
            UI.UDv = Visibility.Visible;
            UI.OPD1v = Visibility.Visible;
            UI.OPD2v = Visibility.Hidden;
            UI.OPD3v = Visibility.Hidden;
            UI.OPD4v = Visibility.Visible;
        }

        private void 仁康醫院ToOnCube(UILayout UI)
        {
            UI.Title = "仁康醫院 > OnCube";
            UI.IP1s = "門診";
            UI.IP2s = "輸入路徑2";
            UI.IP3s = "住院";
            UI.OPD1s = "門診";
            UI.OPD2s = "";
            UI.OPD3s = "養護";
            UI.OPD4s = "";
            UI.IP1b = true;
            UI.IP2b = false;
            UI.IP3b = true;
            UI.UDv = Visibility.Visible;
            UI.OPD1v = Visibility.Visible;
            UI.OPD2v = Visibility.Hidden;
            UI.OPD3v = Visibility.Visible;
            UI.OPD4v = Visibility.Hidden;
        }

        //視窗顯示控制
        public void AllWindowShowOrHide(bool? b1, bool? b2, bool? b3)
        {
            if (b1 != null)
                MainWindow.Visibility = (bool)b1 ? Visibility.Visible : Visibility.Hidden;
            if (b2 != null)
                SF.Visibility = (bool)b2 ? Visibility.Visible : Visibility.Hidden;
            if (b3 != null)
                AS.Visibility = (bool)b3 ? Visibility.Visible : Visibility.Hidden;
        }

        //工具列圖示雙擊
        public void IconDBClick()
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
        public void Start_Control(bool isOPD)
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

        public void AdvancedSettingsShow()
        {
            try
            {
                AS.Window_Loaded(null, null);
                AS.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Msg.Show(ex.ToString(), "錯誤", "Error", Msg.Color.Error);
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
                MainWindow.Btn_OPD_Click(null, null);
        }
    }

    public class UILayout
    {
        public string Title { get; set; }
        public string IP1s { get; set; }
        public string IP2s { get; set; }
        public string IP3s { get; set; }
        public string OPD1s { get; set; }
        public string OPD2s { get; set; }
        public string OPD3s { get; set; }
        public string OPD4s { get; set; }
        public bool IP1b { get; set; }
        public bool IP2b { get; set; }
        public bool IP3b { get; set; }
        public Visibility UDv { get; set; }
        public Visibility OPD1v { get; set; }
        public Visibility OPD2v { get; set; }
        public Visibility OPD3v { get; set; }
        public Visibility OPD4v { get; set; }
    }
}
