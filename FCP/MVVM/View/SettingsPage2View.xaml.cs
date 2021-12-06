using System.Windows.Controls;
using FCP.src.Factory.ViewModel;
using FCP.src;

namespace FCP.MVVM.View
{
    /// <summary>
    /// SettingsPage2View.xaml 的互動邏輯
    /// </summary>
    public partial class SettingsPage2View : UserControl
    {
        public SettingsPage2View()
        {
            InitializeComponent();
            this.DataContext = AdvancedSettingsFactory.GenerateSettingsPage2ViewModel();
        }
    }
}
