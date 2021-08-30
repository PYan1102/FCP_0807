using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCP.MVVM.Models;

namespace FCP.MVVM.Factory.ViewModel
{
    static class MainWindowFacotry
    {
        private static MainWindowModel _MainWindowModel { get; set; }
        public static MainWindowModel GenerateMainWindowModel()
        {
            if (_MainWindowModel == null)
                _MainWindowModel = new MainWindowModel();
            return _MainWindowModel;
        }
    }
}
