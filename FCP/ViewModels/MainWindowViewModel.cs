using FCP.Core;
using System;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using FCP.src.Factory;
using System.Windows.Input;
using FCP.Models;
using FCP.src.Enum;
using FCP.src.Factory.ViewModel;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;
using System.Windows.Forms;
using FCP.Service;
using Helper;
using System.Diagnostics;
using System.IO;
using FCP.src.Factory.Models;
using FCP.src;
using System.Windows.Threading;

namespace FCP.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public bool IsOPD { get; set; }
        public string SuccessPath { get; set; }
        public string FailPath { get; set; }
        public ICommand ShowAdvancedSettings { get; set; }
        public ICommand OPD { get; set; }
        public ICommand UD { get; set; }
        public ICommand Stop { get; set; }
        public ICommand Save { get; set; }
        public ICommand Close { get; set; }
        public ICommand MinimumWindow { get; set; }
        public ICommand Loaded { get; set; }
        public ICommand SwitchWindow { get; set; }
        public ICommand Activate { get; set; }
        public ICommand SelectFolder { get; set; }
        public ICommand ClearPath { get; set; }
        public ICommand ClearLog { get; set; }
        public ICommand OpenLog { get; set; }
        public ICommand ClearProgressBoxContent { get; set; }
        public Action ActivateWindow { get; set; }
        private FunctionCollections _format { get; set; }
        private SettingModel _settingModel { get; set; }
        private Setting _setting { get; set; }
        private MsgBViewModel _msgBVM { get; set; }
        private MainWindowModel _model;

        public MainWindowViewModel()
        {
            _settingModel = SettingFactory.GenerateSettingModel();
            _setting = SettingFactory.GenerateSetting();
            _msgBVM = MsgBFactory.GenerateMsgBViewModel();
            ShowAdvancedSettings = new RelayCommand(ShowAdvancedSettingsFunc, CanStartConverterOrShowAdvancedSettings);
            _model = new MainWindowModel();
            Loaded = new RelayCommand(() => LoadedFuncAsync());
            Close = new RelayCommand(() => Environment.Exit(0));
            MinimumWindow = new RelayCommand(() => Visibility = Visibility.Hidden);
            OPD = new RelayCommand(OPDFunc, CanStartConverterOrShowAdvancedSettings);
            UD = new RelayCommand(UDFunc, CanStartConverterOrShowAdvancedSettings);
            Stop = new RelayCommand(StopFunc);
            Save = new RelayCommand(SaveFunc, CanStartConverterOrShowAdvancedSettings);
            SwitchWindow = new RelayCommand(SwitchWindowFunc, CanStartConverterOrShowAdvancedSettings);
            Activate = new RelayCommand(() => ActivateWindow());
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
            get => _model.Visibility;
            set => _model.Visibility = value;
        }

        public string WindowTitle
        {
            get => _model.WindowTitle;
            set => _model.WindowTitle = value;
        }

        public string InputPath1Title
        {
            get => _model.InputPath1Title;
            set => _model.InputPath1Title = value;
        }

        public string InputPath2Title
        {
            get => _model.InputPath2Title;
            set => _model.InputPath2Title = value;
        }

        public string InputPath3Title
        {
            get => _model.InputPath3Title;
            set => _model.InputPath3Title = value;
        }
        public string InputPath1
        {
            get => _model.InputPath1;
            set => _model.InputPath1 = value;
        }

        public string InputPath2
        {
            get => _model.InputPath2;
            set => _model.InputPath2 = value;
        }

        public string InputPath3
        {
            get => _model.InputPath3;
            set => _model.InputPath3 = value;
        }

        public bool InputPath1Enabled
        {
            get => _model.InputPath1Enabled;
            set => _model.InputPath1Enabled = value;
        }

        public bool InputPath2Enabled
        {
            get => _model.InputPath2Enabled;
            set => _model.InputPath2Enabled = value;
        }

        public bool InputPath3Enabled
        {
            get => _model.InputPath3Enabled;
            set => _model.InputPath3Enabled = value;
        }

        public string OutputPathTitle
        {
            get => _model.OutputPathTitle;
            set => _model.OutputPathTitle = value;
        }

        public string OutputPath
        {
            get => _model.OutputPath;
            set => _model.OutputPath = value;
        }

        public bool OutputPathEnabled
        {
            get => _model.OutputPathEnabled;
            set => _model.OutputPathEnabled = value;
        }

        public Visibility UDButtonVisibility
        {
            get => _model.UDButtonVisibility;
            set => _model.UDButtonVisibility = value;
        }

        public string OPDToogle1
        {
            get => _model.OPDToogle1;
            set => _model.OPDToogle1 = value;
        }

        public string OPDToogle2
        {
            get => _model.OPDToogle2;
            set => _model.OPDToogle2 = value;
        }

        public string OPDToogle3
        {
            get => _model.OPDToogle3;
            set => _model.OPDToogle3 = value;
        }

        public string OPDToogle4
        {
            get => _model.OPDToogle4;
            set => _model.OPDToogle4 = value;
        }

        public bool OPDToogle1Checked
        {
            get => _model.OPDToogle1Checked;
            set => _model.OPDToogle1Checked = value;
        }
        public bool OPDToogle2Checked
        {
            get => _model.OPDToogle2Checked;
            set => _model.OPDToogle2Checked = value;
        }

        public bool OPDToogle3Checked
        {
            get => _model.OPDToogle3Checked;
            set => _model.OPDToogle3Checked = value;
        }

        public bool OPDToogle4Checked
        {
            get => _model.OPDToogle4Checked;
            set => _model.OPDToogle4Checked = value;
        }

        public bool OPDToogle1Enabled
        {
            get => _model.OPDToogle1Enabled;
            set => _model.OPDToogle1Enabled = value;
        }

        public bool OPDToogle2Enabled
        {
            get => _model.OPDToogle2Enabled;
            set => _model.OPDToogle2Enabled = value;
        }

        public bool OPDToogle3Enabled
        {
            get => _model.OPDToogle3Enabled;
            set => _model.OPDToogle3Enabled = value;
        }

        public bool OPDToogle4Enabled
        {
            get => _model.OPDToogle4Enabled;
            set => _model.OPDToogle4Enabled = value;
        }

        public Visibility OPDToogle1Visibility
        {
            get => _model.OPDToogle1Visibility;
            set => _model.OPDToogle1Visibility = value;
        }

        public Visibility OPDToogle2Visibility
        {
            get => _model.OPDToogle2Visibility;
            set => _model.OPDToogle2Visibility = value;
        }

        public Visibility OPDToogle3Visibility
        {
            get => _model.OPDToogle3Visibility;
            set => _model.OPDToogle3Visibility = value;
        }

        public Visibility OPDToogle4Visibility
        {
            get => _model.OPDToogle4Visibility;
            set => _model.OPDToogle4Visibility = value;
        }

        public bool OPDEnabled
        {
            get => _model.OPDEnabled;
            set => _model.OPDEnabled = value;
        }

        public bool UDEnabled
        {
            get => _model.UDEnabled;
            set => _model.UDEnabled = value;
        }

        public bool StopEnabled
        {
            get => _model.StopEnabled;
            set => _model.StopEnabled = value;
        }

        public bool SaveEnabled
        {
            get => _model.SaveEnabled;
            set => _model.SaveEnabled = value;
        }

        public bool StatChecked
        {
            get => _model.StatChecked;
            set => _model.StatChecked = value;
        }

        public bool BatchChecked
        {
            get => _model.BatchChecked;
            set => _model.BatchChecked = value;
        }

        public bool StatEnabled
        {
            get => _model.StatEnabled;
            set => _model.StatEnabled = value;
        }

        public bool BatchEnabled
        {
            get => _model.BatchEnabled;
            set => _model.BatchEnabled = value;
        }

        public Visibility StatVisibility
        {
            get => _model.StatVisibility;
            set => _model.StatVisibility = value;
        }

        public Visibility BatchVisibility
        {
            get => _model.BatchVisibility;
            set => _model.BatchVisibility = value;
        }

        public Visibility SplitEachMealVisibility
        {
            get => _model.SplitEachMealVisibility;
            set => _model.SplitEachMealVisibility = value;
        }

        public bool IsAutoStartChecked
        {
            get => _model.IsAutoStartChecked;
            set => _model.IsAutoStartChecked = value;
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
            get => _model.DoseType;
            set => _model.DoseType = value;
        }

        public string SuccessCount
        {
            get => _model.SuccessCount;
            set => _model.SuccessCount = value;
        }

        public string FailCount
        {
            get => _model.FailCount;
            set => _model.FailCount = value;
        }

        public string WindowX
        {
            get => _model.WindowX;
            set => _model.WindowX = value;
        }

        public string WindowY
        {
            get => _model.WindowY;
            set => _model.WindowY = value;
        }

        public bool WindowXEnabled
        {
            get => _model.WindowXEnabled;
            set => _model.WindowXEnabled = value;
        }

        public bool WindowYEnabled
        {
            get => _model.WindowYEnabled;
            set => _model.WindowYEnabled = value;
        }

        public Visibility WindowXVisibility
        {
            get => _model.WindowXVisibility;
            set => _model.WindowXVisibility = value;
        }

        public Visibility WindowYVisibility
        {
            get => _model.WindowYVisibility;
            set => _model.WindowYVisibility = value;
        }

        public Visibility MinimumAndCloseVisibility
        {
            get => _model.MinimumAndCloseVisibility;
            set => _model.MinimumAndCloseVisibility = value;
        }

        public float OPDOpacity
        {
            get => _model.OPDOpacity;
            set => _model.OPDOpacity = value;
        }

        public float UDOpacity
        {
            get => _model.UDOpacity;
            set => _model.UDOpacity = value;
        }

        public SolidColorBrush OPDBackground
        {
            get => _model.OPDBacground;
            set => _model.OPDBacground = value;
        }

        public SolidColorBrush UDBackground
        {
            get => _model.UDBackground;
            set => _model.UDBackground = value;
        }

        public string ProgressBoxContent
        {
            get => _model.ProgressBoxContent;
            set => _model.ProgressBoxContent = value;
        }
        public void OPDFunc()
        {
            if ((_model.OPDToogle1Checked | _model.OPDToogle2Checked | _model.OPDToogle3Checked | _model.OPDToogle4Checked) == false)
            {
                FCP.src.Message.Show("沒有勾選任一個轉檔位置", "位置未勾選", PackIconKind.Error, KindColors.Error);
                return;
            }
            IsOPD = true;
            _format.ConvertPrepare(IsOPD);
        }

        public void UDFunc()
        {
            IsOPD = false;
            _format.ConvertPrepare(IsOPD);
        }

        public void StopFunc()
        {
            _format.Stop();
            RefreshUIPropertyServices.SwitchMainWindowControlState(true);
            RefreshUIPropertyServices.SwitchUIStateForStop();
        }

        public void SaveFunc()
        {
            Properties.Settings.Default.X = Convert.ToInt32(WindowX);
            Properties.Settings.Default.Y = Convert.ToInt32(WindowY);
            Properties.Settings.Default.Save();
            SettingModel model = _settingModel;
            model.InputPath1 = InputPath1;
            model.InputPath2 = InputPath2;
            model.InputPath3 = InputPath3;
            model.OutputPath = OutputPath;
            model.StatOrBatch= StatChecked ? eDepartment.UDStat : eDepartment.UDBatch;
            model.AutoStart = IsAutoStartChecked;
            _setting.Save(model);
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

        public void GenerateCurrentFormat()
        {
            _format = FormatFactory.GenerateFormat(_settingModel.Format);
            _format.Init();
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
            GenerateCurrentFormat();
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
            string BackupPath = $@"{CommonModel.FileBackupDirectory}\{DateTime.Now:yyyy-MM-dd}";
            if (!Directory.Exists($@"{BackupPath}\Success")) Directory.CreateDirectory($@"{BackupPath}\Success");
            if (!Directory.Exists($@"{BackupPath}\Fail")) Directory.CreateDirectory($@"{BackupPath}\Fail");
            SuccessPath = $@"{BackupPath}\Success";
            FailPath = $@"{BackupPath}\Fail";
        }

        private void ShowAdvancedSettingsFunc()
        {
            AdvancedSettingsFactory.ClearAllViewModel();
            var f = AdvancedSettingsFactory.GenerateAdvancedSettings();
            f.ShowDialog();
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
            string BackupPath = $@"{CommonModel.FileBackupDirectory}\{DateTime.Now:yyyy-MM-dd}";
            SuccessCount = $"{Directory.GetFiles($@"{BackupPath}\Success").Length}";
            FailCount = $"{Directory.GetFiles($@"{BackupPath}\Fail").Length}";
            InputPath1 = _settingModel.InputPath1;
            InputPath2 = _settingModel.InputPath2;
            InputPath3 = _settingModel.InputPath3;
            OutputPath = _settingModel.OutputPath;
            IsAutoStartChecked = _settingModel.AutoStart;
            WindowX = Properties.Settings.Default.X.ToString();
            WindowY = Properties.Settings.Default.Y.ToString();
            StopEnabled = false;
            StatChecked = _settingModel.StatOrBatch == eDepartment.UDStat;
            BatchChecked = _settingModel.StatOrBatch == eDepartment.UDBatch;
        }

        private bool CanStartConverterOrShowAdvancedSettings()
        {
            return !StopEnabled;
        }
    }
}
