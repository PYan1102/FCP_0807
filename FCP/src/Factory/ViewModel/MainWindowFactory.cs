using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCP.Models;
using FCP.ViewModels;

namespace FCP.src.Factory.ViewModel
{
    static class MainWindowFactory
    {
        private static MainWindowModel _MainWindowModel { get; set; }
        private static MainWindowViewModel _MainWindowVM { get; set; }
        public static MainWindowModel GenerateMainWindowModel()
        {
            if (_MainWindowModel == null)
                _MainWindowModel = new MainWindowModel();
            return _MainWindowModel;
        }

        public static MainWindowViewModel GenerateMainWindowViewModel()
        {
            if (_MainWindowVM == null)
                _MainWindowVM = new MainWindowViewModel();
            return _MainWindowVM;
        }
    }
}