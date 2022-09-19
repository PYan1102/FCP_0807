using System.Collections.ObjectModel;
using System.Windows;

namespace FCP.Models
{
    public sealed class SettingPage1Model
    {
        public int SearchFrequency { get; set; } = 500;
        public ObservableCollection<string> Mode { get; set; } = new ObservableCollection<string>();
        public int FormatIndex { get; set; } = 0;
        public bool NormalPackChecked { get; set; }
        public bool FilterAdminCodeChecked { get; set; }
        public bool UseAdminCodeChecked { get; set; }
        public Visibility PackModeVisible { get; set; }
        public string AdminCode { get; set; } = string.Empty;
        public ObservableCollection<string> NeedToFilterAdminCodeList { get; set; }
        public int NeedToFilerAdminCodeIndex { get; set; } = 0;
        public ObservableCollection<RandomInfo> Random { get; set; } = new ObservableCollection<RandomInfo>();
        public int NeedToFilterMedicineCodeIndex { get; set; } = 0;
        public int RandomIndex { get; set; } = 0;
        public bool MultiChecked { get; set; }
        public bool CombiChecked { get; set; }
        public string OutputSpecialAdminCode { get; set; } = string.Empty;
        public string AdminCodeOfCrossDay { get; set; } = string.Empty;
        public string MedicineCode { get; set; } = string.Empty;
        public ObservableCollection<string> NeedToFilterMedicineCodeList { get; set; } = new ObservableCollection<string>();
    }
}
