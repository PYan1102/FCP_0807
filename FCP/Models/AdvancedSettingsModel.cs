using System.Windows.Media;
using System.Windows;

namespace FCP.Models
{
    public class AdvancedSettingsModel
    {
        public SolidColorBrush Page1Background { get; set; }
        public SolidColorBrush Page1Foreground { get; set; }
        public SolidColorBrush Page2Background { get; set; }
        public SolidColorBrush Page2Foreground { get; set; }
        public Visibility Visibility { get; set; }
    }
}
