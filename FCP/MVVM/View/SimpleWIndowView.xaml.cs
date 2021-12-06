using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using FCP.MVVM.Models;
using FCP.MVVM.Control;
using FCP.src.Factory;
using FCP.src.Enum;
using FCP.MVVM.ViewModels;
using FCP.src.Factory.ViewModel;

namespace FCP.MVVM.View
{
    /// <summary>
    /// SmallForm.xaml 的互動邏輯
    /// </summary>
    public partial class SimpleWindowView : Window
    {
        public SimpleWindowView(MainWindow m)
        {
            InitializeComponent();
            this.DataContext = SimpleWindowFactory.GenerateSimpleWindowViewModel();
            var vm = this.DataContext as SimpleWindowViewModel;
            vm.ActivateWindow += ActivateWindow;
        }

        private void ActivateWindow()
        {
            this.Activate();
        }
    }
}
