using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;

namespace FCP.MVVM.Models
{
    class AdvancedSettingsModel
    {
        public SolidColorBrush Page1Background { get; set; }
        public SolidColorBrush Page1Foreground { get; set; }
        public SolidColorBrush Page2Background { get; set; }
        public SolidColorBrush Page2Foreground { get; set; }
        public Visibility Visibility { get; set; }
    }
}
