using System;
using System.Windows;
using FCP.MVVM.ViewModels;
using FCP.MVVM.Factory.ViewModels;

namespace FCP
{
    /// <summary>
    /// MsgB.xaml 的互動邏輯
    /// </summary>
    public partial class MsgB : Window
    {
        private MsgBViewModel _MsgBVM { get; set; }
        public MsgB()
        {
            InitializeComponent();
            _MsgBVM = MsgBFactory.GenerateMsgBViewModel();
            this.DataContext = _MsgBVM;
        }
    }
}
