using System.Windows;
using System.Windows.Media;
using FCP.src.Enum;

namespace FCP.Models
{
    public sealed class MainWindowModel
    {
        public Visibility Visibility { get; set; }
        public bool Focusable { get; set; } = true;
        public string WindowTitle { get; set; }
        public string InputDirectory1Title { get; set; }
        public string InputDirectory2Title { get; set; }
        public string InputDirectory3Title { get; set; }
        public string InputDirectory4Title { get; set; }
        public string InputDirectory5Title { get; set; }
        public string InputDirectory6Title { get; set; }
        public string InputDirectory1 { get; set; }
        public string InputDirectory2 { get; set; }
        public string InputDirectory3 { get; set; }
        public string InputDirectory4 { get; set; }
        public string InputDirectory5 { get; set; }
        public string InputDirectory6 { get; set; }
        public bool InputDirectory1Enabled { get; set; }
        public bool InputDirectory2Enabled { get; set; }
        public bool InputDirectory3Enabled { get; set; }
        public bool InputDirectory4Enabled { get; set; }
        public bool InputDirectory5Enabled { get; set; }
        public bool InputDirectory6Enabled { get; set; }
        public string OutputDirectoryTitle { get; set; } = "輸出路徑";
        public string OutputDirectory { get; set; }
        public bool OutputDirectoryEnabled { get; set; }
        public Visibility UDButtonVisibility { get; set; }
        public string OPDToogle1 { get; set; }
        public string OPDToogle2 { get; set; }
        public string OPDToogle3 { get; set; }
        public string OPDToogle4 { get; set; }
        public bool OPDToogle1Checked { get; set; }
        public bool OPDToogle2Checked { get; set; }
        public bool OPDToogle3Checked { get; set; }
        public bool OPDToogle4Checked { get; set; }
        public bool OPDToogle1Enabled { get; set; }
        public bool OPDToogle2Enabled { get; set; }
        public bool OPDToogle3Enabled { get; set; }
        public bool OPDToogle4Enabled { get; set; }
        public Visibility OPDToogle1Visibility { get; set; }
        public Visibility OPDToogle2Visibility { get; set; }
        public Visibility OPDToogle3Visibility { get; set; }
        public Visibility OPDToogle4Visibility { get; set; }
        public bool OPDEnabled { get; set; }
        public bool UDEnabled { get; set; }
        public bool StopEnabled { get; set; }
        public bool SaveEnabled { get; set; }
        public bool StatChecked { get; set; }
        public bool BatchChecked { get; set; }
        public bool StatEnabled { get; set; }
        public bool BatchEnabled { get; set; }
        public Visibility StatVisibility { get; set; }
        public Visibility BatchVisibility { get; set; }
        public Visibility SplitEachMealVisibility { get; set; }
        public bool IsAutoStartChecked { get; set; }
        public eDoseType DoseType { get; set; }
        public string WindowX { get; set; }
        public string WindowY { get; set; }
        public bool WindowXEnabled { get; set; }
        public bool WindowYEnabled { get; set; }
        public Visibility WindowXVisibility { get; set; }
        public Visibility WindowYVisibility { get; set; }
        public Visibility MinimumAndCloseVisibility { get; set; }
        public float OPDOpacity { get; set; } = 1;
        public float UDOpacity { get; set; } = 1;
        public SolidColorBrush OPDBacground { get; set; } = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        public SolidColorBrush UDBackground { get; set; } = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        public string ProgressBoxContent { get; set; }

        public class ToogleModel
        {
            public bool Toogle1 { get; set; }
            public bool Toogle2 { get; set; }
            public bool Toogle3 { get; set; }
            public bool Toogle4 { get; set; }
        }
    }
}
