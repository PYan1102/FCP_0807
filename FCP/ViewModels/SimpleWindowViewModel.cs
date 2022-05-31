using System;
using System.Windows;
using System.Windows.Media;
using FCP.Models;
using System.Windows.Input;
using FCP.src.Enum;
using System.Text;
using System.Collections.Generic;
using FCP.src.Factory.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using FCP.src.MessageManager;
using Microsoft.Toolkit.Mvvm.Messaging;
using FCP.src.Dictionary;

namespace FCP.ViewModels
{
    class SimpleWindowViewModel : ObservableRecipient
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
        private SimpleWindowModel _model;
        private SettingJsonModel _settingModel;

        public SimpleWindowViewModel()
        {
            _settingModel = ModelsFactory.GenerateSettingModel();
            _model = new SimpleWindowModel();
            Activate = new RelayCommand(() => Messenger.Send(new ActivateMessage(), nameof(FCP.Views.SimpleWindowView)), CanActivate);
            SwitchWindow = new RelayCommand(SwitchWindowFunc, CanStartConverterOrShowAdvancedSettings);
            ShowAndCloseLog = new RelayCommand(ShowAndCloseLogFunc);
            ClearLog = new RelayCommand(() => Log = string.Empty);
            OPD = new RelayCommand(OPDFunc, CanStartConverterOrShowAdvancedSettings);
            UD = new RelayCommand(UDFunc, CanStartConverterOrShowAdvancedSettings);
            Stop = new RelayCommand(StopFunc);
            MinimumWindow = new RelayCommand(() => Visibility = Visibility.Hidden);
            Close = new RelayCommand(() => Environment.Exit(0));
            Loaded = new RelayCommand(LoadedFunc);
        }

        public Visibility Visibility
        {
            get => _model.Visibility;
            set => SetProperty(_model.Visibility, value, _model, (model, _value) => model.Visibility = _value);
        }

        public bool Focusable
        {
            get => _model.Focusable;
            set => SetProperty(_model.Focusable, value, _model, (model, _value) => model.Focusable = _value);
        }

        public int Top
        {
            get => _model.Top;
            set => _model.Top = value;
        }

        public int Left
        {
            get => _model.Left;
            set => _model.Left = value;
        }

        public bool MultiChecked
        {
            get => _model.MultiChecked;
            set => _model.MultiChecked = value;
        }

        public bool CombiChecked
        {
            get => _model.CombiChecked;
            set => _model.CombiChecked = value;
        }

        public bool MultiEnabled
        {
            get => _model.MultiEnabled;
            set => _model.MultiEnabled = value;
        }

        public bool CombiEnabled
        {
            get => _model.CombiEnabled;
            set => _model.CombiEnabled = value;
        }

        public Visibility MultiVisibility
        {
            get => _model.MultiVisibility;
            set => _model.MultiVisibility = value;
        }

        public Visibility CombiVisibility
        {
            get => _model.CombiVisibility;
            set => _model.CombiVisibility = value;
        }

        public string OPDContent
        {
            get => _model.OPDContent;
            set => _model.OPDContent = value;
        }

        public SolidColorBrush OPDBackground
        {
            get => _model.OPDBackground;
            set => _model.OPDBackground = value;
        }

        public bool OPDEnabled
        {
            get => _model.OPDEnabled;
            set => _model.OPDEnabled = value;
        }

        public float OPDOpacity
        {
            get => _model.OPDOpacity;
            set => _model.OPDOpacity = value;
        }

        public SolidColorBrush UDBackground
        {
            get => _model.UDBackground;
            set => _model.UDBackground = value;
        }

        public bool UDEnabled
        {
            get => _model.UDEnabled;
            set => _model.UDEnabled = value;
        }

        public float UDOpacity
        {
            get => _model.UDOpacity;
            set => _model.UDOpacity = value;
        }

        public Visibility UDVisibility
        {
            get => _model.UDVisibility;
            set => _model.UDVisibility = value;
        }

        public SolidColorBrush StopBackground
        {
            get => _model.StopBackground;
            set => _model.StopBackground = value;
        }

        public bool StopEnabled
        {
            get => _model.StopEnabled;
            set => _model.StopEnabled = value;
        }

        public float StopOpacity
        {
            get => _model.StopOpacity;
            set => _model.StopOpacity = value;
        }

        public Visibility StatVisibility
        {
            get => _model.StatVisibility;
            set => _model.StatVisibility = value;
        }

        public bool StatChecked
        {
            get => _model.StatChecked;
            set => _model.StatChecked = value;
        }

        public bool StatEnabled
        {
            get => _model.StatEnabled;
            set => _model.StatEnabled = value;
        }

        public Visibility BatchVisibility
        {
            get => _model.BatchVisibility;
            set => _model.BatchVisibility = value;
        }

        public bool BatchChecked
        {
            get => _model.BatchChecked;
            set => _model.BatchChecked = value;
        }

