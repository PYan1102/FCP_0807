using System.Windows;
using System.Windows.Input;
using FCP.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FCP.Views
{
    /// <summary>
    /// AdvanedSettings.xaml 的互動邏輯
    /// </summary>
    public partial class SettingView : Window
    {
        public SettingView()
        {
            InitializeComponent();
            _settingVM = App.Current.Services.GetService<SettingViewModel>();
            this.DataContext = _settingVM;
        }

        private SettingViewModel _settingVM;

        private void Gd_Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}