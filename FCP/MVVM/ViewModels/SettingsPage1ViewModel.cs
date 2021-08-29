using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCP.Core;
using System.Collections.ObjectModel;
using FCP.MVVM.Models;
using System.Windows;
using System.Windows.Input;
using FCP.MVVM.Factory.ViewModels;
using MaterialDesignThemes.Wpf;

namespace FCP.MVVM.ViewModels
{
    class SettingsPage1ViewModel : ViewModelBase
    {
        public ICommand NormalPack { get; set; }
        public ICommand FilterAdminCode { get; set; }
        public ICommand UseAdminCode { get; set; }
        public ICommand AddFilterMedicineCodeItem { get; set; }
        public ICommand RemoveFilterMedicineCodeItem { get; set; }
        public bool SelectionAll { get; set; }
        private SettingsPage1Model _Model;
        private MsgBViewModel _MsgBVM { get => MsgBFactory.GenerateMsgBViewModel(); }
        public SettingsPage1ViewModel()
        {
            _Model = new SettingsPage1Model();
            SearchFrequency = 100;
            NormalPack = new RelayCommand(() => NormalPackFunc());
            FilterAdminCode = new RelayCommand(() => FilterAdminCodeFunc());
            UseAdminCode = new RelayCommand(() => UseAdminCodeFunc());
            AddFilterMedicineCodeItem = new RelayCommand(() => AddFilterMedicineCodeItemFunc());
            RemoveFilterMedicineCodeItem = new RelayCommand(() => RemoveFilterMedicineCodeItemFunc());
            var list = new ObservableCollection<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");
            list.Add("4");
            FilterMedicineCode = list;
            FilterMedicineCodeIndex = 2;
        }

        public int SearchFrequency
        {
            get => _Model.SearchFrequency;
            set
            {
                _Model.SearchFrequency = value;
                OnPropertyChanged();
            }
        }

        public int Mode
        {
            get => _Model.Mode;
            set
            {
                _Model.Mode = value;
                OnPropertyChanged();
            }
        }

        public bool NormalPackChecked
        {
            get => _Model.NormalPackChecked;
            set
            {
                _Model.NormalPackChecked = value;
                OnPropertyChanged();
            }
        }

        public bool FilterAdminCodeChecked
        {
            get => _Model.FilterAdminCodeChecked;
            set
            {
                _Model.FilterAdminCodeChecked = value;
                OnPropertyChanged();
            }
        }

        public bool UseAdminCodeChecked
        {
            get => _Model.UseAdminCodeChecked;
            set
            {
                _Model.UseAdminCodeChecked = value;
                OnPropertyChanged();
            }
        }

        public Visibility PackMode
        {
            get => _Model.PackModeVisible;
            set
            {
                _Model.PackModeVisible = value;
                OnPropertyChanged();
            }
        }

        public string AdminCode
        {
            get => _Model.AdminCode;
            set
            {
                _Model.AdminCode = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> AdminCodePackList
        {
            get => _Model.AdminCodePackList;
            set
            {
                _Model.AdminCodePackList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Random
        {
            get => _Model.Random;
            set
            {
                _Model.Random = value;
                OnPropertyChanged();
            }
        }

        public bool MultiChecked
        {
            get => _Model.MultiChecked;
            set
            {
                _Model.MultiChecked = value;
                OnPropertyChanged();
            }
        }
        public bool CombiChecked
        {
            get => _Model.CombiChecked;
            set
            {
                _Model.CombiChecked = value;
                OnPropertyChanged();
            }
        }

        public string OutputSpecialAdminCode
        {
            get => _Model.OutputSpecialAdminCode;
            set
            {
                _Model.OutputSpecialAdminCode = value;
                OnPropertyChanged();
            }
        }

        public string CutTime
        {
            get => _Model.CutTime;
            set
            {
                _Model.CutTime = value;
                OnPropertyChanged();
            }
        }

        public string AdminCodeOfCrossDay
        {
            get => _Model.AdminCodeOfCrossDay;
            set
            {
                _Model.AdminCodeOfCrossDay = value;
                OnPropertyChanged();
            }
        }

        public string MedicineCode
        {
            get => _Model.MedicineCode;
            set
            {
                _Model.MedicineCode = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> FilterMedicineCode
        {
            get => _Model.FilterMedicineCode;
            set
            {
                _Model.FilterMedicineCode = value;
                OnPropertyChanged();
            }
        }

        public int FilterMedicineCodeIndex
        {
            get => _Model.FilterMedicineCodeIndex;
            set
            {
                _Model.FilterMedicineCodeIndex = value;
                OnPropertyChanged();
            }
        }

        public void NormalPackFunc()
        {
            NormalPackChecked = true;
            FilterAdminCodeChecked = false;
            UseAdminCodeChecked = false;
            PackMode = Visibility.Hidden;
        }

        public void FilterAdminCodeFunc()
        {
            NormalPackChecked = false;
            FilterAdminCodeChecked = true;
            UseAdminCodeChecked = false;
            PackMode = Visibility.Visible;
        }

        public void UseAdminCodeFunc()
        {
            NormalPackChecked = false;
            FilterAdminCodeChecked = false;
            UseAdminCodeChecked = true;
            PackMode = Visibility.Visible;
        }

        public void AddFilterMedicineCodeItemFunc()
        {
            if (MedicineCode.Trim().Length == 0)
                return;
            if (FilterMedicineCode.Contains(MedicineCode))
            {
                _MsgBVM.Show($"該藥品代碼 {MedicineCode} 已建立，請重新確認", "重複", PackIconKind.Error, KindColors.Error);
                return;
            }
            FilterMedicineCode.Add(MedicineCode);
            MedicineCode = string.Empty;
        }

        public void RemoveFilterMedicineCodeItemFunc()
        {
            if (FilterMedicineCodeIndex == -1)
                return;
            FilterMedicineCode.RemoveAt(FilterMedicineCodeIndex);
            if (FilterMedicineCode.Count == 0)
                return;
            FilterMedicineCodeIndex = 0;
        }
    }
}
