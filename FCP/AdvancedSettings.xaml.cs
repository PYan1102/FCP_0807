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
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

namespace FCP
{
    /// <summary>
    /// AdvanedSettings.xaml 的互動邏輯
    /// </summary>
    public partial class AdvancedSettings : Window
    {
        MsgB Msg = new MsgB();
        ObservableCollection<JVServerRandomclass> JVSRclass = new ObservableCollection<JVServerRandomclass>();
        Settings Settings;
        MainWindow mw;
        List<string> ConvertFormatList = new List<string>() { "JVServer To OnCube", "創聖 To OnCube", "醫聖 To OnCube", "小港醫院 To OnCube", "光田醫院 To OnCube" ,"光田醫院 To JVServer", "民生醫院 To OnCube",
        "宏彥診所 To OnCube", "義大醫院 To OnCube", "長庚醫院磨粉 To JVServer", "長庚醫院 To OnCube", "台北看守所 To OnCube", "仁康醫院 To OnCube", "方鼎 To OnCube"};
        List<string> FilterSpecialList = new List<string>();
        List<string> UseSpecialList = new List<string>();
        List<string> FilterMedicineCode = new List<string>();
        Log log;
        SolidColorBrush Yellow = new SolidColorBrush((Color)Color.FromRgb(244, 208, 63));
        SolidColorBrush White = new SolidColorBrush((Color)Color.FromRgb(255, 255, 255));
        SolidColorBrush DeepBlue = new SolidColorBrush((Color)Color.FromRgb(29, 111, 177));
        public enum PackModeEnum
        {
            正常 = 0, 使用特殊 = 1, 過濾特殊 = 2
        }

