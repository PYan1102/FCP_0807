using System;
using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;
using FCP.MVVM.Models;
using FCP.src.Factory.ViewModel;
using FCP.MVVM.ViewModels;

namespace FCP
{
    /// <summary>
    /// AdvanedSettings.xaml 的互動邏輯
    /// </summary>
    public partial class AdvancedSettings : Window
    {

        public AdvancedSettings()
        {
            InitializeComponent();
            this.Owner = WindowOwner.MainWindowOwner;
            this.DataContext = AdvancedSettingsFactory.GenerateAdvancedSettingsViewModel();
            var vm = this.DataContext as AdvancedSettingsViewModel;
            vm.CloseWindow += CloseWindow;
        }

        private void CloseWindow()
        {
            this.DialogResult = true;
        }

        private void Gd_Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}