using System;
using System.Windows;
using System.Windows.Media;
using FCP.Models;
using System.Windows.Input;
using FCP.src.Enum;
using System.Collections.Generic;
using FCP.src.Factory.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using FCP.src.MessageManager;
using Microsoft.Toolkit.Mvvm.Messaging;
using FCP.Providers;
using FCP.src.MessageManager.Change;

namespace FCP.ViewModels
{
    class SimpleWindowViewModel : ObservableRecipient
    {
        public SimpleWindowViewModel()
        {
            _settingModel = ModelsFactory.GenerateSettingModel();
            _model = new SimpleWindowModel();

            Activate = new RelayCommand(() => Messenger.Send(new ActivateMessage(), nameof(FCP.Views.SimpleWindowView)), CanActivate);
            SwitchWindow = new RelayCommand(SwitchWindowFunc, CanStartConverterOrShowAdvancedSettings);
            ClearLog = new RelayCommand(ClearLogFunc);
            OPD = new RelayCommand(OPDFunc, CanStartConverterOrShowAdvancedSettings);
            UD = new RelayCommand(UDFunc, CanStartConverterOrShowAdvancedSettings);
            Stop = new RelayCommand(StopFunc);
            MinimumWindow = new RelayCommand(() => Visibility = Visibility.Hidden);
            Close = new RelayCommand(() => Environment.Exit(0));
            Loaded = new RelayCommand(LoadedFunc);
        }

        public ICommand Activate { get; set; }
        public ICommand SwitchWindow { get; set; }
        public ICommand ClearLog { get; set; }
        public ICommand OPD { get; set; }
        public ICommand UD { get; set; }
        public ICommand Stop { get; set; }
        public ICommand MinimumWindow { get; set; }
        public ICommand Close { get; set; }
        public ICommand Loaded { get; set; }
        private SimpleWindowModel _model;
        private SettingJsonModel _settingModel;

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
            set => SetProperty(_model.Top, value, _model, (model, _value) => model.Top = _value);
        }

        public int Left
        {
            get => _model.Left;
            set => SetProperty(_model.Left, value, _model, (model, _value) => model.Left = _value);
        }

        public bool MultiChecked
        {
            get => _model.MultiChecked;
            set => SetProperty(_model.MultiChecked, value, _model, (model, _value) => model.MultiChecked = _value);
        }

        public bool CombiChecked
        {
            get => _model.CombiChecked;
            set => SetProperty(_model.CombiChecked, value, _model, (model, _value) => model.CombiChecked = _value);
        }

        public bool MultiEnabled
        {
            get => _model.MultiEnabled;
            set => SetProperty(_model.MultiEnabled, value, _model, (model, _value) => model.MultiEnabled = _value);
        }

        public bool CombiEnabled
        {
            get => _model.CombiEnabled;
            set => SetProperty(_model.CombiEnabled, value, _model, (model, _value) => model.CombiEnabled = _value);
        }

        public Visibility MultiVisibility
        {
            get => _model.MultiVisibility;
            set => SetProperty(_model.MultiVisibility, value, _model, (model, _value) => model.MultiVisibility = _value);
        }

        public Visibility CombiVisibility
        {
            get => _model.CombiVisibility;
            set => SetProperty(_model.CombiVisibility, value, _model, (model, _value) => model.CombiVisibility = _value);
        }

        public string OPDContent
        {
            get => _model.OPDContent;
            set => SetProperty(_model.OPDContent, value, _model, (model, _value) => model.OPDContent = _value);
        }

        public SolidColorBrush OPDBackground
        {
            get => _model.OPDBackground;
            set => SetProperty(_model.OPDBackground, value, _model, (model, _value) => model.OPDBackground = _value);
        }

        public bool OPDEnabled
        {
            get => _model.OPDEnabled;
            set => SetProperty(_model.OPDEnabled, value, _model, (model, _value) => model.OPDEnabled = _value);
        }

