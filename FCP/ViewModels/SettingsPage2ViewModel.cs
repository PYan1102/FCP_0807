using System.Windows;
using System.Windows.Input;
using FCP.Core;
using FCP.Models;
using FCP.src.Factory.Models;

namespace FCP.ViewModels
{
    class SettingsPage2ViewModel : ViewModelBase
    {
        public ICommand ShowOnlyCanisterIn { get; set; }
        private SettingPage2Model _model;
        private SettingModel _settingModel;

        public SettingsPage2ViewModel()
        {
            _settingModel = ModelsFactory.GenerateSettingModel();
            _model = ModelsFactory.GenerateSettingPage2Model();

            ShowOnlyCanisterIn = new RelayCommand(() => ShowOnlyCanisterInFunc());

            Init();
        }

        public bool UseStatAndBatchOptionChecked
        {
            get => _model.UseStatAndBatchOptionChecked;
            set => _model.UseStatAndBatchOptionChecked = value;
        }

        public bool MinimizeWindowWhenProgramStartChecked
        {
            get => _model.MinimizeWindowWhenProgramStartChecked;
            set => _model.MinimizeWindowWhenProgramStartChecked = value;
        }

        public bool ShowCloseAndMinimizeButtonChecked
        {
            get => _model.ShowCloseAndMinimizeButtonChecked;
            set => _model.ShowCloseAndMinimizeButtonChecked = value;
        }

        public bool ShowXYChecked
        {
            get => _model.ShowXYChecked;
            set => _model.ShowXYChecked = value;
        }

        public bool FilterMedicineCodeChecked
        {
            get => _model.FilterMedicineCodeChecked;
            set => _model.FilterMedicineCodeChecked = value;
        }

        public bool OnlyCanisterInChecked
        {
            get => _model.OnlyCanisterInChecked;
            set => _model.OnlyCanisterInChecked = value;
        }

        public Visibility OnlyCanisterInVisibility
        {
            get => _model.OnlyCanisterInVisibility;
            set => _model.OnlyCanisterInVisibility = value;
        }

        public bool WhenCompeletedMoveFileChecked
        {
            get => _model.WhenCompeletedMoveFileChecked;
            set => _model.WhenCompeletedMoveFileChecked = value;
        }

        public bool WhenCompeletedStopChecked
        {
            get => _model.WhenCompeletedStopChecked;
            set => _model.WhenCompeletedStopChecked = value;
        }

        public string FileExtensionName
        {
            get => _model.FileExtensionName;
            set => _model.FileExtensionName = value;
        }

        private void Init()
        {
            UseStatAndBatchOptionChecked = _settingModel.UseStatOrBatch;
            MinimizeWindowWhenProgramStartChecked = _settingModel.WindowMinimize;
            ShowCloseAndMinimizeButtonChecked = _settingModel.ShowWindowOperationButton;
            ShowXYChecked = _settingModel.ShowXYParameter;
            FilterMedicineCodeChecked = _settingModel.UseFilterMedicineCode;
            OnlyCanisterInChecked = _settingModel.FilterNoCanister;
            FileExtensionName = _settingModel.FileExtensionName;
            WhenCompeletedMoveFileChecked = _settingModel.MoveSourceFileToBackupDirectoryWhenDone;
            WhenCompeletedStopChecked = _settingModel.StopWhenDone;
            ShowOnlyCanisterInFunc();
        }

        private void ShowOnlyCanisterInFunc()
        {
            OnlyCanisterInVisibility = FilterMedicineCodeChecked ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
