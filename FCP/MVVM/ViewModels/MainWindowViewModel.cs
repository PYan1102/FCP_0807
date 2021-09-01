using FCP.Core;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCP.MVVM.Factory;
using System.Windows.Input;
using FCP.MVVM.Models;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Factory.ViewModel;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;
using FCP.MVVM.View;

namespace FCP.MVVM.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public bool IsOPD { get; set; }
        public ICommand ShowAdvancedSettings { get; set; }
        public ICommand OPD { get; set; }
        public ICommand UD { get; set; }
        public ICommand Stop { get; set; }
        public ICommand Save { get; set; }
        public ICommand Close { get; set; }
        public ICommand Closing { get; set; }
        public ICommand Loaded { get; set; }
        public ICommand SwitchWindow { get; set; }
        public ICommand Activate { get; set; }
        private MainWindowModel _Model;
        private FunctionCollections _Format { get; set; }
        private SettingsModel _SettingsModel { get; set; }
        private MsgBViewModel _MsgBVM { get; set; }

        public MainWindowViewModel()
        {
            _SettingsModel = SettingsFactory.GenerateSettingsModel();
            _MsgBVM = MsgBFactory.GenerateMsgBViewModel();
            ShowAdvancedSettings = new RelayCommand(ShowAdvancedSettingsFunc, CanStartConverterOrShowAdvancedSettings);
            _Model = new MainWindowModel();
            Loaded = new RelayCommand(() => LoadedFunc());
            Close = new RelayCommand(() => Environment.Exit(0));
            Closing = new RelayCommand(() => Disconnect小港醫院NetDiskFunc());
            OPD = new RelayCommand(OPDFunc, CanStartConverterOrShowAdvancedSettings);
            UD = new RelayCommand(UDFunc, CanStartConverterOrShowAdvancedSettings);
            Stop = new RelayCommand(StopFunc, CanStartConverterOrShowAdvancedSettings);
            Save = new RelayCommand(SaveFunc, CanStartConverterOrShowAdvancedSettings);
            SwitchWindow = new RelayCommand(SwitchWindowFunc,CanStartConverterOrShowAdvancedSettings);
            Activate = new ObjectRelayCommand(o => ActivateFunc((Window)o), o => CanActivate());
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

        public void OPDFunc()
        {
            this.Visibility = Visibility.Hidden;
            return;
            if ((_Model.OPDToogle1Checked | _Model.OPDToogle2Checked | _Model.OPDToogle3Checked | _Model.OPDToogle4Checked) == false)
            {
                _MsgBVM.Show("沒有勾選任一個轉檔位置", "位置未勾選", PackIconKind.Error, KindColors.Error);
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
        }

        public void SaveFunc()
        {
            _Format.Save();
        }

        public void ActivateFunc(Window window)
        {
            window.Activate();
            window.Focus();
        }

        private void SwitchWindowFunc()
        {
            Visibility = Visibility.Hidden;
            var vm = SimpleWindowFactory.GenerateSimpleWindowViewModel();
            vm.Visibility = Visibility.Visible;
            RefreshUIPropertyServices.InitSimpleWindow();
        }

        private void LoadedFunc()
        {
            JudgeCurrentFormat(true);
        }

        private void JudgeCurrentFormat(bool isStart)
        {
            _Format = FormatFactory.GenerateFormat(_SettingsModel.Mode);
            _Format.SetWindow(null);
            _Format.Init();
            if (isStart) _Format.AutoStart();
        }

        private void ShowAdvancedSettingsFunc()
        {
            var window = AdvancedSettingsFactory.GenerateAdvancedSettings();
            window.ShowDialog();
            window = null;
        }

        private void Disconnect小港醫院NetDiskFunc()
        {
            switch (_SettingsModel.Mode)
            {
                case Format.小港醫院TOC:
                    _Format.Stop();
                    break;
            }
        }

        private bool CanStartConverterOrShowAdvancedSettings()
        {
            return !StopEnabled;
        }

        private bool CanActivate()
        {
            return Visibility == Visibility.Visible;
        }
    }
}
