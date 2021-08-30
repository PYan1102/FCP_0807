using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FCP.Core;
using System.Windows.Media;
using FCP.MVVM.Models;
using FCP.MVVM.Factory;
using System.Windows;

namespace FCP.MVVM.ViewModels
{
    class AdvancedSettingsViewModel : ViewModelBase
    {
        public ICommand Page1 { get; set; }
        public ICommand Page2 { get; set; }
        public ICommand Save { get; set; }
        public ICommand Close { get; set; }
        private SettingsPage1ViewModel _SettingsPage1VM { get; set; }
        private SettingsPage2ViewModel _SettingsPage2VM { get; set; }
        private object _CurrentView;
        private AdvancedSettingsModel _Model;
        private SolidColorBrush Yellow = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4D03F"));
        private SolidColorBrush Blue = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1D6FB1"));
        private SolidColorBrush White = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));

        public AdvancedSettingsViewModel()
        {
            _Model = new AdvancedSettingsModel();
            Page1 = new RelayCommand(() => Page1Func());
            Page2 = new RelayCommand(() => Page2Func());
            Save = new RelayCommand(() => SaveFunc());
            Close = new ObjectRelayCommand(o => ((Window)o).DialogResult = true);
        }

        public object CurrentView
        {
            get => _CurrentView;
            set => _CurrentView = value;
        }

        public SolidColorBrush Page1Backround
        {
            get => _Model.Page1Background;
            set => _Model.Page1Background = value;
        }

        public SolidColorBrush Page1Foreground
        {
            get => _Model.Page1Foreground;
            set => _Model.Page1Foreground = value;
        }

        public SolidColorBrush Page2Backround
        {
            get => _Model.Page2Background;
            set => _Model.Page2Background = value;
        }

        public SolidColorBrush Page2Foreground
        {
            get => _Model.Page2Foreground;
            set => _Model.Page2Foreground = value;
        }

        public void Page1Func()
        {
            Page1Backround = Yellow;
            Page1Foreground = White;
            Page2Backround = White;
            Page2Foreground = Blue;
            if (_SettingsPage1VM == null)
                _SettingsPage1VM = new SettingsPage1ViewModel();
            CurrentView = _SettingsPage1VM;
        }

        public void Page2Func()
        {
            Page1Backround = White;
            Page1Foreground = Blue;
            Page2Backround = Yellow;
            Page2Foreground = White;
            if (_SettingsPage2VM == null)
                _SettingsPage2VM = new SettingsPage2ViewModel();
            CurrentView = _SettingsPage2VM;
        }

        public void SaveFunc()
        {
            Console.WriteLine(_SettingsPage1VM.SearchFrequency);
        }
    }
}
