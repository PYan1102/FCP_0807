using System.Windows;

namespace FCP.Models
{
    public sealed class WindowPositionModel
    {
        public int WindowX { get; set; } = 0;
        public int WindowY { get; set; } = 0;
        public bool WindowXEnabled { get; set; } = true;
        public bool WindowYEnabled { get; set; } = true;
        public Visibility Visibility { get; set; } = Visibility.Visible;
    }
}
