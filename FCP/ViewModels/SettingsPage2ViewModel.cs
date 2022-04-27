using System;
using System.Windows;
using System.Windows.Input;
using FCP.Core;
using FCP.Models;
using FCP.src.Factory;
using FCP.src.Factory.Models;

namespace FCP.ViewModels
{
    class SettingsPage2ViewModel : ViewModelBase
    {
        public ICommand ShowOnlyCanisterIn { get; set; }
        private SettingsPage2Model _Model;
        private SettingsModel _SettingsModel;

        public SettingsPage2ViewModel()
        {
            ShowOnlyCanisterIn = new RelayCommand(() => ShowOnlyCanisterInFunc());
            _Model = ModelsFactory.GenerateSettingsPage2Model();
            _SettingsModel = SettingsFactory.GenerateSettingsModel();

            Init();
        }

        public bool ShowStatAndBatchOptionChecked
        {
            get => _Model.ShowStatAndBatchOptionChecked;
            set => _Model.ShowStatAndBatchOptionChecked = value;
        }

        public bool MinimizeWindowWhenProgramStartChecked
        {
            get => _Model.MinimizeWindowWhenProgramStartChecked;
            set => _Model.MinimizeWindowWhenProgramStartChecked = value;
        }

        public bool ShowCloseAndMinimizeButtonChecked
        {
            get => _Model.ShowCloseAndMinimizeButtonChecked;
            set => _Model.ShowCloseAndMinimizeButtonChecked = value;
        }

        public bool ShowXYChecked
        {
            get => _Model.ShowXYChecked;
            set => _Model.ShowXYChecked = value;
        }

        public bool FilterMedicineCodeChecked
        {
            get => _Model.FilterMedicineCodeChecked;
            set => _Model.FilterMedicineCodeChecked = value;
        }

        public bool OnlyCanisterInChecked
        {
            get => _Model.OnlyCanisterInChecked;
            set => _Model.OnlyCanisterInChecked = value;
        }

        public Visibility OnlyCanisterInVisibility
        {
            get => _Model.OnlyCanisterInVisibility;
            set => _Model.OnlyCanisterInVisibility = value;
        }

        public bool WhenCompeletedMoveFileChecked
        {
            get => _Model.WhenCompeletedMoveFileChecked;
            set => _Model.WhenCompeletedMoveFileChecked = value;
        }

        public bool WhenCompeletedStopChecked
        {
            get => _Model.WhenCompeletedStopChecked;
            set => _Model.WhenCompeletedStopChecked = value;
        }

        public string FileExtensionName
        {
            get => _Model.FileExtensionName;
            set => _Model.FileExtensionName = value;
        }

        private void Init()
        {
            ShowStatAndBatchOptionChecked = _SettingsModel.EN_StatOrBatch;
            MinimizeWindowWhenProgramStartChecked = _SettingsModel.EN_WindowMinimumWhenOpen;
            ShowCloseAndMinimizeButtonChecked = _SettingsModel.EN_ShowControlButton;
            ShowXYChecked = _SettingsModel.EN_ShowXY;
            FilterMedicineCodeChecked = _SettingsModel.EN_FilterMedicineCode;
            OnlyCanisterInChecked = _SettingsModel.EN_OnlyCanisterIn;
            FileExtensionName = _SettingsModel.FileExtensionName;
            WhenCompeletedMoveFileChecked = _SettingsModel.EN_WhenCompeletedMoveFile;
            WhenCompeletedStopChecked = _SettingsModel.EN_WhenCompeletedStop;
            ShowOnlyCanisterInFunc();
        }

        private void ShowOnlyCanisterInFunc()
        {
            OnlyCanisterInVisibility = FilterMedicineCodeChecked ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
