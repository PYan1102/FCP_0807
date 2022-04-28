using System.Windows.Controls;
using FCP.src;
using FCP.src.Factory.ViewModel;
using FCP.ViewModels;
using System;
using System.Collections.Generic;

namespace FCP.Views
{
    /// <summary>
    /// SettingsPage1View.xaml 的互動邏輯
    /// </summary>
    public partial class SettingsPage1View : UserControl
    {
        public SettingsPage1View()
        {
            InitializeComponent();
            this.DataContext = AdvancedSettingFactory.GenerateSettingsPage1ViewModel();
            var vm = this.DataContext as SettingsPage1ViewModel;
            vm.FocusFilterAdminCode += FocusFilterAdminCode;
            vm.SelectAllFilterAdminCode += SelectAllFilterAdminCode;
            vm.FocusFilterMedicineCode += FocusFilterMedicineCode;
            vm.SelectAllFilterMedicineCode += SelectAllFilterMedicineCode;
            vm.RefreshRandomDataGridView += RefreshRandomDataGridView;
            vm.RefreshFilterMedicineCodeComboBox += RefreshFilterMedicineCodeComboBox;
        }

        public void FocusFilterAdminCode()
        {
            Txt_FilterAdminCode.Focus();
        }

        public void SelectAllFilterAdminCode()
        {
            Txt_FilterAdminCode.SelectAll();
        }

        private void SelectAllFilterMedicineCode()
        {
            Txt_FilterMedicineCode.Focus();
        }

        private void FocusFilterMedicineCode()
        {
            Txt_FilterMedicineCode.SelectAll();
        }

        private void RefreshRandomDataGridView()
        {
            Dg_JVServerRandomSetting.Items.Refresh();
        }

        private void RefreshFilterMedicineCodeComboBox()
        {
            Cbo_FilterMedicineCode.Items.Refresh();
        }
    }
}
