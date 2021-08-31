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
using FCP.MVVM.Models;
using FCP.MVVM.Control;
using FCP.MVVM.Factory;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.ViewModels;
using FCP.MVVM.Factory.ViewModel;

namespace FCP.MVVM.View
{
    /// <summary>
    /// SmallForm.xaml 的互動邏輯
    /// </summary>
    public partial class SimpleWindow : Window
    {
        private SettingsModel _SettingsModel { get; set; }
        private MainWindowViewModel _MainWindowVM { get; set; }
        private Settings _Settings { get; set; }
        SolidColorBrush DeepBlue = new SolidColorBrush((Color)(Color.FromRgb(17, 68, 109)));
        SolidColorBrush ShinyBlue = new SolidColorBrush((Color)(Color.FromRgb(9, 225, 255)));
        SolidColorBrush White = new SolidColorBrush((Color)(Color.FromRgb(255, 255, 255)));
        SolidColorBrush Red = new SolidColorBrush((Color)Color.FromRgb(255, 82, 85));
        public MainWindow mw;
        public SimpleWindow(MainWindow m)
        {
            InitializeComponent();
            mw = m;
            this.DataContext = SimpleWindowFactory.GenerateSimpleWindowViewModel();
            _Settings = SettingsFactory.GenerateSettingsControl();
            _SettingsModel = SettingsFactory.GenerateSettingsModel();
            _MainWindowVM = MainWindowFactory.GenerateMainWindowViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void Initialize()
        {
            
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Btn_Minimum_Click(object sender, RoutedEventArgs e)
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
            if (_SettingsModel.Mode == Format.小港醫院TOC || _SettingsModel.Mode == Format.光田醫院TOC || _SettingsModel.Mode == Format.民生醫院TOC || _SettingsModel.Mode == Format.義大醫院TOC)
            {
                Txtb_StartConverter.Text = "門診F5";
                Btn_UD.Visibility = Visibility.Visible;
            }
            Rdo_Combi.Visibility = _SettingsModel.Mode == Format.光田醫院TOC ? Visibility.Visible : Visibility.Hidden;
            Rdo_Multi.Visibility = _SettingsModel.Mode == Format.光田醫院TOC ? Visibility.Visible : Visibility.Hidden;
            if (_SettingsModel.Mode == Format.JVS)
            {
                Txtb_StartConverter.Text = "磨粉F5";
                Btn_UD.Visibility = Visibility.Hidden;
            }
            if (_SettingsModel.EN_StatOrBatch)
            {
                Bod_Batch.Visibility = Visibility.Visible;
                Bod_OnTime.Visibility = Visibility.Visible;
            }
            else
            {
                Bod_Batch.Visibility = Visibility.Hidden;
                Bod_OnTime.Visibility = Visibility.Hidden;
            }
            if (_SettingsModel.StatOrBatch == "S")
                Rdo_OnTime.IsChecked = true;
            else
                Rdo_Batch.IsChecked = true;
            if (_SettingsModel.EN_ShowControlButton)
            {
                Btn_Close.Visibility = Visibility.Visible;
                Btn_Minimum.Visibility = Visibility.Visible;
            }
            else
            {
                Btn_Close.Visibility = Visibility.Hidden;
                Btn_Minimum.Visibility = Visibility.Hidden;
            }
            Btn_StopConverter.IsEnabled = false;
            if (Properties.Settings.Default.DoseType == "M")
                Rdo_Multi.IsChecked = true;
            else
                Rdo_Combi.IsChecked = true;
        }

        private void ChangeSize_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Btn_StopConverter.IsEnabled;
        }

        private void ChangeSize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mw.ChangeSize_Executed(null, null);
        }

        private void StartConverter_CanExecute(object sender,CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Btn_StopConverter.IsEnabled;
        }

        private void StartConverter_Executed(object sender,ExecutedRoutedEventArgs e)
        {
            Btn_StartConverter_Click(null, null);
        }

        public void Btn_StartConverter_Click(object sender, RoutedEventArgs e)
        {
            _MainWindowVM.OPDFunc();
            Btn_StartConverter.IsEnabled = false;
            Btn_UD.IsEnabled = false;
            Btn_StopConverter.IsEnabled = true;
            Bod_OnTime.IsEnabled = false;
            Bod_Batch.IsEnabled = false;
            Rdo_Combi.IsEnabled = false;
            Rdo_Multi.IsEnabled = false;
            Btn_StartConverter.Background = Red;
            Btn_UD.Opacity = 0.2;
        }

        private void UDConverter_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Btn_StopConverter.IsEnabled;
        }

        private void UDConverter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Btn_UD_Click(null, null);
        }

        public void Btn_UD_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DoseType = Rdo_Multi.IsChecked == true ? "M" : "C";
            Properties.Settings.Default.Save();
            if (Rdo_OnTime.IsChecked == true)
                mw.ChangeUDFormatType("S");
            else
                mw.ChangeUDFormatType("B");
            _MainWindowVM.UDFunc();
            Btn_StartConverter.IsEnabled = false;
            Btn_UD.IsEnabled = false;
            Btn_StopConverter.IsEnabled = true;
            Bod_OnTime.IsEnabled = false;
            Bod_Batch.IsEnabled = false;
            Rdo_Combi.IsEnabled = false;
            Rdo_Multi.IsEnabled = false;
            Btn_UD.Background = Red;
            Btn_StartConverter.Opacity = 0.2;
        }

        private void StopConverter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Btn_StopConverter_Click(null, null);
        }

        public void Stop()
        {
            Btn_StartConverter.IsEnabled = true;
            Btn_UD.IsEnabled = true;
            Btn_StopConverter.IsEnabled = false;
            Bod_OnTime.IsEnabled = true;
            Bod_Batch.IsEnabled = true;
            Rdo_Combi.IsEnabled = true;
            Rdo_Multi.IsEnabled = true;
            Btn_StartConverter.Background = White;
            Btn_UD.Background = White;
            Btn_StartConverter.Opacity = 1;
            Btn_UD.Opacity = 1;
            //Rdo_Multi.IsChecked = true;
        }

        public void Btn_StopConverter_Click(object sender, RoutedEventArgs e)
        {
            mw.Btn_Stop_Click(null, null);
            Stop();
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Btn_Close_Click(null, null);
        }

        private void Btn_Open_Click(object sender, RoutedEventArgs e)
        {
            if (Txt_ProgressBox.Visibility == Visibility.Visible)
            {
                Txt_ProgressBox.Visibility = Visibility.Hidden;
                return;
            }
            Txt_ProgressBox.Visibility = Visibility.Visible;
        }

        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            ProgressBoxClear();
        }

        public void ProgressBoxClear()
        {
            Txt_ProgressBox.Clear();
        }

        public void ProgressBoxAdd(string Result)
        {
            Txt_ProgressBox.AppendText(Result);
            Txt_ProgressBox.ScrollToEnd();
        }
    }
}
