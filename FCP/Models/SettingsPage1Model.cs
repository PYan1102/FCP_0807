using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using FCP.ViewModels;

namespace FCP.Models
{
    class SettingPage1Model
    {
        public int SearchFrequency { get; set; } = 100;
        public ObservableCollection<string> Mode { get; set; }
        public int FormatIndex { get; set; } = 0;
        public bool NormalPackChecked { get; set; }
        public bool FilterAdminCodeChecked { get; set; }
        public bool UseAdminCodeChecked { get; set; }
        public Visibility PackModeVisible { get; set; }
        public string AdminCode { get; set; } = string.Empty;
        public ObservableCollection<string> FilterAdminCodeList { get; set; }
        public int FilerAdminCodeIndex { get; set; } = 0;
        public ObservableCollection<RandomInfo> Random { get; set; } = new ObservableCollection<RandomInfo>();
        public int RandomIndex { get; set; } = 0;
        public bool MultiChecked { get; set; }
        public bool CombiChecked { get; set; }
        public string OutputSpecialAdminCode { get; set; } = string.Empty;
        public string CutTime { get; set; } = string.Empty;
        public string AdminCodeOfCrossDay { get; set; } = string.Empty;
        public string MedicineCode { get; set; } = string.Empty;
        public ObservableCollection<string> FilterMedicineCodeList { get; set; }
        public int FilterMedicineCodeIndex { get; set; } = 0;
    }
}
