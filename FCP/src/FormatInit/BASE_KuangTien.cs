using FCP.Models;
using FCP.src.Enum;
using FCP.src.FormatControl;
using Helper;
using System.Windows;

namespace FCP.src.FormatInit
{
    class BASE_KuangTien : FormatBase
    {
        public string StatOrBatch { get; set; }
        private FMT_KuangTien _KT { get; set; }

        public override void Init()
        {
            base.Init();
            MainWindowVM.OPDToogle2Checked = true;
        }

        public override void ConvertPrepare()
        {
            SetFileSearchMode(eFileSearchMode.根據檔名開頭);
            if (SettingModel.Format == eFormat.光田醫院OC)
            {
                if (SettingModel.DoseType == eDoseType.種包)
                {
                    //沙鹿
                    //CommonModel.SqlHelper.Execute(@"update PrintFormItem set DeletedYN=1 where RawID in (120180,120195)");

                    //大甲
                    CommonModel.SqlHelper.Execute(@"update PrintFormItem set DeletedYN=1 where RawID in (120156,120172)");
                }
                else
                {
                    //沙鹿
                    //CommonModel.SqlHelper.Execute(@"update PrintFormItem set DeletedYN=0 where RawID in (120180,120195)");

                    //大甲
                    CommonModel.SqlHelper.Execute(@"update PrintFormItem set DeletedYN=0 where RawID in (120156,120172)");
                }
            }
            else if (SettingModel.Format == eFormat.光田醫院JVS)  //磨粉
            {
                base.ConvertPrepare();
                SetPowderRule();
                Start();
                return;
            }
            base.ConvertPrepare();
            SetOPDRule();
            SetStatRule("uds3200");
            SetBatchRule("uds9100");
            Start();
        }

        public override void Converter()
        {
            base.Converter();
            if (_KT == null)
                _KT = new FMT_KuangTien();
            _KT.StatOrBatch = this.StatOrBatch;
            var result = _KT.MethodShunt();
            Result(result, true);
        }

        public override UILayout SetUILayout(UILayout UI)
        {
            UI.Title = SettingModel.Format == eFormat.光田醫院OC ? "光田醫院 > OnCube" : "光田醫院 > JVServer";
            UI.IP1Title = SettingModel.Format == eFormat.光田醫院OC ? "門   診" : "輸入路徑1";
            UI.IP2Title = SettingModel.Format == eFormat.光田醫院OC ? "輸入路徑2" : "磨粉";
            UI.IP3Title = SettingModel.Format == eFormat.光田醫院OC ? "住   院" : "輸入路徑3";
            UI.OPDToogle1 = SettingModel.Format == eFormat.光田醫院OC ? "門診" : "";
            UI.OPDToogle2 = SettingModel.Format == eFormat.光田醫院OC ? "" : "磨粉";
            UI.OPDToogle3 = "";
            UI.OPDToogle4 = "";
            UI.IP1Enabled = SettingModel.Format == eFormat.光田醫院OC;
            UI.IP2Enabled = !(SettingModel.Format == eFormat.光田醫院OC);
            UI.IP3Enabled = SettingModel.Format == eFormat.光田醫院OC;
            UI.UDVisibility = SettingModel.Format == eFormat.光田醫院OC ? Visibility.Visible : Visibility.Hidden;
            UI.OPD1Visibility = SettingModel.Format == eFormat.光田醫院OC ? Visibility.Visible : Visibility.Hidden;
            UI.OPD2Visibility = SettingModel.Format == eFormat.光田醫院OC ? Visibility.Hidden : Visibility.Visible;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
            return UI;
        }
    }
}
