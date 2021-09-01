using System;
using FCP.MVVM.View;
using FCP.MVVM.ViewModels;

namespace FCP.MVVM.Factory.ViewModel
{
    static class SimpleWindowFactory
    {
        private static SimpleWindowView _SimpleWindow { get; set; }
        private static SimpleWindowViewModel _SimpleWindowVM { get; set; }
        public static SimpleWindowView GenerateSimpleWindow()
        {
            if (_SimpleWindow == null)
                _SimpleWindow = new SimpleWindowView(null);
            return _SimpleWindow;
        }

        public static SimpleWindowViewModel GenerateSimpleWindowViewModel()
        {
            if (_SimpleWindowVM == null)
                _SimpleWindowVM = new SimpleWindowViewModel();
            return _SimpleWindowVM;
        }
    }
}