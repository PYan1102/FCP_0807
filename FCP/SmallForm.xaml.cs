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
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Threading;
using System.Runtime.InteropServices;

namespace FCP
{
    /// <summary>
    /// SmallForm.xaml 的互動邏輯
    /// </summary>
    public partial class SmallForm : Window
    {
        MsgB Msg = new MsgB();
        public Settings Settings;
        SolidColorBrush DeepBlue = new SolidColorBrush((Color)(Color.FromRgb(17, 68, 109)));
        SolidColorBrush ShinyBlue = new SolidColorBrush((Color)(Color.FromRgb(9, 225, 255)));
        SolidColorBrush White = new SolidColorBrush((Color)(Color.FromRgb(255, 255, 255)));
        SolidColorBrush Red = new SolidColorBrush((Color)Color.FromRgb(255, 82, 85));
        public MainWindow mw;
        public SmallForm(MainWindow m, Settings s)
        {
            InitializeComponent();
            mw = m;
            Settings = s;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void Initialize()
        {
            Settings.Check();
        }

        private void Close_button_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Minimum_button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        public void SetFormLocation(int X, int Y)
        {
            this.Top = Y;
            this.Left = X;
        }

        public void SetFormSize(int width,int hight)
        {
            this.Width = width;
            this.Height = Height;
        }

        public void ChangeLayout()
        {
            if (Settings.Mode == (int)Settings.ModeEnum.小港醫院 | Settings.Mode == (int)Settings.ModeEnum.光田OnCube | Settings.Mode == (int)Settings.ModeEnum.民生醫院 | Settings.Mode == (int)Settings.ModeEnum.義大醫院)
            {
                StartConverter_textblock.Text = "門診F5";
                UD_button.Visibility = Visibility.Visible;
            }
            Combi_raduibutton.Visibility = Settings.Mode == (int)Settings.ModeEnum.光田OnCube ? Visibility.Visible : Visibility.Hidden;
            Multi_raduibutton.Visibility = Settings.Mode == (int)Settings.ModeEnum.光田OnCube ? Visibility.Visible : Visibility.Hidden;
            if (Settings.Mode == (int)Settings.ModeEnum.JVS)
            {
                StartConverter_textblock.Text = "磨粉F5";
                UD_button.Visibility = Visibility.Hidden;
            }
            if (Settings.EN_StatOrBatch)
            {
                Batch_border.Visibility = Visibility.Visible;
                OnTime_border.Visibility = Visibility.Visible;
            }
            else
            {
                Batch_border.Visibility = Visibility.Hidden;
                OnTime_border.Visibility = Visibility.Hidden;
            }
            if (Settings.StatOrBatch == "S")
                OnTime_radiobutton.IsChecked = true;
            else
                Batch_radiobutton.IsChecked = true;
            if (Settings.EN_ShowControlButton)
            {
                Close_button.Visibility = Visibility.Visible;
                Minimum_button.Visibility = Visibility.Visible;
            }
            else
            {
                Close_button.Visibility = Visibility.Hidden;
                Minimum_button.Visibility = Visibility.Hidden;
            }
            StopConverter_button.IsEnabled = false;
            if (Properties.Settings.Default.DoseType == "M")
                Multi_raduibutton.IsChecked = true;
            else
                Combi_raduibutton.IsChecked = true;
        }

        private void ChangeSize_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !StopConverter_button.IsEnabled;
        }

        private void ChangeSize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mw.ChangeSize_Executed(null, null);
        }

        private void StartConverter_CanExecute(object sender,CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !StopConverter_button.IsEnabled;
        }

        private void StartConverter_Executed(object sender,ExecutedRoutedEventArgs e)
        {
            StartConverter_button_Click(null, null);
        }

        public void StartConverter_button_Click(object sender, RoutedEventArgs e)
        {
            mw.btn_OPD_Click(null, null);
            StartConverter_button.IsEnabled = false;
            UD_button.IsEnabled = false;
            StopConverter_button.IsEnabled = true;
            OnTime_border.IsEnabled = false;
            Batch_border.IsEnabled = false;
            Combi_raduibutton.IsEnabled = false;
            Multi_raduibutton.IsEnabled = false;
            StartConverter_button.Background = Red;
            UD_button.Opacity = 0.2;
        }

        private void UDConverter_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !StopConverter_button.IsEnabled;
        }

        private void UDConverter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UD_button_Click(null, null);
        }

        public void UD_button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DoseType = Multi_raduibutton.IsChecked == true ? "M" : "C";
            Properties.Settings.Default.Save();
            if (OnTime_radiobutton.IsChecked == true)
                mw.ChangeUDFormatType("S");
            else
                mw.ChangeUDFormatType("B");
            mw.btn_UD_Click(null, null);
            StartConverter_button.IsEnabled = false;
            UD_button.IsEnabled = false;
            StopConverter_button.IsEnabled = true;
            OnTime_border.IsEnabled = false;
            Batch_border.IsEnabled = false;
            Combi_raduibutton.IsEnabled = false;
            Multi_raduibutton.IsEnabled = false;
            UD_button.Background = Red;
            StartConverter_button.Opacity = 0.2;
        }

        private void StopConverter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StopConverter_button_Click(null, null);
        }

        public void Stop()
        {
            StartConverter_button.IsEnabled = true;
            UD_button.IsEnabled = true;
            StopConverter_button.IsEnabled = false;
            OnTime_border.IsEnabled = true;
            Batch_border.IsEnabled = true;
            Combi_raduibutton.IsEnabled = true;
            Multi_raduibutton.IsEnabled = true;
            StartConverter_button.Background = White;
            UD_button.Background = White;
            StartConverter_button.Opacity = 1;
            UD_button.Opacity = 1;
            //Multi_raduibutton.IsChecked = true;
        }

        public void StopConverter_button_Click(object sender, RoutedEventArgs e)
        {
            mw.btn_Stop_Click(null, null);
            Stop();
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close_button_Click(null, null);
        }

        private void Open_button_Click(object sender, RoutedEventArgs e)
        {
            if (ProgressBox_textbox.Visibility == Visibility.Visible)
            {
                ProgressBox_textbox.Visibility = Visibility.Hidden;
                return;
            }
            ProgressBox_textbox.Visibility = Visibility.Visible;
        }

        private void Clear_button_Click(object sender, RoutedEventArgs e)
        {
            ProgressBoxClear();
        }

        public void ProgressBoxClear()
        {
            ProgressBox_textbox.Clear();
        }

        public void ProgressBoxAdd(string Result)
        {
            ProgressBox_textbox.AppendText(Result);
            ProgressBox_textbox.ScrollToEnd();
        }
    }
}