        public AdvancedSettings(MainWindow m, Settings s, Log l)
        {
            InitializeComponent();
            Settings = s;
            log = l;
            mw = m;
        }

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void Title_grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ConvertFormat_combobox.ItemsSource = ConvertFormatList;
                Settings.Check();
                SearchFrequency_textbox.Text = Settings.Speed.ToString();
                switch (Settings.Mode)
                {
                    case (int)Settings.ModeEnum.JVS:
                        ConvertFormat_combobox.SelectedIndex = 0;
                        break;
                    case (int)Settings.ModeEnum.創聖:
                        ConvertFormat_combobox.SelectedIndex = 1;
                        break;
                    case (int)Settings.ModeEnum.醫聖:
                        ConvertFormat_combobox.SelectedIndex = 2;
                        break;
                    case (int)Settings.ModeEnum.小港醫院:
                        ConvertFormat_combobox.SelectedIndex = 3;
                        break;
                    case (int)Settings.ModeEnum.光田OnCube:
                        ConvertFormat_combobox.SelectedIndex = 4;
                        break;
                    case (int)Settings.ModeEnum.光田JVS:
                        ConvertFormat_combobox.SelectedIndex = 5;
                        break;
                    case (int)Settings.ModeEnum.民生醫院:
                        ConvertFormat_combobox.SelectedIndex = 6;
                        break;
                    case (int)Settings.ModeEnum.宏彥診所:
                        ConvertFormat_combobox.SelectedIndex = 7;
                        break;
                    case (int)Settings.ModeEnum.義大醫院:
                        ConvertFormat_combobox.SelectedIndex = 8;
                        break;
                    case (int)Settings.ModeEnum.長庚磨粉:
                        ConvertFormat_combobox.SelectedIndex = 9;
                        break;
                    case (int)Settings.ModeEnum.長庚醫院:
                        ConvertFormat_combobox.SelectedIndex = 10;
                        break;
                    case (int)Settings.ModeEnum.台北看守所:
                        ConvertFormat_combobox.SelectedIndex = 11;
                        break;
                    case (int)Settings.ModeEnum.仁康醫院:
                        ConvertFormat_combobox.SelectedIndex = 12;
                        break;
                    case (int)Settings.ModeEnum.方鼎:
                        ConvertFormat_combobox.SelectedIndex = 13;
                        break;
                }
                if (Settings.DoseMode == "M")
                    MultiDose_radiobutton.IsChecked = true;
                else
                    CombiDose_radiobutton.IsChecked = true;
                SpecialAdminTimeOutput_textbox.Text = "";
                Settings.OppositeAdminCode.ForEach(x => SpecialAdminTimeOutput_textbox.Text += $"{x},");
                SpecialAdminTimeOutput_textbox.Text = SpecialAdminTimeOutput_textbox.Text.TrimEnd(',');
                var v = from s in Settings.AdminCodeFilter
                        where s.Trim() != ""
                        orderby s
                        select s;
                var v1 = from s in Settings.AdminCodeUse
                         where s.Trim() != ""
                         orderby s
                         select s;
                FilterSpecialList = v.ToList();
                UseSpecialList = v1.ToList();
                if (Settings.PackMode == (int)PackModeEnum.正常)
                {
                    UseNormalPack_radiobutton.IsChecked = true;
                    PackFunction_grid.Visibility = Visibility.Hidden;
                }
                else if (Settings.PackMode == (int)PackModeEnum.過濾特殊)
                {
                    PackFunction_grid.Visibility = Visibility.Visible;
                    FilterSpecialAdminTimePack_radiobutton.IsChecked = true;
                    PackFunctionAdminTime_combobox.ItemsSource = FilterSpecialList;
                }
                else
                {
                    PackFunction_grid.Visibility = Visibility.Visible;
                    UseSpecialAdminTimePack_radiobutton.IsChecked = true;
                    PackFunctionAdminTime_combobox.ItemsSource = UseSpecialList;
                }
                PackFunctionAdminTime_combobox.SelectedIndex = 0;
                CutTime_textbox.Text = Settings.CutTime;
                CrossAdminTime_textbox.Text = "";
                Settings.CrossDayAdminCode.ForEach(x => CrossAdminTime_textbox.Text += $"{x},");
                CrossAdminTime_textbox.Text = CrossAdminTime_textbox.Text.TrimEnd(',');
                txt_FilterMedicineCode.Text = "";
                FilterMedicineCode.Clear();
                foreach (string s in Settings.FilterMedicineCode)
                {
                    if (s.Trim() == "")
                        continue;
                    FilterMedicineCode.Add(s);
                }
                cbo_FilterMedicineCode.ItemsSource = FilterMedicineCode;
                JVSRclass.Clear();
                string[] ExtraRandom = Settings.ExtraRandom.Split(',');
                if (ExtraRandom.Length >= 1 & ExtraRandom[0].Trim() != "")
                {
                    foreach (string s in ExtraRandom)
                    {
                        string No = s.Substring(0, s.IndexOf(":"));
                        string JVServer = s.Substring(s.IndexOf(":") + 1, s.IndexOf("&") - s.IndexOf(":") - 1);
                        string OnCube = s.Substring(s.IndexOf("&") + 1, s.Length - s.IndexOf("&") - 1);
                        JVSRclass.Add(new JVServerRandomclass
                        {
                            No = No,
                            JVServer = JVServer,
                            OnCube = OnCube
                        });
                    }
                }
                txt_DeputyFileName.Text = Settings.DeputyFileName.Substring(2);
                tgl_OnTimeAndBatch.IsChecked = Settings.EN_StatOrBatch;
                tgl_MinimizeTheWindow.IsChecked = Settings.EN_WindowMinimumWhenOpen;
                tgl_ShowCloseAndMinimumButton.IsChecked = Settings.EN_ShowControlButton;
                tgl_ShowXY.IsChecked = Settings.EN_ShowXY;
                tgl_FilterMedicineCode.IsChecked = Settings.EN_FilterMedicineCode;
                tgl_OnlyCanisterIn.IsChecked = Settings.EN_OnlyCanisterIn;
                tgl_OnlyCanisterIn.Visibility = Settings.EN_FilterMedicineCode ? Visibility.Visible : Visibility.Hidden;
                txt_OnlyCanisterIn.Visibility = Settings.EN_FilterMedicineCode ? Visibility.Visible : Visibility.Hidden;
                JVServerRandomSetting_datagrid.ItemsSource = JVSRclass;
                Page1_button_Click(null, null);
            }
            catch (Exception a)
            {
                Msg.Show(a.ToString(), "錯誤", "Error", Msg.Color.Error);
                log.Write(a.ToString());
            }
        }

        private void Close_button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            Msg.Visibility = Visibility.Hidden;
            mw.Activate();
        }

        private void JVServerRandomAdd_Click(object sender, RoutedEventArgs e)
        {
            if (JVServerRandomSetting_datagrid.Items.Count <= 5)
            {
                int Index = JVServerRandomSetting_datagrid.Items.Count;
                JVSRclass.Add(new JVServerRandomclass
                {
                    No = Index.ToString(),
                    JVServer = "1",
                    OnCube = "1"
                });
                JVServerRandomSetting_datagrid.SelectedIndex = Index;
                JVServerRandomSetting_datagrid.ScrollIntoView(JVSRclass[JVSRclass.Count - 1]);
            }
        }

        private void JVServerRandomRemove_Click(object sender, RoutedEventArgs e)
        {
            int Index = JVServerRandomSetting_datagrid.SelectedIndex;
            if (Index >= 0)
            {
                JVSRclass.RemoveAt(Index);
                for (int x = 0; x <= JVSRclass.Count - 1; x++)
                {
                    JVSRclass[x].No = x.ToString();
                }
            }
            JVServerRandomSetting_datagrid.Items.Refresh();
        }

        private void Save_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.Check();
                var ModeTemp = Settings.Mode;
                string DoseType = "";
                string FilterSpecialAdminTime = "";
                string UseSpecialAdminTime = "";
                string Random = "";
                int PackedFunction;
                string FilterMedicineCode = "";
                string Temp = ConvertFormat_combobox.SelectedItem.ToString();
                DoseType = MultiDose_radiobutton.IsChecked == true ? "M" : "C";
                if (FilterSpecialAdminTimePack_radiobutton.IsChecked == true)
                    PackedFunction = (int)PackModeEnum.過濾特殊;
                else if (UseSpecialAdminTimePack_radiobutton.IsChecked == true)
                    PackedFunction = (int)PackModeEnum.使用特殊;
                else
                    PackedFunction = (int)PackModeEnum.正常;
                FilterSpecialList.ForEach(x => FilterSpecialAdminTime += x + ",");
                UseSpecialList.ForEach(x => UseSpecialAdminTime += x + ",");
                if (FilterSpecialAdminTime.Length >= 1)
                {
                    if (FilterSpecialAdminTime.Substring(FilterSpecialAdminTime.Length - 1) == ",")
                        FilterSpecialAdminTime = FilterSpecialAdminTime.Substring(0, FilterSpecialAdminTime.Length - 1);
                }
                if (UseSpecialAdminTime.Length >= 1)
                {
                    if (UseSpecialAdminTime.Substring(UseSpecialAdminTime.Length - 1) == ",")
                        UseSpecialAdminTime = UseSpecialAdminTime.Substring(0, UseSpecialAdminTime.Length - 1);
                }
                if (JVSRclass.Count >= 1)
                {
                    for (int x = 0; x <= JVSRclass.Count - 1; x++)
                    {
                        Random += JVSRclass[x].No + ":";
                        Random += JVSRclass[x].JVServer + "&";
                        Random += JVSRclass[x].OnCube + ",";
                    }
                    Random = Random.Substring(0, Random.Length - 1);
                }
                this.FilterMedicineCode.ForEach(x => FilterMedicineCode += $"{x},");
                Settings.SaveForm2(txt_DeputyFileName.Text.Trim(), ConvertFormat_combobox.SelectedIndex, Convert.ToInt32(SearchFrequency_textbox.Text.Trim()), PackedFunction, FilterSpecialAdminTime, UseSpecialAdminTime, Random, DoseType,
                    SpecialAdminTimeOutput_textbox.Text.Trim(), CutTime_textbox.Text.Trim(), CrossAdminTime_textbox.Text.Trim(), FilterMedicineCode.TrimEnd(','), (bool)tgl_OnTimeAndBatch.IsChecked, (bool)tgl_MinimizeTheWindow.IsChecked,
                    (bool)tgl_ShowCloseAndMinimumButton.IsChecked, (bool)tgl_ShowXY.IsChecked, (bool)tgl_FilterMedicineCode.IsChecked, (bool)tgl_OnlyCanisterIn.IsChecked);
                Settings.Check();
                if (ModeTemp != (int)Settings.Mode)
                {
                    mw.ClearObject(ModeTemp);
                    mw.Judge(false);
                }
                Msg.Show("儲存完成", "成功", "Information", Msg.Color.Information);
            }
            catch (Exception a)
            {
                Msg.Show(a.ToString(), "錯誤", "Error", Msg.Color.Error);
                log.Write(a.ToString());
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save_button_Click(null, null);
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close_button_Click(null, null);
        }

        private void FilterSpecialAdminTimePack_radiobutton_Checked(object sender, RoutedEventArgs e)
        {
            PackFunction_grid.Visibility = Visibility.Visible;
            PackFunctionAdminTime_combobox.ItemsSource = FilterSpecialList;
            PackFunctionAdminTime_combobox.SelectedIndex = 0;
        }

        private void UseSpecialAdminTimePack_radiobutton_Checked(object sender, RoutedEventArgs e)
        {
            PackFunction_grid.Visibility = Visibility.Visible;
            PackFunctionAdminTime_combobox.ItemsSource = UseSpecialList;
            PackFunctionAdminTime_combobox.SelectedIndex = 0;
        }

        private void UseNormalPack_radiobutton_Checked(object sender, RoutedEventArgs e)
        {
            PackFunction_grid.Visibility = Visibility.Hidden;
        }

        private void PackFunctionAdd_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AdminCode = PackFunctionAdminTime_textbox.Text.Trim();
                if (AdminCode == "")
                {
                    Msg.Show("頻率不能為空白", "輸入錯誤", "Error", Msg.Color.Error);
                    return;
                }
                if ((bool)FilterSpecialAdminTimePack_radiobutton.IsChecked)
                {
                    if (!FilterSpecialList.Contains(AdminCode))
                        FilterSpecialList.Add(AdminCode);
                    else
                        Msg.Show($"此頻率 {AdminCode} 已添加過，請確認", "重複", "Warning", Msg.Color.Error);
                    PackFunctionAdminTime_combobox.ItemsSource = FilterSpecialList;
                }
                else if ((bool)UseSpecialAdminTimePack_radiobutton.IsChecked)
                {
                    if (!UseSpecialList.Contains(AdminCode))
                        UseSpecialList.Add(AdminCode);
                    else
                        Msg.Show($"此頻率 {AdminCode} 已添加過，請確認", "重複", "Warning", Msg.Color.Warning);
                    PackFunctionAdminTime_combobox.ItemsSource = UseSpecialList;
                }
                PackFunctionAdminTime_combobox.Items.Refresh();
                PackFunctionAdminTime_combobox.SelectedIndex = 0;
                PackFunctionAdminTime_textbox.Text = "";
                PackFunctionAdminTime_textbox.Focus();
            }
            catch (Exception a)
            {
                Msg.Show(a.ToString(), "錯誤", "Error", Msg.Color.Error);
                log.Write(a.ToString());
            }
        }

        private void PackFunctionRemove_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PackFunctionAdminTime_combobox.Items.Count == 0)
                    return;
                string AdminCode = PackFunctionAdminTime_combobox.SelectedItem.ToString();
                if ((bool)FilterSpecialAdminTimePack_radiobutton.IsChecked)
                {
                    if (FilterSpecialList.Contains(AdminCode))
                        FilterSpecialList.RemoveAll(x => x == AdminCode);
                    else
                        Msg.Show($"找無此頻率 {AdminCode}，請確認", "", "Error", Msg.Color.Error);
                    PackFunctionAdminTime_combobox.ItemsSource = FilterSpecialList;
                }
                else if ((bool)UseSpecialAdminTimePack_radiobutton.IsChecked)
                {
                    if (UseSpecialList.Contains(AdminCode))
                        UseSpecialList.RemoveAll(x => x == AdminCode);
                    else
                        Msg.Show($"找無此頻率 {AdminCode}，請確認", "", "Error", Msg.Color.Error);
                    PackFunctionAdminTime_combobox.ItemsSource = UseSpecialList;
                }
                PackFunctionAdminTime_combobox.Items.Refresh();
                PackFunctionAdminTime_combobox.SelectedIndex = 0;
            }
            catch (Exception a)
            {
                Msg.Show(a.ToString(), "錯誤", "Error", Msg.Color.Error);
                log.Write(a.ToString());
            }
        }

        private void PackFunctionAdminTime_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                PackFunctionAdd_button_Click(null, null);
        }

        private void Page1_button_Click(object sender, RoutedEventArgs e)
        {
            Page1_border.Visibility = Visibility.Visible;
            Page2_border.Visibility = Visibility.Hidden;
            Page3_border.Visibility = Visibility.Hidden;
            Page1_button.Background = Yellow;
            Page1_button.Foreground = White;
            Page2_button.Background = White;
            Page2_button.Foreground = DeepBlue;
            Page3_button.Background = White;
            Page3_button.Foreground = DeepBlue;
        }

        private void Page2_button_Click(object sender, RoutedEventArgs e)
        {
            Page1_border.Visibility = Visibility.Hidden;
            Page2_border.Visibility = Visibility.Visible;
            Page3_border.Visibility = Visibility.Hidden;
            Page1_button.Background = White;
            Page1_button.Foreground = DeepBlue;
            Page2_button.Background = Yellow;
            Page2_button.Foreground = White;
            Page3_button.Background = White;
            Page3_button.Foreground = DeepBlue;
        }

        private void Page3_button_Click(object sender, RoutedEventArgs e)
        {
            Page1_border.Visibility = Visibility.Hidden;
            Page2_border.Visibility = Visibility.Hidden;
            Page3_border.Visibility = Visibility.Visible;
            Page1_button.Background = White;
            Page1_button.Foreground = DeepBlue;
            Page2_button.Background = White;
            Page2_button.Foreground = DeepBlue;
            Page3_button.Background = Yellow;
            Page3_button.Foreground = White;
        }

        public class JVServerRandomclass
        {
            public string No { get; set; }
            public string JVServer { get; set; }
            public string OnCube { get; set; }
        }

        private void tgl_FilterMedicineCode_Checked(object sender, RoutedEventArgs e)
        {
            tgl_OnlyCanisterIn.Visibility = Visibility.Visible;
            txt_OnlyCanisterIn.Visibility = Visibility.Visible;
        }

        private void tgl_FilterMedicineCode_Unchecked(object sender, RoutedEventArgs e)
        {
            tgl_OnlyCanisterIn.Visibility = Visibility.Hidden;
            txt_OnlyCanisterIn.Visibility = Visibility.Hidden;
        }

        private void btn_FilterMediicneCodeAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txt_FilterMedicineCode.Text.Trim() == "")
                return;
            string Code = txt_FilterMedicineCode.Text.Trim();
            if (!FilterMedicineCode.Contains(Code))
            {
                FilterMedicineCode.Add(Code);
                FilterMedicineCode.Sort();
                cbo_FilterMedicineCode.ItemsSource = FilterMedicineCode;
                cbo_FilterMedicineCode.Items.Refresh();
                txt_FilterMedicineCode.Text = "";
                txt_FilterMedicineCode.Focus();
            }
            else
            {
                Msg.Show($"{Code} 已重複", "重複的藥品代碼", "Error", Msg.Color.Error);
                txt_FilterMedicineCode.Focus();
                txt_FilterMedicineCode.SelectAll();
            }
        }

        private void btn_FilterMedicineCodeRemove_Click(object sender, RoutedEventArgs e)
        {
            if (cbo_FilterMedicineCode.Items.Count == 0)
                return;
            FilterMedicineCode.RemoveAt(cbo_FilterMedicineCode.SelectedIndex);
            cbo_FilterMedicineCode.ItemsSource = FilterMedicineCode;
            cbo_FilterMedicineCode.Items.Refresh();
            cbo_FilterMedicineCode.SelectedIndex = 0;
        }
    }
}