        public float OPDOpacity
        {
            get => _model.OPDOpacity;
            set => SetProperty(_model.OPDOpacity, value, _model, (model, _value) => model.OPDOpacity = _value);
        }

        public SolidColorBrush UDBackground
        {
            get => _model.UDBackground;
            set => SetProperty(_model.UDBackground, value, _model, (model, _value) => model.UDBackground = _value);
        }

        public bool UDEnabled
        {
            get => _model.UDEnabled;
            set => SetProperty(_model.UDEnabled, value, _model, (model, _value) => model.UDEnabled = _value);
        }

        public float UDOpacity
        {
            get => _model.UDOpacity;
            set => SetProperty(_model.UDOpacity, value, _model, (model, _value) => model.UDOpacity = _value);
        }

        public Visibility UDVisibility
        {
            get => _model.UDVisibility;
            set => SetProperty(_model.UDVisibility, value, _model, (model, _value) => model.UDVisibility = _value);
        }

        public SolidColorBrush StopBackground
        {
            get => _model.StopBackground;
            set => SetProperty(_model.StopBackground, value, _model, (model, _value) => model.StopBackground = _value);
        }

        public bool StopEnabled
        {
            get => _model.StopEnabled;
            set => SetProperty(_model.StopEnabled, value, _model, (model, _value) => model.StopEnabled = _value);
        }

        public float StopOpacity
        {
            get => _model.StopOpacity;
            set => SetProperty(_model.StopOpacity, value, _model, (model, _value) => model.StopOpacity = _value);
        }

        public Visibility StatVisibility
        {
            get => _model.StatVisibility;
            set => SetProperty(_model.StatVisibility, value, _model, (model, _value) => model.StatVisibility = _value);
        }

        public bool StatChecked
        {
            get
            {
                _settingModel.StatOrBatch = _model.StatChecked ? eDepartment.Stat : eDepartment.Batch;
                return _model.StatChecked;
            }
            set => SetProperty(_model.StatChecked, value, _model, (model, _value) => model.StatChecked = _value);
        }

        public bool StatEnabled
        {
            get => _model.StatEnabled;
            set => SetProperty(_model.StatEnabled, value, _model, (model, _value) => model.StatEnabled = _value);
        }

        public Visibility BatchVisibility
        {
            get => _model.BatchVisibility;
            set => SetProperty(_model.BatchVisibility, value, _model, (model, _value) => model.BatchVisibility = _value);
        }

        public bool BatchChecked
        {
            get => _model.BatchChecked;
            set => SetProperty(_model.BatchChecked, value, _model, (model, _value) => model.BatchChecked = _value);
        }

        public bool BatchEnabled
        {
            get => _model.BatchEnabled;
            set => SetProperty(_model.BatchEnabled, value, _model, (model, _value) => model.BatchEnabled = _value);
        }

        public string ProgressBox
        {
            get => _model.ProgressBox;
            set => SetProperty(_model.ProgressBox, value, _model, (model, _value) => model.ProgressBox = _value);
        }

        public Visibility CloseVisibility
        {
            get => _model.CloseVisibility;
            set => SetProperty(_model.CloseVisibility, value, _model, (model, _value) => model.CloseVisibility = _value);
        }

        public Visibility MinimumVisibility
        {
            get => _model.MinimumVisibility;
            set => SetProperty(_model.MinimumVisibility, value, _model, (model, _value) => model.MinimumVisibility = _value);
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<VisibilityMessage, string>(this, nameof(SimpleWindowViewModel), (r, m) =>
            {
                Visibility = Visibility.Visible;
                Focusable = true;
            });
            Messenger.Register<CommandMessage, string>(this, nameof(eCommandCollection.SetSimpleWindowPosition), (r, m) => SetWindowPosition());
            Messenger.Register<LogChangeMessage>(this, (r, m) =>
            {
                ProgressBox += $"{m.Value}\n";
            });
        }

