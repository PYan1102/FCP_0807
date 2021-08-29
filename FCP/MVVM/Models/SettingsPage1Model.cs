using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FCP.MVVM.Models
{
    class SettingsPage1Model
    {
        public int SearchFrequency { get; set; }
        public int Mode { get; set; }
        public bool NormalPackChecked { get; set; }
        public bool FilterAdminCodeChecked { get; set; }
        public bool UseAdminCodeChecked { get; set; }
        public Visibility PackModeVisible { get; set; }
        public string AdminCode { get; set; }
        public ObservableCollection<string> AdminCodePackList { get; set; }
        public ObservableCollection<string> Random { get; set; }
        public bool MultiChecked { get; set; }
        public bool CombiChecked { get; set; }
        public string OutputSpecialAdminCode { get; set; }
        public string CutTime { get; set; }
        public string AdminCodeOfCrossDay { get; set; }
        public string MedicineCode { get; set; }
        public ObservableCollection<string> FilterMedicineCode { get; set; }
        public int FilterMedicineCodeIndex { get; set; }
    }
}
