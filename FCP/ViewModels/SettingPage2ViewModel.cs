using FCP.Models;
using FCP.src.Factory.Models;
using FCP.src.MessageManager.Request;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FCP.ViewModels
{
    class SettingPage2ViewModel : ObservableValidator
    {
        public SettingPage2ViewModel()
        {
            _settingModel = ModelsFactory.GenerateSettingModel();
            _model = ModelsFactory.GenerateSettingPage2Model();

            ShowOnlyCanisterIn = new RelayCommand(ShowOnlyCanisterInFunc);

            Init();
        }

        public ICommand ShowOnlyCanisterIn { get; set; }
        private SettingPage2Model _model;
        private SettingJsonModel _settingModel;

        public bool UseStatAndBatchOptionChecked
        {
            get => _model.UseStatAndBatchOptionChecked;
            set => SetProperty(_model.UseStatAndBatchOptionChecked, value, _model, (model, _value) => model.UseStatAndBatchOptionChecked = _value);
        }

        public bool MinimizeWindowWhenProgramStartChecked
        {
            get => _model.MinimizeWindowWhenProgramStartChecked;
            set => SetProperty(_model.MinimizeWindowWhenProgramStartChecked, value, _model, (model, _value) => model.MinimizeWindowWhenProgramStartChecked = _value);
        }

        public bool ShowCloseAndMinimizeButtonChecked
        {
            get => _model.ShowCloseAndMinimizeButtonChecked;
            set => SetProperty(_model.ShowCloseAndMinimizeButtonChecked, value, _model, (model, _value) => model.ShowCloseAndMinimizeButtonChecked = _value);
        }

        public bool ShowXYChecked
        {
            get => _model.ShowXYChecked;
            set => SetProperty(_model.ShowXYChecked, value, _model, (model, _value) => model.ShowXYChecked = _value);
        }

        public bool FilterMedicineCodeChecked
        {
            get => _model.FilterMedicineCodeChecked;
            set => SetProperty(_model.FilterMedicineCodeChecked, value, _model, (model, _value) => model.FilterMedicineCodeChecked = _value);
        }

        public bool OnlyCanisterInChecked
        {
            get => _model.OnlyCanisterInChecked;
            set => SetProperty(_model.OnlyCanisterInChecked, value, _model, (model, _value) => model.OnlyCanisterInChecked = _value);
        }

        public Visibility OnlyCanisterInVisibility
        {
            get => _model.OnlyCanisterInVisibility;
            set => SetProperty(_model.OnlyCanisterInVisibility, value, _model, (model, _value) => model.OnlyCanisterInVisibility = _value);
        }

        public bool WhenCompeletedMoveFileChecked
        {
            get => _model.WhenCompeletedMoveFileChecked;
            set => SetProperty(_model.WhenCompeletedMoveFileChecked, value, _model, (model, _value) => model.WhenCompeletedMoveFileChecked = _value);
        }

        public bool WhenCompeletedStopChecked
        {
            get => _model.WhenCompeletedStopChecked;
            set => SetProperty(_model.WhenCompeletedStopChecked, value, _model, (model, _value) => model.WhenCompeletedStopChecked = _value);
        }

        public bool IgnoreAdminCodeIfNotInOnCubeChecked
        {
            get => _model.IgnoreAdminCodeIfNotInOnCubeChecked;
            set => SetProperty(_model.IgnoreAdminCodeIfNotInOnCubeChecked, value, _model, (model, _value) => model.IgnoreAdminCodeIfNotInOnCubeChecked = _value);
        }

        [Required(ErrorMessage = "此欄位不可為空")]
        public string FileExtensionNames
        {
            get => _model.FileExtensionNames;
            set => SetProperty(_model.FileExtensionNames, value, _model, (model, _value) => model.FileExtensionNames = _value, true);
        }

        public void ResetModel()
        {
            WeakReferenceMessenger.Default.Unregister<HasErrorsRequestMessage, string>(this, nameof(SettingPage2ViewModel));
            Init();
        }

        private void Init()
        {
            WeakReferenceMessenger.Default.Register<HasErrorsRequestMessage, string>(this, nameof(SettingPage2ViewModel), (r, m) => m.Reply(this.HasErrors));

            UseStatAndBatchOptionChecked = _settingModel.UseStatAndBatchOption;
            MinimizeWindowWhenProgramStartChecked = _settingModel.MinimizeWindowWhenProgramStart;
            ShowCloseAndMinimizeButtonChecked = _settingModel.ShowCloseAndMinimizeButton;
            ShowXYChecked = _settingModel.ShowXY;
            FilterMedicineCodeChecked = _settingModel.FilterMedicineCode;
            OnlyCanisterInChecked = _settingModel.OnlyCanisterIn;
            string extensionNames = "";
            _settingModel.FileExtensionNames.ForEach(x => extensionNames += $"{x},");
            extensionNames = extensionNames.TrimEnd(',');
            FileExtensionNames = extensionNames;
            WhenCompeletedMoveFileChecked = _settingModel.WhenCompeletedMoveFile;
            WhenCompeletedStopChecked = _settingModel.WhenCompeletedStop;
            IgnoreAdminCodeIfNotInOnCubeChecked = _settingModel.IgnoreAdminCodeIfNotInOnCube;
            ShowOnlyCanisterInFunc();
        }

        private void ShowOnlyCanisterInFunc()
        {
            OnlyCanisterInVisibility = FilterMedicineCodeChecked ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
