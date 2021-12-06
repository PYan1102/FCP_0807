using System;
using System.Windows;
using System.Windows.Media;
using FCP.Core;
using FCP.MVVM.Models;
using FCP.src.Factory.ViewModel;
using System.Windows.Input;
using FCP.src.Enum;
using System.Text;
using FCP.src.Factory.Models;
using System.Collections.Generic;

namespace FCP.MVVM.ViewModels
{
    class SimpleWindowViewModel : ViewModelBase
    {
        public ICommand Activate { get; set; }
        public ICommand SwitchWindow { get; set; }
        public ICommand ShowAndCloseLog { get; set; }
        public ICommand ClearLog { get; set; }
        public ICommand OPD { get; set; }
        public ICommand UD { get; set; }
        public ICommand Stop { get; set; }
        public ICommand MinimumWindow { get; set; }
        public ICommand Close { get; set; }
        public ICommand Loaded { get; set; }
        public Action ActivateWindow { get; set; }

        private MainWindowViewModel _MainWindowVM;
        private SimpleWindowModel _Model;
        private SolidColorBrush _DeepBlue = new SolidColorBrush((Color)Color.FromRgb(17, 68, 109));
        private SolidColorBrush _ShinyBlue = new SolidColorBrush((Color)Color.FromRgb(9, 225, 255));
        private SolidColorBrush _White = new SolidColorBrush((Color)Color.FromRgb(255, 255, 255));
        private SolidColorBrush _Red = new SolidColorBrush((Color)Color.FromRgb(255, 82, 85));
        private SettingsModel _SettingsModel;

        public SimpleWindowViewModel()
        {
            _MainWindowVM = MainWindowFactory.GenerateMainWindowViewModel();
            _Model = new SimpleWindowModel();
            _SettingsModel = SettingsFactory.GenerateSettingsModel();
            Activate = new ObjectRelayCommand(o => ActivateFunc((Window)o), o => CanActivate());
            SwitchWindow = new RelayCommand(SwitchWindowFunc);
            ShowAndCloseLog = new RelayCommand(ShowAndCloseLogFunc);
            ClearLog = new RelayCommand(() => Log = string.Empty);
            OPD = new RelayCommand(OPDFunc, CanStartConverterOrShowAdvancedSettings);
            UD = new RelayCommand(UDFunc, CanStartConverterOrShowAdvancedSettings);
            Stop = new RelayCommand(StopFunc);
            MinimumWindow = new RelayCommand(() => Visibility = Visibility.Hidden);
            Close = new RelayCommand(() => Environment.Exit(0));
            Loaded = new RelayCommand(() => LoadedFunc());
        }

        public Visibility Visibility
        {
            get => _Model.Visibility;
            set => _Model.Visibility = value;
        }

        public bool Topmost
        {
            get => _Model.Topmost;
            set => _Model.Topmost = value;
        }

        public bool Enabled
        {
            get => _Model.Enabled;
            set => _Model.Enabled = value;
        }

        public int Top
        {
            get => _Model.Top;
            set => _Model.Top = value;
        }

        public int Left
        {
            get => _Model.Left;
            set => _Model.Left = value;
        }

        public bool MultiChecked
        {
            get => _Model.MultiChecked;
            set => _Model.MultiChecked = value;
        }

        public bool CombiChecked
        {
            get => _Model.CombiChecked;
            set => _Model.CombiChecked = value;
        }

        public bool MultiEnabled
        {
            get => _Model.MultiEnabled;
            set => _Model.MultiEnabled = value;
        }

        public bool CombiEnabled
        {
            get => _Model.CombiEnabled;
            set => _Model.CombiEnabled = value;
        }

        public Visibility MultiVisibility
        {
            get => _Model.MultiVisibility;
            set => _Model.MultiVisibility = value;
        }

        public Visibility CombiVisibility
        {
            get => _Model.CombiVisibility;
            set => _Model.CombiVisibility = value;
        }

        public string OPDContent
        {
            get => _Model.OPDContent;
            set => _Model.OPDContent = value;
        }

        public SolidColorBrush OPDBackground
        {
            get => _Model.OPDBackground;
            set => _Model.OPDBackground = value;
        }

        public bool OPDEnabled
        {
            get => _Model.OPDEnabled;
            set => _Model.OPDEnabled = value;
        }

        public float OPDOpacity
        {
            get => _Model.OPDOpacity;
            set => _Model.OPDOpacity = value;
        }

        public SolidColorBrush UDBackground
        {
            get => _Model.UDBackground;
            set => _Model.UDBackground = value;
        }

        public bool UDEnabled
        {
            get => _Model.UDEnabled;
            set => _Model.UDEnabled = value;
        }

        public float UDOpacity
        {
            get => _Model.UDOpacity;
            set => _Model.UDOpacity = value;
        }

        public Visibility UDVisibility
        {
            get => _Model.UDVisibility;
            set => _Model.UDVisibility = value;
        }

        public SolidColorBrush StopBackground
        {
            get => _Model.StopBackground;
            set => _Model.StopBackground = value;
        }

        public bool StopEnabled
        {
            get => _Model.StopEnabled;
            set => _Model.StopEnabled = value;
        }

        public float StopOpacity
        {
            get => _Model.StopOpacity;
            set => _Model.StopOpacity = value;
        }

