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
using FCP.Services;
using Helper;
using System.Diagnostics;
using FCP.src;
using FCP.src.Factory.Models;

namespace FCP.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
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
        private FormatBase _formatBase;
        private SettingModel _settingModel;
        private MsgViewModel _msgVM;
        private MainWindowModel _model;

        public MainWindowViewModel()
        {
            _msgVM = MsgFactory.GenerateMsgViewModel();
            _settingModel = ModelsFactory.GenerateSettingModel();

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
                Process.Start($@"{Environment.CurrentDirectory}\Log\{DateTime.Now:yyyy-MM-dd}\Log_{DateTime.Now:HH}.txt");
            }
            catch
            {
                MsgCollection.Show("目前尚未產生Log檔", "錯誤", PackIconKind.Error, KindColors.Error);
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

        public string InputDirectory1Title
        {
            get => _model.InputDirectory1Title;
            set => _model.InputDirectory1Title = value;
        }

        public string InputDirectory2Title
        {
            get => _model.InputDirectory2Title;
            set => _model.InputDirectory2Title = value;
        }

        public string InputDirectory3Title
        {
            get => _model.InputDirectory3Title;
            set => _model.InputDirectory3Title = value;
        }

        public string InputDirectory4Title
        {
            get => _model.InputDirectory4Title;
            set => _model.InputDirectory4Title = value;
        }

        public string InputDirectory5Title
        {
            get => _model.InputDirectory5Title;
            set => _model.InputDirectory5Title = value;
        }

        public string InputDirectory6Title
        {
            get => _model.InputDirectory6Title;
            set => _model.InputDirectory6Title = value;
        }

        public string InputDirectory1
        {
            get => _model.InputDirectory1;
            set => _model.InputDirectory1 = value;
        }

        public string InputDirectory2
        {
            get => _model.InputDirectory2;
            set => _model.InputDirectory2 = value;
        }

        public string InputDirectory3
        {
            get => _model.InputDirectory3;
            set => _model.InputDirectory3 = value;
        }


        public string InputDirectory4
        {
            get => _model.InputDirectory4;
            set => _model.InputDirectory4 = value;
        }

        public string InputDirectory5
        {
            get => _model.InputDirectory5;
            set => _model.InputDirectory5 = value;
        }

        public string InputDirectory6
        {
            get => _model.InputDirectory6;
            set => _model.InputDirectory6 = value;
        }

        public bool InputDirectory1Enabled
        {
            get => _model.InputDirectory1Enabled;
            set => _model.InputDirectory1Enabled = value;
        }

        public bool InputDirectory2Enabled
        {
            get => _model.InputDirectory2Enabled;
            set => _model.InputDirectory2Enabled = value;
        }

        public bool InputDirectory3Enabled
        {
            get => _model.InputDirectory3Enabled;
            set => _model.InputDirectory3Enabled = value;
        }

        public bool InputDirectory4Enabled
        {
            get => _model.InputDirectory4Enabled;
            set => _model.InputDirectory4Enabled = value;
        }

        public bool InputDirectory5Enabled
        {
            get => _model.InputDirectory5Enabled;
            set => _model.InputDirectory5Enabled = value;
        }

        public bool InputDirectory6Enabled
        {
            get => _model.InputDirectory6Enabled;
            set => _model.InputDirectory6Enabled = value;
        }

        public string OutputDirectoryTitle
        {
            get => _model.OutputDirectoryTitle;
            set => _model.OutputDirectoryTitle = value;
        }

        public string OutputDirectory
        {
            get => _model.OutputDirectory;
            set => _model.OutputDirectory = value;
        }

        public bool OutputDirectoryEnabled
        {
            get => _model.OutputDirectoryEnabled;
            set => _model.OutputDirectoryEnabled = value;
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
            get
            {
                _settingModel.StatOrBatch = _model.StatChecked ? eDepartment.Stat : eDepartment.Batch;
                return _model.StatChecked;
            }
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
            if ((_model.OPDToogle1Checked || _model.OPDToogle2Checked || _model.OPDToogle3Checked || _model.OPDToogle4Checked) == false)
            {
                MsgCollection.Show("沒有勾選任一個模式", "模式未勾選", PackIconKind.Error, KindColors.Error);
                return;
            }
            CommonModel.CurrentDepartment = eDepartment.OPD;
            _formatBase.ConvertPrepare();
        }

        public void UDFunc()
        {
            CommonModel.CurrentDepartment = eDepartment.UD;
            _formatBase.ConvertPrepare();
        }

        public void StopFunc()
        {
            _formatBase.Stop();
            RefreshUIPropertyServices.SwitchMainWindowControlState(true);
            RefreshUIPropertyServices.SwitchUIStateForStop();
        }

        public void SaveFunc()
        {
            Properties.Settings.Default.X = Convert.ToInt32(WindowX);
            Properties.Settings.Default.Y = Convert.ToInt32(WindowY);
            Properties.Settings.Default.Save();
            SettingModel model = _settingModel;
            model.InputDirectory1 = InputDirectory1;
            model.InputDirectory2 = InputDirectory2;
            model.InputDirectory3 = InputDirectory3;
            model.InputDirectory4 = InputDirectory4;
            model.InputDirectory5 = InputDirectory5;
            model.InputDirectory6 = InputDirectory6;
            model.OutputDirectory = OutputDirectory;
            model.AutoStart = IsAutoStartChecked;
            Setting.Save(model);
            AddLog("儲存成功");
        }

        public void AddLog(string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ProgressBoxContent);
            sb.AppendLine(message);
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
            _formatBase = FormatFactory.GenerateFormat(_settingModel.Format);
            _formatBase.Init();
        }

        private async void LoadedFuncAsync()
        {
            Log.Path = $@"{Environment.CurrentDirectory}\Log";
            CheckProgramIsAlreadyOpen();
            RefreshUIPropertyServices.SwitchMainWindowControlState(true);
            NotifyIconHelper.Init(Properties.Resources.FCP, "轉檔");
            NotifyIconHelper.DoubleClickAction += NotifyIconDBClick;
            CommonModel.NotifyIcon = NotifyIconHelper.NotifyIcon;
            GenerateCurrentFormat();
            Init();
            if (IsAutoStartChecked)
            {
                await Task.Delay(1000);
                OPDFunc();
            }
        }

        //檢查程式是否已開啟
        private void CheckProgramIsAlreadyOpen()
        {
            if (Process.GetProcessesByName("FCP").Length > 1)
            {
                Log.Write("程式已開啟，請確認工具列");
                MsgCollection.Show("程式已開啟，請確認工具列", "重複開啟", PackIconKind.Error, KindColors.Error);
                Environment.Exit(0);
            }
        }

        private void ShowAdvancedSettingsFunc()
        {
            AdvancedSettingFactory.ClearAllViewModel();
            var f = AdvancedSettingFactory.GenerateAdvancedSettings();
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
                    InputDirectory1 = path;
                    break;
                case "IP2":
                    InputDirectory2 = path;
                    break;
                case "IP3":
                    InputDirectory3 = path;
                    break;
                case "IP4":
                    InputDirectory4 = path;
                    break;
                case "IP5":
                    InputDirectory5 = path;
                    break;
                case "IP6":
                    InputDirectory6 = path;
                    break;
                case "OP":
                    OutputDirectory = path;
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
                    InputDirectory1 = string.Empty;
                    break;
                case "IP2":
                    InputDirectory2 = string.Empty;
                    break;
                case "IP3":
                    InputDirectory3 = string.Empty;
                    break;
                case "IP4":
                    InputDirectory4 = string.Empty;
                    break;
                case "IP5":
                    InputDirectory5 = string.Empty;
                    break;
                case "IP6":
                    InputDirectory6 = string.Empty;
                    break;
                default:
                    break;
            }
        }

        private void Init()
        {
            InputDirectory1 = _settingModel.InputDirectory1;
            InputDirectory2 = _settingModel.InputDirectory2;
            InputDirectory3 = _settingModel.InputDirectory3;
            InputDirectory4 = _settingModel.InputDirectory4;
            InputDirectory5 = _settingModel.InputDirectory5;
            InputDirectory6 = _settingModel.InputDirectory6;
            OutputDirectory = _settingModel.OutputDirectory;
            IsAutoStartChecked = _settingModel.AutoStart;
            WindowX = Properties.Settings.Default.X.ToString();
            WindowY = Properties.Settings.Default.Y.ToString();
            StopEnabled = false;
            StatChecked = _settingModel.StatOrBatch == eDepartment.Stat;
            BatchChecked = _settingModel.StatOrBatch == eDepartment.Batch;
        }

        private bool CanStartConverterOrShowAdvancedSettings()
        {
            return !StopEnabled;
        }
    }
}