        private void SwitchWindowFunc()
        {
            CommonModel.WindowType = eWindowType.MainWindow;
            Messenger.Send(new CloseWindowMessage(), nameof(FCP.Views.SimpleWindowView));
            Messenger.Send(new VisibilityMessage(), nameof(MainWindowViewModel));
        }

        private void ClearLogFunc()
        {
            ProgressBox = string.Empty;
        }

        private void OPDFunc()
        {
            Messenger.Send(new OpreationMessage(), nameof(eOpreation.OPD));
            OPDEnabled = false;
            UDEnabled = false;
            StopEnabled = true;
            StatEnabled = false;
            BatchEnabled = false;
            MultiEnabled = false;
            CombiEnabled = false;
            OPDBackground = ColorProvider.GetSolidColorBrush(eColor.Red);
            UDOpacity = 0.2f;
            StopOpacity = 1;
        }

        private void UDFunc()
        {
            Messenger.Send(new OpreationMessage(), nameof(eOpreation.UD));
            Properties.Settings.Default.DoseType = MultiChecked ? "Multi" : "Combi";
            Properties.Settings.Default.Save();
            OPDEnabled = false;
            UDEnabled = false;
            StopEnabled = true;
            StatEnabled = false;
            BatchEnabled = false;
            CombiEnabled = false;
            MultiEnabled = false;
            UDBackground = ColorProvider.GetSolidColorBrush(eColor.Red);
            OPDOpacity = 0.2f;
            StopOpacity = 1;
        }

        private void StopFunc()
        {
            Messenger.Send(new OpreationMessage(), nameof(eOpreation.Stop));
            OPDEnabled = true;
            UDEnabled = true;
            StopEnabled = false;
            StatEnabled = true;
            BatchEnabled = true;
            CombiEnabled = true;
            MultiEnabled = true;
            MultiChecked = true;
            Properties.Settings.Default.DoseType = "Multi";
            Properties.Settings.Default.Save();
            OPDBackground = ColorProvider.GetSolidColorBrush(eColor.White);
            UDBackground = ColorProvider.GetSolidColorBrush(eColor.White);
            OPDOpacity = 1;
            UDOpacity = 1;
            StopOpacity = 0.2f;
        }

        private void LoadedFunc()
        {
            List<eFormat> hospitalCustomers = new List<eFormat>() { eFormat.光田醫院_大甲OC, eFormat.民生醫院OC, eFormat.義大醫院OC };
            List<eFormat> powderCustomers = new List<eFormat>() { eFormat.光田醫院JVS };
            if (hospitalCustomers.Contains(_settingModel.Format))
            {
                OPDContent = "門 診F5";
                UDVisibility = Visibility.Visible;
            }
            MultiVisibility = _settingModel.Format == eFormat.光田醫院_大甲OC ? Visibility.Visible : Visibility.Hidden;
            CombiVisibility = _settingModel.Format == eFormat.光田醫院_大甲OC ? Visibility.Visible : Visibility.Hidden;
            Properties.Settings.Default.DoseType = "Multi";
            Properties.Settings.Default.Save();
            MultiChecked = true;
            if (powderCustomers.Contains(_settingModel.Format))
            {
                OPDContent = "磨 粉F5";
                UDVisibility = Visibility.Hidden;
            }
            StatVisibility = _settingModel.UseStatAndBatchOption ? Visibility.Visible : Visibility.Hidden;
            BatchVisibility = _settingModel.UseStatAndBatchOption ? Visibility.Visible : Visibility.Hidden;
            CloseVisibility = _settingModel.ShowCloseAndMinimizeButton ? Visibility.Visible : Visibility.Hidden;
            MinimumVisibility = _settingModel.ShowCloseAndMinimizeButton ? Visibility.Visible : Visibility.Hidden;
            StatChecked = _settingModel.StatOrBatch == eDepartment.Stat;
            BatchChecked = _settingModel.StatOrBatch == eDepartment.Batch;
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