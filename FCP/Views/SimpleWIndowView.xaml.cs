using System.Windows;
using FCP.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using FCP.src.MessageManager;

namespace FCP.Views
{
    /// <summary>
    /// SmallForm.xaml 的互動邏輯
    /// </summary>
    public partial class SimpleWindowView : Window
    {
        public SimpleWindowView()
        {
            InitializeComponent();
            _simpleWindowVM = App.Current.Services.GetService<SimpleWindowViewModel>();
            this.DataContext = _simpleWindowVM;
        }

        private SimpleWindowViewModel _simpleWindowVM;

        private void ActivateWindow()
        {
            this.Activate();
            this.Focus();
            this.Topmost = true;
        }

        private void CloseWindow()
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _simpleWindowVM.IsActive = true;
            WeakReferenceMessenger.Default.Register<ActivateMessage, string>(this, nameof(SimpleWindowView), (r, m) => ActivateWindow());
            WeakReferenceMessenger.Default.Register<CloseWindowMessage, string>(this, nameof(SimpleWindowView), (r, m) => CloseWindow());
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _simpleWindowVM.IsActive = false;
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}