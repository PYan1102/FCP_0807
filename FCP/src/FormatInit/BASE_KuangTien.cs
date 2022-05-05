using FCP.Models;
using FCP.src.Enum;
using FCP.src.FormatControl;
using System.Windows;

namespace FCP.src.FormatInit
{
    class BASE_KuangTien : FormatBase
    {
        public string StatOrBatch { get; set; }
        private FMT_KuangTien _format;

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
            _format = _format ?? new FMT_KuangTien();
            var result = _format.DepartmentShunt();
            Result(result, true);
        }

        public override MainUILayoutModel SetUILayout(MainUILayoutModel UI)
        {
            bool oncube = SettingModel.Format == eFormat.光田醫院OC;
            UI.Title = "光田醫院";
            UI.IP1Title = oncube ? "門診" : UI.IP1Title;
            UI.IP2Title = oncube ? UI.IP2Title : "磨粉";
            UI.IP1Enabled = oncube;
            UI.IP2Enabled = !(oncube);
            UI.IP3Enabled = false;
            UI.IP4Enabled = false;
            UI.IP5Enabled = oncube;
            UI.IP6Enabled = oncube;
            UI.OPDToogle1 = oncube ? "門診" : string.Empty; ;
            UI.OPDToogle2 = oncube ? string.Empty : "磨粉";
            UI.OPDToogle3 = string.Empty;
            UI.OPDToogle4 = string.Empty;
            UI.UDVisibility = oncube ? Visibility.Visible : Visibility.Hidden;
            UI.OPD1Visibility = oncube ? Visibility.Visible : Visibility.Hidden;
            UI.OPD2Visibility = oncube ? Visibility.Hidden : Visibility.Visible;
            UI.OPD3Visibility = Visibility.Hidden;
            UI.OPD4Visibility = Visibility.Hidden;
            return UI;
        }
    }
}
