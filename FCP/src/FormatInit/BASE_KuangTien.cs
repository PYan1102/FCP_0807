﻿using FCP.src.Enum;
using FCP.src.FormatControl;
using FCP.MVVM.ViewModels.GetConvertFile;
using Helper;
using System.Windows;

namespace FCP.src.FormatInit
{
    class BASE_KuangTien : FunctionCollections
    {
        public string StatOrBatch { get; set; }
        private FMT_KuangTien _KT { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindowVM.OPDToogle2Checked = true;
            InitFindFileMode(eFindFileMode.根據檔名開頭);
        }

        public override void ConvertPrepare(bool isOPD)
        {
            if (SettingsModel.Mode == eFormat.光田醫院TOC)
            {
                if (SettingsModel.DoseType == eDoseType.種包)
                {
                    //沙鹿
                    //MSSql.RunSQL(@"update PrintFormItem set DeletedYN=1 where RawID in (120180,120195)");

                    //大甲
                    MSSql.RunSQL(@"update PrintFormItem set DeletedYN=1 where RawID in (120156,120172)");
                }
                else
                {
                    //沙鹿
                    //MSSql.RunSQL(@"update PrintFormItem set DeletedYN=0 where RawID in (120180,120195)");

                    //大甲
                    MSSql.RunSQL(@"update PrintFormItem set DeletedYN=0 where RawID in (120156,120172)");
                }
            }
            else if (SettingsModel.Mode == eFormat.光田醫院TJVS)  //磨粉
            {
                base.ConvertPrepare(isOPD);
                SetIntoProperty(isOPD);
                FindFile.SetPowderDefault();
                GetFileAsync();
                return;
            }
            base.ConvertPrepare(isOPD);
            SetOPDRule(nameof(DefaultEnum.Default));
            SetUDStatRule("uds3200");
            SetUDBatchRule("uds9100");
            SetIntoProperty(isOPD);
            GetFileAsync();
        }

        public override void SetConvertInformation()
        {
            base.SetConvertInformation();
            if (_KT == null)
                _KT = new FMT_KuangTien();
            _KT.StatOrBatch = this.StatOrBatch;
            var result = _KT.MethodShunt();
            Result(result, true);
        }

        public override UILayout SetUILayout(UILayout UI)
        {
            UI.Title = SettingsModel.Mode == eFormat.光田醫院TOC ? "光田醫院 > OnCube" : "光田醫院 > JVServer";
            UI.IP1Title = SettingsModel.Mode == eFormat.光田醫院TOC ? "門   診" : "輸入路徑1";
            UI.IP2Title = SettingsModel.Mode == eFormat.光田醫院TOC ? "輸入路徑2" : "磨粉";
            UI.IP3Title = SettingsModel.Mode == eFormat.光田醫院TOC ? "住   院" : "輸入路徑3";
            UI.OPDToogle1 = SettingsModel.Mode == eFormat.光田醫院TOC ? "門診" : "";
            UI.OPDToogle2 = SettingsModel.Mode == eFormat.光田醫院TOC ? "" : "磨粉";
            UI.OPDToogle3 = "";
            UI.OPDToogle4 = "";
            UI.IP1Enabled = SettingsModel.Mode == eFormat.光田醫院TOC;
            UI.IP2Enabled = !(SettingsModel.Mode == eFormat.光田醫院TOC);
            UI.IP3Enabled = SettingsModel.Mode == eFormat.光田醫院TOC;
            UI.UDVisibility = SettingsModel.Mode == eFormat.光田醫院TOC ? Visibility.Visible : Visibility.Hidden;
            UI.OPD1Visibility = SettingsModel.Mode == eFormat.光田醫院TOC ? Visibility.Visible : Visibility.Hidden;
            UI.OPD2Visibility = SettingsModel.Mode == eFormat.光田醫院TOC ? Visibility.Hidden : Visibility.Visible;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
            return UI;
        }
    }
}
