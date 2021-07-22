using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;

namespace FCP
{
    /// <summary>
    /// MsgB.xaml 的互動邏輯
    /// </summary>
    public partial class MsgB : Window
    {
        public _Color Color = new _Color();

        [DllImport("User32.dll")]
        public static extern bool MessageBeep(uint uType);

        public MsgB()
        {
            InitializeComponent();
        }

        public void Show(string c, string t, string i, Color color)
        {
            this.Visibility = Visibility.Visible;
            MsgBIcon.Kind = (MaterialDesignThemes.Wpf.PackIconKind)Enum.Parse(typeof(MaterialDesignThemes.Wpf.PackIconKind), i);
            Title_textblock.Text = t;
            MsgBContent.Text = c;
            MsgBIcon.Foreground = new SolidColorBrush((Color)color);
            OK_button.Focus();
            MessageBeep(1);
            this.ShowDialog();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void OK_button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = false;
        }

        private void Close_button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close_button_Click(null, null);
        }
    }

    public class _Color
    {
        public Color Error = Color.FromRgb(255, 82, 85);
        public Color Warning = Color.FromRgb(225, 219, 96);
        public Color Information = Color.FromRgb(0, 100, 213);
    }
}
