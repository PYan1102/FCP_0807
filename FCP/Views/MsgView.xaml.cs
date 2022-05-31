using System.Windows;
using FCP.ViewModels;

namespace FCP.Views
{
    /// <summary>
    /// Msg.xaml 的互動邏輯
    /// </summary>
    public partial class MsgView : Window
    {
        public MsgView(MsgViewModel viewModel)
        {
            InitializeComponent();
            _msgVM = viewModel;
            this.DataContext = _msgVM;
        }

        private MsgViewModel _msgVM { get; set; }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