        public Visibility StatVisibility
        {
            get => _Model.StatVisibility;
            set => _Model.StatVisibility = value;
        }

        public bool StatChecked
        {
            get => _Model.StatChecked;
            set => _Model.StatChecked = value;
        }

        public bool StatEnabled
        {
            get => _Model.StatEnabled;
            set => _Model.StatEnabled = value;
        }

        public Visibility BatchVisibility
        {
            get => _Model.BatchVisibility;
            set => _Model.BatchVisibility = value;
        }

        public bool BatchChecked
        {
            get => _Model.BatchChecked;
            set => _Model.BatchChecked = value;
        }

        public bool BatchEnabled
        {
            get => _Model.BatchEnabled;
            set => _Model.BatchEnabled = value;
        }

        public string Log
        {
            get => _Model.Log;
            set => _Model.Log = value;
        }

        public Visibility CloseVisibility
        {
            get => _Model.CloseVisibility;
            set => _Model.CloseVisibility = value;
        }

        public Visibility MinimumVisibility
        {
            get => _Model.MinimumVisibility;
            set => _Model.MinimumVisibility = value;
        }

        public Visibility LogVisibility
        {
            get => _Model.LogVisibility;
            set => _Model.LogVisibility = value;
        }

        public void ActivateFunc(Window window)
        {
            window.Activate();
            Topmost = true;
            Enabled = true;
            window.Focus();
        }

        public void SwitchWindowFunc()
        {
            Visibility = Visibility.Hidden;
            Topmost = false;
            Enabled = false;
            var vm = MainWindowFactory.GenerateMainWindowViewModel();
            vm.Visibility = Visibility.Visible;
        }

        public void SetWindowPosition(int top, int left)
        {
            Top = top;
            Left = left;
        }

        public void ShowAndCloseLogFunc()
        {
            LogVisibility = LogVisibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        public void OPDFunc()
        {
            _MainWindowVM.OPDFunc();
            OPDEnabled = false;
            UDEnabled = false;
            StopEnabled = true;
            StatEnabled = false;
            BatchEnabled = false;
            MultiEnabled = false;
            CombiEnabled = false;
            OPDBackground = _Red;
            UDOpacity = 0.2f;
            StopOpacity = 1;
        }

        public void UDFunc()
        {
            //_MainWindowVM.UDFunc();
            Properties.Settings.Default.DoseType = MultiChecked ? "M" : "C";
            Properties.Settings.Default.Save();
            if (StatChecked)
                _MainWindowVM.StatChecked = true;
            else
                _MainWindowVM.BatchChecked = true;
            OPDEnabled = false;
            UDEnabled = false;
            StopEnabled = true;
            StatEnabled = false;
            BatchEnabled = false;
            CombiEnabled = false;
            MultiEnabled = false;
            UDBackground = _Red;
            OPDOpacity = 0.2f;
            StopOpacity = 1;
        }

        public void StopFunc()
        {
            //_MainWindowVM.StopFunc();
            OPDEnabled = true;
            UDEnabled = true;
            StopEnabled = false;
            StatEnabled = true;
            BatchEnabled = true;
            CombiEnabled = true;
            MultiEnabled = true;
            OPDBackground = _White;
            UDBackground = _White;
            OPDOpacity = 1;
            UDOpacity = 1;
            StopOpacity = 0.2f;
        }

        public void AddLog(string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Log);
            sb.Append($"{message}\n");
            Log = sb.ToString();
            sb = null;
        }

        private void LoadedFunc()
        {
            List<eFormat> hospitalCustomers = new List<eFormat>() { eFormat.小港醫院TOC, eFormat.光田醫院TOC, eFormat.民生醫院TOC, eFormat.義大醫院TOC };
            List<eFormat> powderCustomers = new List<eFormat>() { eFormat.光田醫院TJVS, eFormat.長庚磨粉TJVS };
            if (hospitalCustomers.Contains(_SettingsModel.Mode))
            {
                OPDContent = "門 診F5";
                UDVisibility = Visibility.Visible;
            }
            MultiVisibility = _SettingsModel.Mode == eFormat.光田醫院TOC ? Visibility.Visible : Visibility.Hidden;
            CombiVisibility = _SettingsModel.Mode == eFormat.光田醫院TOC ? Visibility.Visible : Visibility.Hidden;
            MultiChecked = Properties.Settings.Default.DoseType == "M";
            if (powderCustomers.Contains(_SettingsModel.Mode))
            {
                OPDContent = "磨 粉F5";
                UDVisibility = Visibility.Hidden;
            }
            StatVisibility = _SettingsModel.EN_StatOrBatch ? Visibility.Visible : Visibility.Hidden;
            BatchVisibility = _SettingsModel.EN_StatOrBatch ? Visibility.Visible : Visibility.Hidden;
            CloseVisibility = _SettingsModel.EN_ShowControlButton ? Visibility.Visible : Visibility.Hidden;
            MinimumVisibility = _SettingsModel.EN_ShowControlButton ? Visibility.Visible : Visibility.Hidden;
            StatChecked = _SettingsModel.StatOrBatch == "S";
            StopEnabled = false;
            Left = Properties.Settings.Default.X;
            Top = Properties.Settings.Default.Y;
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