        public bool BatchEnabled
        {
            get => _model.BatchEnabled;
            set => _model.BatchEnabled = value;
        }

        public string Log
        {
            get => _model.Log;
            set => _model.Log = value;
        }

        public Visibility CloseVisibility
        {
            get => _model.CloseVisibility;
            set => _model.CloseVisibility = value;
        }

        public Visibility MinimumVisibility
        {
            get => _model.MinimumVisibility;
            set => _model.MinimumVisibility = value;
        }

        public Visibility LogVisibility
        {
            get => _model.LogVisibility;
            set => _model.LogVisibility = value;
        }

        public void SwitchWindowFunc()
        {
            Messenger.Send(new CloseWindowMessage(), nameof(FCP.Views.SimpleWindowView));
            Messenger.Send(new VisibilityMessage(), nameof(MainWindowViewModel));
            CommonModel.WindowType = eWindowType.MainWindow;
        }

        public void ShowAndCloseLogFunc()
        {
            LogVisibility = LogVisibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        public void OPDFunc()
        {
            Messenger.Send(new OpreationMessage(), nameof(eOpreation.OPD));
            OPDEnabled = false;
            UDEnabled = false;
            StopEnabled = true;
            StatEnabled = false;
            BatchEnabled = false;
            MultiEnabled = false;
            CombiEnabled = false;
            OPDBackground = dColor.GetSolidColorBrush(eColor.Red);
            UDOpacity = 0.2f;
            StopOpacity = 1;
        }

        public void UDFunc()
        {
            Messenger.Send(new OpreationMessage(), nameof(eOpreation.UD));
            Properties.Settings.Default.DoseType = MultiChecked ? "M" : "C";
            Properties.Settings.Default.Save();
            //if (StatChecked)
            //    _mainWindowVM.StatChecked = true;
            //else
            //    _mainWindowVM.BatchChecked = true;
            OPDEnabled = false;
            UDEnabled = false;
            StopEnabled = true;
            StatEnabled = false;
            BatchEnabled = false;
            CombiEnabled = false;
            MultiEnabled = false;
            UDBackground = dColor.GetSolidColorBrush(eColor.Red);
            OPDOpacity = 0.2f;
            StopOpacity = 1;
        }

        public void StopFunc()
        {
            Messenger.Send(new OpreationMessage(), nameof(eOpreation.Stop));
            OPDEnabled = true;
            UDEnabled = true;
            StopEnabled = false;
            StatEnabled = true;
            BatchEnabled = true;
            CombiEnabled = true;
            MultiEnabled = true;
            OPDBackground = dColor.GetSolidColorBrush(eColor.White);
            UDBackground = dColor.GetSolidColorBrush(eColor.White);
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

        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<VisibilityMessage, string>(this, nameof(SimpleWindowViewModel), (r, m) =>
            {
                Visibility = Visibility.Visible;
                Focusable = true;
            });
            Messenger.Register<CommandMessage, string>(this, nameof(eCommandCollection.SetSimpleWindowPosition), (r, m) => { SetWindowPosition(); });
            Messenger.Register<ClearSimpleWindowLogMessage>(this, (r, m) => { Log = string.Empty; });
        }

        private void LoadedFunc()
        {
            List<eFormat> hospitalCustomers = new List<eFormat>() { eFormat.光田醫院OC, eFormat.民生醫院OC, eFormat.義大醫院OC };
            List<eFormat> powderCustomers = new List<eFormat>() { eFormat.光田醫院JVS };
            if (hospitalCustomers.Contains(_settingModel.Format))
            {
                OPDContent = "門 診F5";
                UDVisibility = Visibility.Visible;
            }
            MultiVisibility = _settingModel.Format == eFormat.光田醫院OC ? Visibility.Visible : Visibility.Hidden;
            CombiVisibility = _settingModel.Format == eFormat.光田醫院OC ? Visibility.Visible : Visibility.Hidden;
            MultiChecked = Properties.Settings.Default.DoseType == "M";
            if (powderCustomers.Contains(_settingModel.Format))
            {
                OPDContent = "磨 粉F5";
                UDVisibility = Visibility.Hidden;
            }
            StatVisibility = _settingModel.UseStatOrBatch ? Visibility.Visible : Visibility.Hidden;
            BatchVisibility = _settingModel.UseStatOrBatch ? Visibility.Visible : Visibility.Hidden;
            CloseVisibility = _settingModel.ShowWindowOperationButton ? Visibility.Visible : Visibility.Hidden;
            MinimumVisibility = _settingModel.ShowWindowOperationButton ? Visibility.Visible : Visibility.Hidden;
            StatChecked = _settingModel.StatOrBatch == eDepartment.Stat;
            StopEnabled = false;
            Top = Properties.Settings.Default.Y;
            Left = Properties.Settings.Default.X;
        }

        private void SetWindowPosition()
        {
            Top = Properties.Settings.Default.Y;
            Left = Properties.Settings.Default.X;
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