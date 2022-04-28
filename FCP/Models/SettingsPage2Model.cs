using System.Windows;

namespace FCP.Models
{
    class SettingPage2Model
    {
        public bool ShowStatAndBatchOptionChecked { get; set; }
        public bool MinimizeWindowWhenProgramStartChecked { get; set; }
        public bool ShowCloseAndMinimizeButtonChecked { get; set; }
        public bool ShowXYChecked { get; set; }
        public bool FilterMedicineCodeChecked { get; set; }
        public bool OnlyCanisterInChecked { get; set; }
        public Visibility OnlyCanisterInVisibility { get; set; }
        public bool WhenCompeletedMoveFileChecked { get; set; }
        public bool WhenCompeletedStopChecked { get; set; }
        public string FileExtensionName { get; set; }
    }
}
