using FCP.Core;
using System;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using FCP.src.Factory;
using System.Windows.Input;
using FCP.MVVM.Models;
using FCP.src.Enum;
using FCP.src.Factory.ViewModel;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;
using System.Windows.Forms;
using FCP.MVVM.Control;
using Helper;
using System.Diagnostics;
using System.IO;
using FCP.src.Factory.Models;
using FCP.src;
using System.Windows.Threading;

namespace FCP.MVVM.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public bool IsOPD { get; set; }
        public string SuccessPath { get; set; }
        public string FailPath { get; set; }
        public readonly string FileBackupPath = @"D:\Converter_Backup";
        public ICommand ShowAdvancedSettings { get; set; }
        public ICommand OPD { get; set; }
        public ICommand UD { get; set; }
        public ICommand Stop { get; set; }
        public ICommand Save { get; set; }
        public ICommand Close { get; set; }
        public ICommand MinimumWindow { get; set; }
        public ICommand Closing { get; set; }
        public ICommand Loaded { get; set; }
        public ICommand SwitchWindow { get; set; }
        public ICommand Activate { get; set; }
        public ICommand DragMove { get; set; }
        public ICommand SelectFolder { get; set; }
        public ICommand ClearPath { get; set; }
        public ICommand ClearLog { get; set; }
        public ICommand OpenLog { get; set; }
        public ICommand ClearProgressBoxContent { get; set; }
        public Action ActivateWindow { get; set; }
        public Action DragMoveWindow { get; set; }
        private FunctionCollections _Format { get; set; }
        private SettingsModel _SettingsModel { get; set; }
        private Settings _Settings { get; set; }
        private MsgBViewModel _MsgBVM { get; set; }
        private MainWindowModel _Model;

        public MainWindowViewModel()
        {
            _SettingsModel = SettingsFactory.GenerateSettingsModel();
            _Settings = SettingsFactory.GenerateSettingsControl();
            _MsgBVM = MsgBFactory.GenerateMsgBViewModel();
            ShowAdvancedSettings = new RelayCommand(ShowAdvancedSettingsFunc, CanStartConverterOrShowAdvancedSettings);
            _Model = new MainWindowModel();
            Loaded = new RelayCommand(() => LoadedFuncAsync());
            Close = new RelayCommand(() => Environment.Exit(0));
            MinimumWindow = new RelayCommand(() => Visibility = Visibility.Hidden);
            Closing = new RelayCommand(() => Disconnect小港醫院NetDiskFunc());
            OPD = new RelayCommand(OPDFunc, CanStartConverterOrShowAdvancedSettings);
            UD = new RelayCommand(UDFunc, CanStartConverterOrShowAdvancedSettings);
            Stop = new RelayCommand(StopFunc);
            Save = new RelayCommand(SaveFunc, CanStartConverterOrShowAdvancedSettings);
            SwitchWindow = new RelayCommand(SwitchWindowFunc, CanStartConverterOrShowAdvancedSettings);
            Activate = new RelayCommand(() => ActivateWindow());
            DragMove = new RelayCommand(() => DragMoveWindow());
            SelectFolder = new ObjectRelayCommand(o => SelectFolderFunc((string)o));
            ClearPath = new ObjectRelayCommand(o => ClearPathFunc((string)o), o => CanStartConverterOrShowAdvancedSettings());
            OpenLog = new RelayCommand(() => OpenLogFunc());
            ClearProgressBoxContent = new RelayCommand(() => ClearProgressBoxContentFunc());
        }

        private void ClearProgressBoxContentFunc()
        {
            ProgressBoxContent = string.Empty;
            var vm = SimpleWindowFactory.GenerateSimpleWindowViewModel();
            vm.Log = string.Empty;
            vm = null;
        }

        private void OpenLogFunc()
        {
            try
            {
                Process.Start($@"{Environment.CurrentDirectory}\Log\{DateTime.Now:yyyy-MM-dd}\Error_Log.txt");
            }
            catch (Exception a)
            {
                FCP.src.Message.Show(a.ToString(), "錯誤", PackIconKind.Error, KindColors.Error);
            }
        }

        public Visibility Visibility
        {
            get => _Model.Visibility;
            set => _Model.Visibility = value;
        }

        public string WindowTitle
        {
            get => _Model.WindowTitle;
            set => _Model.WindowTitle = value;
        }

        public string InputPath1Title
        {
            get => _Model.InputPath1Title;
            set => _Model.InputPath1Title = value;
        }

        public string InputPath2Title
        {
            get => _Model.InputPath2Title;
            set => _Model.InputPath2Title = value;
        }

        public string InputPath3Title
        {
            get => _Model.InputPath3Title;
            set => _Model.InputPath3Title = value;
        }
        public string InputPath1
        {
            get => _Model.InputPath1;
            set => _Model.InputPath1 = value;
        }

        public string InputPath2
        {
            get => _Model.InputPath2;
            set => _Model.InputPath2 = value;
        }

        public string InputPath3
        {
            get => _Model.InputPath3;
            set => _Model.InputPath3 = value;
        }

        public bool InputPath1Enabled
        {
            get => _Model.InputPath1Enabled;
            set => _Model.InputPath1Enabled = value;
        }

        public bool InputPath2Enabled
        {
            get => _Model.InputPath2Enabled;
            set => _Model.InputPath2Enabled = value;
        }

        public bool InputPath3Enabled
        {
            get => _Model.InputPath3Enabled;
            set => _Model.InputPath3Enabled = value;
        }

        public string OutputPathTitle
        {
            get => _Model.OutputPathTitle;
            set => _Model.OutputPathTitle = value;
        }

        public string OutputPath
        {
            get => _Model.OutputPath;
            set => _Model.OutputPath = value;
        }

        public bool OutputPathEnabled
        {
            get => _Model.OutputPathEnabled;
            set => _Model.OutputPathEnabled = value;
        }

        public Visibility UDButtonVisibility
        {
            get => _Model.UDButtonVisibility;
            set => _Model.UDButtonVisibility = value;
        }

        public string OPDToogle1
        {
            get => _Model.OPDToogle1;
            set => _Model.OPDToogle1 = value;
        }

        public string OPDToogle2
        {
            get => _Model.OPDToogle2;
            set => _Model.OPDToogle2 = value;
        }

        public string OPDToogle3
        {
            get => _Model.OPDToogle3;
            set => _Model.OPDToogle3 = value;
        }

        public string OPDToogle4
        {
            get => _Model.OPDToogle4;
            set => _Model.OPDToogle4 = value;
        }

        public bool OPDToogle1Checked
        {
            get => _Model.OPDToogle1Checked;
            set => _Model.OPDToogle1Checked = value;
        }
        public bool OPDToogle2Checked
        {
            get => _Model.OPDToogle2Checked;
            set => _Model.OPDToogle2Checked = value;
        }

        public bool OPDToogle3Checked
        {
            get => _Model.OPDToogle3Checked;
            set => _Model.OPDToogle3Checked = value;
        }

        public bool OPDToogle4Checked
        {
            get => _Model.OPDToogle4Checked;
            set => _Model.OPDToogle4Checked = value;
        }

        public bool OPDToogle1Enabled
        {
            get => _Model.OPDToogle1Enabled;
            set => _Model.OPDToogle1Enabled = value;
        }

        public bool OPDToogle2Enabled
        {
            get => _Model.OPDToogle2Enabled;
            set => _Model.OPDToogle2Enabled = value;
        }

        public bool OPDToogle3Enabled
        {
            get => _Model.OPDToogle3Enabled;
            set => _Model.OPDToogle3Enabled = value;
        }

        public bool OPDToogle4Enabled
        {
            get => _Model.OPDToogle4Enabled;
            set => _Model.OPDToogle4Enabled = value;
        }

        public Visibility OPDToogle1Visibility
        {
            get => _Model.OPDToogle1Visibility;
            set => _Model.OPDToogle1Visibility = value;
        }

        public Visibility OPDToogle2Visibility
        {
            get => _Model.OPDToogle2Visibility;
            set => _Model.OPDToogle2Visibility = value;
        }

        public Visibility OPDToogle3Visibility
        {
            get => _Model.OPDToogle3Visibility;
            set => _Model.OPDToogle3Visibility = value;
        }

        public Visibility OPDToogle4Visibility
        {
            get => _Model.OPDToogle4Visibility;
            set => _Model.OPDToogle4Visibility = value;
        }

        public bool OPDEnabled
        {
            get => _Model.OPDEnabled;
            set => _Model.OPDEnabled = value;
        }

        public bool UDEnabled
        {
            get => _Model.UDEnabled;
            set => _Model.UDEnabled = value;
        }

        public bool StopEnabled
        {
            get => _Model.StopEnabled;
            set => _Model.StopEnabled = value;
        }

        public bool SaveEnabled
        {
            get => _Model.SaveEnabled;
            set => _Model.SaveEnabled = value;
        }

        public bool StatChecked
        {
            get => _Model.StatChecked;
            set => _Model.StatChecked = value;
        }

        public bool BatchChecked
        {
            get => _Model.BatchChecked;
            set => _Model.BatchChecked = value;
        }

        public bool StatEnabled
        {
            get => _Model.StatEnabled;
            set => _Model.StatEnabled = value;
        }

        public bool BatchEnabled
        {
            get => _Model.BatchEnabled;
            set => _Model.BatchEnabled = value;
        }

        public Visibility StatVisibility
        {
            get => _Model.StatVisibility;
            set => _Model.StatVisibility = value;
        }

        public Visibility BatchVisibility
        {
            get => _Model.BatchVisibility;
            set => _Model.BatchVisibility = value;
        }

        public bool IsAutoStartChecked
        {
            get => _Model.IsAutoStartChecked;
            set => _Model.IsAutoStartChecked = value;
        }

        public bool IsSplitEachMealChecked
        {
            get => Properties.Settings.Default.IsSplitEachMeal;
            set
            {
                Properties.Settings.Default.IsSplitEachMeal = value;
                Properties.Settings.Default.Save();
            }
        }
        public string DoseType
        {
            get => _Model.DoseType;
            set => _Model.DoseType = value;
        }

        public string SuccessCount
        {
            get => _Model.SuccessCount;
            set => _Model.SuccessCount = value;
        }

        public string FailCount
        {
            get => _Model.FailCount;
            set => _Model.FailCount = value;
        }

        public string WindowX
        {
            get => _Model.WindowX;
            set => _Model.WindowX = value;
        }

        public string WindowY
        {
            get => _Model.WindowY;
            set => _Model.WindowY = value;
        }

        public bool WindowXEnabled
        {
            get => _Model.WindowXEnabled;
            set => _Model.WindowXEnabled = value;
        }

        public bool WindowYEnabled
        {
            get => _Model.WindowYEnabled;
            set => _Model.WindowYEnabled = value;
        }

        public Visibility WindowXVisibility
        {
            get => _Model.WindowXVisibility;
            set => _Model.WindowXVisibility = value;
        }

        public Visibility WindowYVisibility
        {
            get => _Model.WindowYVisibility;
            set => _Model.WindowYVisibility = value;
        }

        public Visibility MinimumAndCloseVisibility
        {
            get => _Model.MinimumAndCloseVisibility;
            set => _Model.MinimumAndCloseVisibility = value;
        }

        public float OPDOpacity
        {
            get => _Model.OPDOpacity;
            set => _Model.OPDOpacity = value;
        }

        public float UDOpacity
        {
            get => _Model.UDOpacity;
            set => _Model.UDOpacity = value;
        }

        public SolidColorBrush OPDBackground
        {
            get => _Model.OPDBacground;
            set => _Model.OPDBacground = value;
        }

        public SolidColorBrush UDBackground
        {
            get => _Model.UDBackground;
            set => _Model.UDBackground = value;
        }

        public string ProgressBoxContent
        {
            get => _Model.ProgressBoxContent;
            set => _Model.ProgressBoxContent = value;
        }
        public void OPDFunc()
        {
            if ((_Model.OPDToogle1Checked | _Model.OPDToogle2Checked | _Model.OPDToogle3Checked | _Model.OPDToogle4Checked) == false)
            {
                FCP.src.Message.Show("沒有勾選任一個轉檔位置", "位置未勾選", PackIconKind.Error, KindColors.Error);
                return;
            }
            IsOPD = true;
            _Format.ConvertPrepare(IsOPD);
        }

        public void UDFunc()
        {
            IsOPD = false;
            _Format.ConvertPrepare(IsOPD);
        }

        public void StopFunc()
        {
            _Format.Stop();
            RefreshUIPropertyServices.SwitchMainWindowControlState(true);
            RefreshUIPropertyServices.SwitchUIStateForStop();
        }

        public void SaveFunc()
        {
            Properties.Settings.Default.X = Convert.ToInt32(WindowX);
            Properties.Settings.Default.Y = Convert.ToInt32(WindowY);
            Properties.Settings.Default.Save();

            _Settings.SaveMainWidow(InputPath1, InputPath2, InputPath3, OutputPath, IsAutoStartChecked, StatChecked ? "S" : "B");
            AddLog("儲存成功");
        }

        public void AddLog(string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ProgressBoxContent);
            sb.Append($"{message}\n");
            ProgressBoxContent = sb.ToString();
            sb = null;
        }

        public void SwitchWindowFunc()
        {
            Visibility = Visibility.Hidden;
            var vm = SimpleWindowFactory.GenerateSimpleWindowViewModel();
            vm.Visibility = Visibility.Visible;
        }

        public void NotifyIconDBClick()
        {
            var vm = SimpleWindowFactory.GenerateSimpleWindowViewModel();
            if (vm.Enabled)
            {
                vm.Visibility = Visibility.Visible;
                SimpleWindowFactory.GenerateSimpleWindowViewModel().ActivateWindow();
            }
            else
            {
                Visibility = Visibility.Visible;
                ActivateWindow();
            }
        }

        public void JudgeCurrentFormat()
        {
            _Format = FormatFactory.GenerateFormat(_SettingsModel.Mode);
            _Format.Init();
        }

        private async void LoadedFuncAsync()
        {
            Log.Path = $@"{Environment.CurrentDirectory}\Log";
            CheckProgramStart();
            CheckFileBackupPath();
            RefreshUIPropertyServices.SwitchMainWindowControlState(true);
            NotifyIconHelper.Init(Properties.Resources.FCP, "轉檔");
            NotifyIconHelper.DoubleClickAction += NotifyIconDBClick;
            FunctionCollections.NotifyIcon = NotifyIconHelper.NotifyIcon;
            JudgeCurrentFormat();
            Init();
            if (IsAutoStartChecked)
            {
                await Task.Delay(1000);
                OPDFunc();
            }
        }

        //檢查程式是否已開啟
        private void CheckProgramStart()
        {
            if (Process.GetProcessesByName("FCP").Length >= 2)
            {
                FCP.src.Message.Show("程式已開啟，請確認工具列", "重複開啟", PackIconKind.Error, KindColors.Error);
                Log.Write("程式已開啟，請確認工具列");
                Environment.Exit(0);
            }
        }

        //檢查備份資料夾是否存在
        private void CheckFileBackupPath()
        {
            string BackupPath = $@"{FileBackupPath}\{DateTime.Now:yyyy-MM-dd}";
            if (!Directory.Exists($@"{BackupPath}\Success")) Directory.CreateDirectory($@"{BackupPath}\Success");
            if (!Directory.Exists($@"{BackupPath}\Fail")) Directory.CreateDirectory($@"{BackupPath}\Fail");
            SuccessPath = $@"{BackupPath}\Success";
            FailPath = $@"{BackupPath}\Fail";
        }

        private void ShowAdvancedSettingsFunc()
        {
            AdvancedSettingsFactory.ClearAllViewModel();
            var window = AdvancedSettingsFactory.GenerateAdvancedSettings();
            window.ShowDialog();
            window = null;
        }

        private void Disconnect小港醫院NetDiskFunc()
        {
            switch (_SettingsModel.Mode)
            {
                case eFormat.小港醫院TOC:
                    _Format.Stop();
                    break;
            }
        }

        private void SelectFolderFunc(string name)
        {
            string path = string.Empty;
            using (FolderBrowserDialog folder = new FolderBrowserDialog())
            {
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    path = folder.SelectedPath;
                }
            }
            switch (name)
            {
                case "IP1":
                    InputPath1 = path;
                    break;
                case "IP2":
                    InputPath2 = path;
                    break;
                case "IP3":
                    InputPath3 = path;
                    break;
                case "OP":
                    OutputPath = path;
                    break;
                default:
                    break;
            }
        }

        private void ClearPathFunc(string name)
        {
            switch (name)
            {
                case "IP1":
                    InputPath1 = string.Empty;
                    break;
                case "IP2":
                    InputPath2 = string.Empty;
                    break;
                case "IP3":
                    InputPath3 = string.Empty;
                    break;
                default:
                    break;
            }
        }

        private void Init()
        {
            string BackupPath = $@"{FileBackupPath}\{DateTime.Now:yyyy-MM-dd}";
            SuccessCount = $"{Directory.GetFiles($@"{BackupPath}\Success").Length}";
            FailCount = $"{Directory.GetFiles($@"{BackupPath}\Fail").Length}";
            InputPath1 = _SettingsModel.InputPath1;
            InputPath2 = _SettingsModel.InputPath2;
            InputPath3 = _SettingsModel.InputPath3;
            OutputPath = _SettingsModel.OutputPath;
            IsAutoStartChecked = _SettingsModel.EN_AutoStart;
            WindowX = Properties.Settings.Default.X.ToString();
            WindowY = Properties.Settings.Default.Y.ToString();
            StopEnabled = false;
            StatChecked = _SettingsModel.StatOrBatch == "S";
            BatchChecked = _SettingsModel.StatOrBatch == "B";
        }

        private bool CanStartConverterOrShowAdvancedSettings()
        {
            return !StopEnabled;
        }
    }
}
