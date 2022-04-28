using System;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace FCP.Models
{
    class MsgModel
    {
        public string Content { get; set; }
        public string Title { get; set; }
        public PackIconKind Kind { get; set; }
        public Color KindColor { get; set; }
        public Visibility WindowVisibility { get; set; }
        public bool OKButtonFocus { get; set; }
    }
}
