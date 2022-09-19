using FCP.src.MessageManager;
using FCP.ViewModels;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace FCP.Views
{
    /// <summary>
    /// SettingsPage1View.xaml 的互動邏輯
    /// </summary>
    public partial class SettingPage1View : UserControl
    {
        public SettingPage1View()
        {
            InitializeComponent();
        }

        public void FocusFilterAdminCode()
        {
            Txt_FilterAdminCode.Focus();
        }

        private void FocusFilterMedicineCode()
        {
            Txt_FilterMedicineCode.Focus();
        }

        public void SelectAllFilterAdminCode()
        {
            Txt_FilterAdminCode.SelectAll();
        }

        private void SelectAllFilterMedicineCode()
        {
            Txt_FilterMedicineCode.SelectAll();
        }

        private void RefreshRandomDataGridView()
        {
            Dg_RandomSetting.Items.Refresh();
        }

        private void RefreshFilterMedicineCodeComboBox()
        {
            Cbo_FilterMedicineCode.Items.Refresh();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Register<View_FocusedElementMessage, string>(this, "Txt_FilterAdminCode", (r, m) =>  FocusFilterAdminCode());
            WeakReferenceMessenger.Default.Register<View_FocusedElementMessage, string>(this, "Txt_FilterMedicineCode", (r, m) =>  FocusFilterMedicineCode());
            WeakReferenceMessenger.Default.Register<View_SelectedAllMessage, string>(this, "Txt_FilterAdminCode", (r, m) =>  SelectAllFilterAdminCode());
            WeakReferenceMessenger.Default.Register<View_SelectedAllMessage, string>(this, "Txt_FilterMedicineCode", (r, m) =>  SelectAllFilterMedicineCode());
            WeakReferenceMessenger.Default.Register<View_RefresedhElementMessage, string>(this, "Dg_RandomSetting", (r, m) => RefreshRandomDataGridView());
            WeakReferenceMessenger.Default.Register<View_RefresedhElementMessage, string>(this, "Cbo_FilterMedicineCode", (r, m) =>  RefreshFilterMedicineCodeComboBox());
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
    }
}
