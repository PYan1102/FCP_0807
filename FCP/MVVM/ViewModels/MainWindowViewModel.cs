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
using FCP.MVVM.Factory.ViewModels;
using MaterialDesignThemes.Wpf;

namespace FCP.MVVM.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public bool IsOPD { get; set; }
        public ICommand ShowAdvancedSettings { get; set; }
        public ICommand OPD { get; set; }
        public ICommand UD { get; set; }
        public ICommand Stop { get; set; }
        public ICommand Close { get; set; }
        public ICommand Closing { get; set; }
        public ICommand Check { get; set; }
        private MainWindowModel _Model;
        private FunctionCollections _Format { get; set; }
        private SettingsModel _SettingsModel { get; set; }
        private MsgBViewModel _MsgBVM { get; set; }

        public MainWindowViewModel()
        {
            _SettingsModel = SettingsFactory.GenerateSettingsModels();
            _MsgBVM = MsgBFactory.GenerateMsgBViewModel();
            ShowAdvancedSettings = new RelayCommand(ShowAdvancedSettingsFunc, CanStartConverterOrShowAdvancedSettings);
            _Model = new MainWindowModel();
            Close = new RelayCommand(() => Environment.Exit(0));
            Closing = new RelayCommand(() => Disconnect小港醫院NetDiskFunc());
            OPD = new RelayCommand(OPDFunc, CanStartConverterOrShowAdvancedSettings);
            UD = new RelayCommand(UDFunc, CanStartConverterOrShowAdvancedSettings);
            Stop = new RelayCommand(StopFunc, CanStartConverterOrShowAdvancedSettings);
            Check = new RelayCommand(() => Console.WriteLine(IsStatChecked));
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

        public string OPDButtonContent
        {
            get => _Model.OPDButtonContent;
            set => _Model.OPDButtonContent = value;
        }

        public string UDButtonContent
        {
            get => _Model.UDButtonContent;
            set => _Model.UDButtonContent = value;
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

        public bool IsStatChecked
        {
            get => _Model.IsStatChecked;
            set => _Model.IsStatChecked = value;
        }

        public bool IsBatchChecked
        {
            get => _Model.IsBatchChecked;
            set => _Model.IsBatchChecked = value;
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

        private void ShowAdvancedSettingsFunc()
        {
            var window = SettingsFactory.GenerateAdvancesSettings();
            window.ShowDialog();
        }

        private void JudgeCurrentFormat(bool isStart)
        {
            _Format = FormatFactory.GenerateFormat(_SettingsModel.Mode);
            _Format.SetWindow(null);
            _Format.Init();
            if (isStart) _Format.AutoStart();
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

        private void OPDFunc()
        {
            if ((_Model.OPDToogle1Checked | _Model.OPDToogle2Checked | _Model.OPDToogle3Checked | _Model.OPDToogle4Checked) == false)
            {
                _MsgBVM.Show("沒有勾選任一個轉檔位置", "位置未勾選", PackIconKind.Error, KindColors.Error);
                return;
            }
            IsOPD = true;
            _Format.ConvertPrepare(IsOPD);
        }

        private void UDFunc()
        {
            IsOPD = false;
            _Format.ConvertPrepare(IsOPD);
        }

        private void StopFunc()
        {

        }

        private bool CanStartConverterOrShowAdvancedSettings()
        {
            return !StopEnabled;
        }
    }
}
