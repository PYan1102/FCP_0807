using System;
using System.Windows;
using FCP.ViewModels;
using FCP.src.Factory.ViewModel;

namespace FCP.Views
{
    /// <summary>
    /// Msg.xaml 的互動邏輯
    /// </summary>
    public partial class Msg : Window
    {
        private MsgViewModel _msgVM { get; set; }
        public Msg()
        {
            InitializeComponent();
            _msgVM = MsgFactory.GenerateMsgViewModel();
            this.DataContext = _msgVM;
        }
    }
}
