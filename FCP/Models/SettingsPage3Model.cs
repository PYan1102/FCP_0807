using FCP.src.Enum;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace FCP.Models
{
    public sealed class SettingPage3Model
    {
        public ObservableCollection<ETCData> ETCData { get; set; }
        public int DataGridSelectedIndex { get; set; }
    }

    public sealed class ETCData
    {
        public List<string> ETC { get; set; }
        public int ETCIndex { get; set; } = 0;
        public List<string> PrescriptionParameters { get; set; }
        public int PrescriptionParameterIndex { get; set; } = 0;
        public string Format { get; set; } = "{0}";
    }
}
