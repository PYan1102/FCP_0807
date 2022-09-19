using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using FCP.Models;
using FCP.src.Enum;
using FCP.src.Factory.Models;
using FCP.src.MessageManager.Request;
using Helper;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace FCP.ViewModels
{
    class SettingPage3ViewModel : ObservableObject
    {
        public SettingPage3ViewModel()
        {
            _settingModel = ModelsFactory.GenerateSettingModel();
            _model = new SettingPage3Model();

            Append = new RelayCommand(AppendFunc);
            Remove = new RelayCommand(RemoveFunc, CanRemove);
            CellEditEnding = new RelayCommand(CellEditEndingFunc);
            Init();
        }

        public ICommand Append { get; set; }
        public ICommand Remove { get; set; }
        public ICommand CellEditEnding { get; set; }
        private List<string> _prescriptionParameters;
        private List<string> _etc;
        private SettingPage3Model _model;
        private SettingJsonModel _settingModel;

        public ObservableCollection<ETCData> ETCData
        {
            get => _model.ETCData;
            set => SetProperty(_model.ETCData, value, _model, (model, _value) => model.ETCData = _value);
        }

        public int DataGridSelectedIndex
        {
            get => _model.DataGridSelectedIndex;
            set => SetProperty(_model.DataGridSelectedIndex, value, _model, (model, _value) => model.DataGridSelectedIndex = _value);
        }

        private void AppendFunc()
        {
            ETCData.Add(new ETCData() { ETC = _etc, PrescriptionParameters = _prescriptionParameters });
        }

        private void RemoveFunc()
        {
            ETCData.RemoveAt(DataGridSelectedIndex);
        }

        private void CellEditEndingFunc()
        {
            var data = ETCData[DataGridSelectedIndex];
            if (data.Format.Trim().Length == 0)
            {
                data.Format = "{0}";
            }
        }

        public void ResetModel()
        {
            WeakReferenceMessenger.Default.Unregister<HasErrorsRequestMessage, string>(this, nameof(SettingPage3ViewModel));
            Init();
        }

        private void Init()
        {
            _prescriptionParameters = EnumHelper.ToList<ePrescriptionParameters>();
            _etc = EnumHelper.ToList<eETC>();

            ETCData = new ObservableCollection<ETCData>();
            foreach (var v in _settingModel.ETCData)
            {
                ETCData.Add(new ETCData()
                {
                    ETC = _etc,
                    ETCIndex = v.ETCIndex,
                    PrescriptionParameters = _prescriptionParameters,
                    PrescriptionParameterIndex = v.PrescriptionParameterIndex,
                    Format = v.Format
                });
            }
        }

        private bool CanRemove()
        {
            return ETCData.Count > -1 && DataGridSelectedIndex > -1;
        }
    }
}
