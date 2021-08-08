using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCP.Core;

namespace FCP.MVVM.ViewModels
{
    class MainWindowViewModel : ObservableObject
    {
        public RelayCommand AdvancedSettingsCommand { get; set; }
        public AdvancedSettingsViewModel AdvancedSettingsVM { get; set; }

        private object _CurrentView { get; set; }

        public object CurrentView
        {
            get
            {
                return _CurrentView;
            }
            set
            {
                _CurrentView = value;
                OnPropertyChanged();
            }
        }
        public MainWindowViewModel()
        {
            AdvancedSettingsVM = new AdvancedSettingsViewModel();
            AdvancedSettingsCommand = new RelayCommand(o =>
            {
                CurrentView = AdvancedSettingsVM;
            });
        }
    }
}
