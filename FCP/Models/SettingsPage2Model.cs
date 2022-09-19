using System.Windows;

namespace FCP.Models
{
    public sealed class SettingPage2Model
    {
        public bool UseStatAndBatchOptionChecked { get; set; }
        public bool MinimizeWindowWhenProgramStartChecked { get; set; }
        public bool ShowCloseAndMinimizeButtonChecked { get; set; }
        public bool ShowXYChecked { get; set; }
        public bool FilterMedicineCodeChecked { get; set; }
        public bool OnlyCanisterInChecked { get; set; }
        public Visibility OnlyCanisterInVisibility { get; set; }
        public bool WhenCompeletedMoveFileChecked { get; set; }
        public bool WhenCompeletedStopChecked { get; set; }
        public bool IgnoreAdminCodeIfNotInOnCubeChecked { get; set; }
        public string FileExtensionName { get; set; }
    }
}
