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
using FCP.MVVM.Factory;
using FCP.MVVM.Models;
using FCP.MVVM.Control;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Helper;

namespace FCP
{
    /// <summary>
    /// AdvanedSettings.xaml 的互動邏輯
    /// </summary>
    public partial class AdvancedSettings : Window
    {
        MsgB Msg = new MsgB();
        ObservableCollection<JVServerRandomclass> JVSRclass = new ObservableCollection<JVServerRandomclass>();
        private Settings _Settings { get; set; }
        private SettingsModel _SettingsModel { get; set; }
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

        public AdvancedSettings(MainWindow m, Log l)
        {
            InitializeComponent();
            log = l;
            mw = m;
            _Settings = SettingsFactory.GenerateSettingsControl();
            _SettingsModel = SettingsFactory.GenerateSettingsModels();
        }

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void Gd_Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Cbo_ConvertFormat.ItemsSource = EnumHelper.ToList<Format>();
                
                Txt_SearchFrequency.Text = _SettingsModel.Speed.ToString();
                switch (_SettingsModel.Mode)
                {
                    case Format.JVS:
                        Cbo_ConvertFormat.SelectedIndex = 0;
                        break;
                    case Format.創聖系統TOC:
                        Cbo_ConvertFormat.SelectedIndex = 1;
                        break;
                    case Format.醫聖系統TOC:
                        Cbo_ConvertFormat.SelectedIndex = 2;
                        break;
                    case Format.小港醫院TOC:
                        Cbo_ConvertFormat.SelectedIndex = 3;
                        break;
                    case Format.光田醫院TOC:
                        Cbo_ConvertFormat.SelectedIndex = 4;
                        break;
                    case Format.光田醫院TJVS:
                        Cbo_ConvertFormat.SelectedIndex = 5;
                        break;
                    case Format.民生醫院TOC:
                        Cbo_ConvertFormat.SelectedIndex = 6;
                        break;
                    case Format.宏彥診所TOC:
                        Cbo_ConvertFormat.SelectedIndex = 7;
                        break;
                    case Format.義大醫院TOC:
                        Cbo_ConvertFormat.SelectedIndex = 8;
                        break;
                    case Format.長庚磨粉TJVS:
                        Cbo_ConvertFormat.SelectedIndex = 9;
                        break;
                    case Format.長庚醫院TOC:
                        Cbo_ConvertFormat.SelectedIndex = 10;
                        break;
                    case Format.台北看守所TOC:
                        Cbo_ConvertFormat.SelectedIndex = 11;
                        break;
                    case Format.仁康醫院TOC:
                        Cbo_ConvertFormat.SelectedIndex = 12;
                        break;
                    case Format.方鼎系統TOC:
                        Cbo_ConvertFormat.SelectedIndex = 13;
                        break;
                    default:
                        throw new Exception($"沒有找到適當的格式 {nameof(_SettingsModel.Mode)}");
                }
                if (_SettingsModel.DoseMode == DoseMode.餐包)
                    Rdo_MultiDose.IsChecked = true;
                else
                    Rdo_CombiDose.IsChecked = true;
                Txt_SpecialAdminTimeOutput.Text = "";
                _SettingsModel.OppositeAdminCode.ForEach(x => Txt_SpecialAdminTimeOutput.Text += $"{x},");
                Txt_SpecialAdminTimeOutput.Text = Txt_SpecialAdminTimeOutput.Text.TrimEnd(',');
                var v = from s in _SettingsModel.AdminCodeFilter
                        where s.Trim() != ""
                        orderby s
                        select s;
                var v1 = from s in _SettingsModel.AdminCodeUse
                         where s.Trim() != ""
                         orderby s
                         select s;
                FilterSpecialList = v.ToList();
                UseSpecialList = v1.ToList();
                if (_SettingsModel.PackMode == PackMode.正常)
                {
                    Rdo_UseNormalPack.IsChecked = true;
                    Gd_PackFunction.Visibility = Visibility.Hidden;
                }
                else if (_SettingsModel.PackMode == PackMode.過濾特殊)
                {
                    Gd_PackFunction.Visibility = Visibility.Visible;
                    Rdo_FilterSpecialAdminTimePack.IsChecked = true;
                    Cbo_PackFunctionAdminTime.ItemsSource = FilterSpecialList;
                }
                else
                {
                    Gd_PackFunction.Visibility = Visibility.Visible;
                    Rdo_UseSpecialAdminTimePack.IsChecked = true;
                    Cbo_PackFunctionAdminTime.ItemsSource = UseSpecialList;
                }
                Cbo_PackFunctionAdminTime.SelectedIndex = 0;
                Txt_CutTime.Text = _SettingsModel.CutTime;
                Txt_CrossAdminTime.Text = "";
                _SettingsModel.CrossDayAdminCode.ForEach(x => Txt_CrossAdminTime.Text += $"{x},");
                Txt_CrossAdminTime.Text = Txt_CrossAdminTime.Text.TrimEnd(',');
                Txt_FilterMedicineCode.Text = "";
                FilterMedicineCode.Clear();
                foreach (string s in _SettingsModel.FilterMedicineCode)
                {
                    if (s.Trim() == "")
                        continue;
                    FilterMedicineCode.Add(s);
                }
                Cbo_FilterMedicineCode.ItemsSource = FilterMedicineCode;
                JVSRclass.Clear();
                string[] ExtraRandom = _SettingsModel.ExtraRandom.Split(',');
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
                Txt_DeputyFileName.Text = _SettingsModel.DeputyFileName.Substring(2);
                Tgl_OnTimeAndBatch.IsChecked = _SettingsModel.EN_StatOrBatch;
                Tgl_MinimizeTheWindow.IsChecked = _SettingsModel.EN_WindowMinimumWhenOpen;
                Tgl_ShowCloseAndMinimumButton.IsChecked = _SettingsModel.EN_ShowControlButton;
                Tgl_ShowXY.IsChecked = _SettingsModel.EN_ShowXY;
                Tgl_FilterMedicineCode.IsChecked = _SettingsModel.EN_FilterMedicineCode;
                Tgl_OnlyCanisterIn.IsChecked = _SettingsModel.EN_OnlyCanisterIn;
                Tgl_OnlyCanisterIn.Visibility = _SettingsModel.EN_FilterMedicineCode ? Visibility.Visible : Visibility.Hidden;
                Txt_OnlyCanisterIn.Visibility = _SettingsModel.EN_FilterMedicineCode ? Visibility.Visible : Visibility.Hidden;
                Dg_JVServerRandomSetting.ItemsSource = JVSRclass;
                Btn_Page1_Click(null, null);
            }
            catch (Exception a)
            {
                Msg.Show(a.ToString(), "錯誤", "Error", Msg.Color.Error);
                log.Write(a.ToString());
            }
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            Msg.Visibility = Visibility.Hidden;
            mw.Activate();
        }

        private void JVServerRandomAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Dg_JVServerRandomSetting.Items.Count <= 5)
            {
                int Index = Dg_JVServerRandomSetting.Items.Count;
                JVSRclass.Add(new JVServerRandomclass
                {
                    No = Index.ToString(),
                    JVServer = "1",
                    OnCube = "1"
                });
                Dg_JVServerRandomSetting.SelectedIndex = Index;
                Dg_JVServerRandomSetting.ScrollIntoView(JVSRclass[JVSRclass.Count - 1]);
            }
        }

        private void JVServerRandomRemove_Click(object sender, RoutedEventArgs e)
        {
            int Index = Dg_JVServerRandomSetting.SelectedIndex;
            if (Index >= 0)
            {
                JVSRclass.RemoveAt(Index);
                for (int x = 0; x <= JVSRclass.Count - 1; x++)
                {
                    JVSRclass[x].No = x.ToString();
                }
            }
            Dg_JVServerRandomSetting.Items.Refresh();
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                var ModeTemp = _SettingsModel.Mode;
                string FilterSpecialAdminTime = "";
                string UseSpecialAdminTime = "";
                string Random = "";
                string FilterMedicineCode = "";
                string Temp = Cbo_ConvertFormat.SelectedItem.ToString();
                DoseMode doseMode = (bool)Rdo_MultiDose.IsChecked ? DoseMode.餐包 : DoseMode.種包;
                PackMode packMode;
                if (Rdo_FilterSpecialAdminTimePack.IsChecked == true)
                    packMode = PackMode.過濾特殊;
                else if (Rdo_UseSpecialAdminTimePack.IsChecked == true)
                    packMode = PackMode.使用特殊;
                else
                    packMode = PackMode.正常;
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
                _Settings.SaveAdvancedSettings(Txt_DeputyFileName.Text.Trim(),
                    (Format)Cbo_ConvertFormat.SelectedIndex,
                    Convert.ToInt32(Txt_SearchFrequency.Text.Trim()),
                    packMode,
                    FilterSpecialAdminTime,
                    UseSpecialAdminTime,
                    Random,
                    doseMode,
                    Txt_SpecialAdminTimeOutput.Text.Trim(),
                    Txt_CutTime.Text.Trim(),
                    Txt_CrossAdminTime.Text.Trim(),
                    FilterMedicineCode.TrimEnd(','),
                    (bool)Tgl_OnTimeAndBatch.IsChecked,
                    (bool)Tgl_MinimizeTheWindow.IsChecked,
                    (bool)Tgl_ShowCloseAndMinimumButton.IsChecked,
                    (bool)Tgl_ShowXY.IsChecked,
                    (bool)Tgl_FilterMedicineCode.IsChecked,
                    (bool)Tgl_OnlyCanisterIn.IsChecked);
                
                if (ModeTemp != _SettingsModel.Mode)
                {
                    mw.ClearObject();
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
            Btn_Save_Click(null, null);
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Btn_Close_Click(null, null);
        }

        private void Rdo_FilterSpecialAdminTimePack_Checked(object sender, RoutedEventArgs e)
        {
            Gd_PackFunction.Visibility = Visibility.Visible;
            Cbo_PackFunctionAdminTime.ItemsSource = FilterSpecialList;
            Cbo_PackFunctionAdminTime.SelectedIndex = 0;
        }

        private void Rdo_UseSpecialAdminTimePack_Checked(object sender, RoutedEventArgs e)
        {
            Gd_PackFunction.Visibility = Visibility.Visible;
            Cbo_PackFunctionAdminTime.ItemsSource = UseSpecialList;
            Cbo_PackFunctionAdminTime.SelectedIndex = 0;
        }

        private void Rdo_UseNormalPack_Checked(object sender, RoutedEventArgs e)
        {
            Gd_PackFunction.Visibility = Visibility.Hidden;
        }

        private void Btn_PackFunctionAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AdminCode = Txt_PackFunctionAdminTime.Text.Trim();
                if (AdminCode == "")
                {
                    Msg.Show("頻率不能為空白", "輸入錯誤", "Error", Msg.Color.Error);
                    return;
                }
                if ((bool)Rdo_FilterSpecialAdminTimePack.IsChecked)
                {
                    if (!FilterSpecialList.Contains(AdminCode))
                        FilterSpecialList.Add(AdminCode);
                    else
                        Msg.Show($"此頻率 {AdminCode} 已添加過，請確認", "重複", "Warning", Msg.Color.Error);
                    Cbo_PackFunctionAdminTime.ItemsSource = FilterSpecialList;
                }
                else if ((bool)Rdo_UseSpecialAdminTimePack.IsChecked)
                {
                    if (!UseSpecialList.Contains(AdminCode))
                        UseSpecialList.Add(AdminCode);
                    else
                        Msg.Show($"此頻率 {AdminCode} 已添加過，請確認", "重複", "Warning", Msg.Color.Warning);
                    Cbo_PackFunctionAdminTime.ItemsSource = UseSpecialList;
                }
                Cbo_PackFunctionAdminTime.Items.Refresh();
                Cbo_PackFunctionAdminTime.SelectedIndex = 0;
                Txt_PackFunctionAdminTime.Text = "";
                Txt_PackFunctionAdminTime.Focus();
            }
            catch (Exception a)
            {
                Msg.Show(a.ToString(), "錯誤", "Error", Msg.Color.Error);
                log.Write(a.ToString());
            }
        }

        private void Btn_PackFunctionRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Cbo_PackFunctionAdminTime.Items.Count == 0)
                    return;
                string AdminCode = Cbo_PackFunctionAdminTime.SelectedItem.ToString();
                if ((bool)Rdo_FilterSpecialAdminTimePack.IsChecked)
                {
                    if (FilterSpecialList.Contains(AdminCode))
                        FilterSpecialList.RemoveAll(x => x == AdminCode);
                    else
                        Msg.Show($"找無此頻率 {AdminCode}，請確認", "", "Error", Msg.Color.Error);
                    Cbo_PackFunctionAdminTime.ItemsSource = FilterSpecialList;
                }
                else if ((bool)Rdo_UseSpecialAdminTimePack.IsChecked)
                {
                    if (UseSpecialList.Contains(AdminCode))
                        UseSpecialList.RemoveAll(x => x == AdminCode);
                    else
                        Msg.Show($"找無此頻率 {AdminCode}，請確認", "", "Error", Msg.Color.Error);
                    Cbo_PackFunctionAdminTime.ItemsSource = UseSpecialList;
                }
                Cbo_PackFunctionAdminTime.Items.Refresh();
                Cbo_PackFunctionAdminTime.SelectedIndex = 0;
            }
            catch (Exception a)
            {
                Msg.Show(a.ToString(), "錯誤", "Error", Msg.Color.Error);
                log.Write(a.ToString());
            }
        }

        private void Txt_PackFunctionAdminTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Btn_PackFunctionAdd_Click(null, null);
        }

        private void Btn_Page1_Click(object sender, RoutedEventArgs e)
        {
            Bod_Page1.Visibility = Visibility.Visible;
            Bod_Page2.Visibility = Visibility.Hidden;
            Bod_Page3.Visibility = Visibility.Hidden;
            Btn_Page1.Background = Yellow;
            Btn_Page1.Foreground = White;
            Btn_Page2.Background = White;
            Btn_Page2.Foreground = DeepBlue;
            Btn_Page3.Background = White;
            Btn_Page3.Foreground = DeepBlue;
        }

        private void Btn_Page2_Click(object sender, RoutedEventArgs e)
        {
            Bod_Page1.Visibility = Visibility.Hidden;
            Bod_Page2.Visibility = Visibility.Visible;
            Bod_Page3.Visibility = Visibility.Hidden;
            Btn_Page1.Background = White;
            Btn_Page1.Foreground = DeepBlue;
            Btn_Page2.Background = Yellow;
            Btn_Page2.Foreground = White;
            Btn_Page3.Background = White;
            Btn_Page3.Foreground = DeepBlue;
        }

        private void Btn_Page3_Click(object sender, RoutedEventArgs e)
        {
            Bod_Page1.Visibility = Visibility.Hidden;
            Bod_Page2.Visibility = Visibility.Hidden;
            Bod_Page3.Visibility = Visibility.Visible;
            Btn_Page1.Background = White;
            Btn_Page1.Foreground = DeepBlue;
            Btn_Page2.Background = White;
            Btn_Page2.Foreground = DeepBlue;
            Btn_Page3.Background = Yellow;
            Btn_Page3.Foreground = White;
        }

        public class JVServerRandomclass
        {
            public string No { get; set; }
            public string JVServer { get; set; }
            public string OnCube { get; set; }
        }

        private void Tgl_FilterMedicineCode_Checked(object sender, RoutedEventArgs e)
        {
            Tgl_OnlyCanisterIn.Visibility = Visibility.Visible;
            Txt_OnlyCanisterIn.Visibility = Visibility.Visible;
        }

        private void Tgl_FilterMedicineCode_Unchecked(object sender, RoutedEventArgs e)
        {
            Tgl_OnlyCanisterIn.Visibility = Visibility.Hidden;
            Txt_OnlyCanisterIn.Visibility = Visibility.Hidden;
        }

        private void Btn_FilterMediicneCodeAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Txt_FilterMedicineCode.Text.Trim() == "")
                return;
            string Code = Txt_FilterMedicineCode.Text.Trim();
            if (!FilterMedicineCode.Contains(Code))
            {
                FilterMedicineCode.Add(Code);
                FilterMedicineCode.Sort();
                Cbo_FilterMedicineCode.ItemsSource = FilterMedicineCode;
                Cbo_FilterMedicineCode.Items.Refresh();
                Txt_FilterMedicineCode.Text = "";
                Txt_FilterMedicineCode.Focus();
            }
            else
            {
                Msg.Show($"{Code} 已重複", "重複的藥品代碼", "Error", Msg.Color.Error);
                Txt_FilterMedicineCode.Focus();
                Txt_FilterMedicineCode.SelectAll();
            }
        }

        private void Btn_FilterMedicineCodeRemove_Click(object sender, RoutedEventArgs e)
        {
            if (Cbo_FilterMedicineCode.Items.Count == 0)
                return;
            FilterMedicineCode.RemoveAt(Cbo_FilterMedicineCode.SelectedIndex);
            Cbo_FilterMedicineCode.ItemsSource = FilterMedicineCode;
            Cbo_FilterMedicineCode.Items.Refresh();
            Cbo_FilterMedicineCode.SelectedIndex = 0;
        }
    }
}
