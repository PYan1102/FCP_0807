using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCP.MVVM.ViewModels.MainWindow;
using FCP.MVVM.Models;

namespace FCP.MVVM.Factory.ViewModels
{
    static class MainWindowFacotry
    {
        private static PropertyChange _PropertyChange { get; set; }
        private static MainWindowModel _MainWindowModel { get; set; }

        public static PropertyChange GeneratePropertyChange()
        {
            if (_PropertyChange == null)
                _PropertyChange = new PropertyChange();
            return _PropertyChange;
        }

        public static MainWindowModel GenerateMainWindowModel()
        {
            if (_MainWindowModel == null)
                _MainWindowModel = new MainWindowModel();
            return _MainWindowModel;
        }
    }
}
