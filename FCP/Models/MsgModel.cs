using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace FCP.Models
{
    public sealed class MsgModel
    {
        public string Content { get; set; }
        public string Title { get; set; }
        public PackIconKind Kind { get; set; }
        public SolidColorBrush KindColor { get; set; }
        public bool OKButtonFocus { get; set; }
    }
}
