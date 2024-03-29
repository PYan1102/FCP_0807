﻿using FCP.Models;
using FCP.src.Enum;
using FCP.src.FormatLogic;
using FCP.src.MessageManager;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows;

namespace FCP.src.FormatInit
{
    class BASE_KuangTien : ConvertBase
    {
        public BASE_KuangTien(bool? daJia)
        {
            _daJia = daJia;
        }

        private FMT_KuangTien _format;
        private bool? _daJia;

        public override void Init()
        {
            base.Init();
            if (SettingModel.Format == eFormat.光田醫院_大甲OC || SettingModel.Format == eFormat.光田醫院_沙鹿OC)
            {
                WeakReferenceMessenger.Default.Send(new SetMainWindowToogleCheckedChangeMessage(new MainWindowModel.ToggleModel() { Toggle1 = true }));
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new SetMainWindowToogleCheckedChangeMessage(new MainWindowModel.ToggleModel() { Toggle2 = true }));
            }
        }

        public override ActionResult PrepareStart()
        {
            SetFileSearchMode(eFileSearchMode.根據檔名開頭);
            if (SettingModel.Format == eFormat.光田醫院_大甲OC || SettingModel.Format == eFormat.光田醫院_沙鹿OC)
            {
                if (SettingModel.DoseType == eDoseType.種包)
                {
                    CommonModel.SqlHelper.Execute((bool)_daJia ? "update PrintFormItem set DeletedYN = 1 where RawID in (120156,120172)" : "update PrintFormItem set DeletedYN = 1 where RawID in (120180,120195)");
                }
                else
                {
                    CommonModel.SqlHelper.Execute((bool)_daJia ? "update PrintFormItem set DeletedYN = 0 where RawID in (120156, 120172)" : "update PrintFormItem set DeletedYN=0 where RawID in (120180,120195)");
                }
            }
            else if (SettingModel.Format == eFormat.光田醫院JVS)  //磨粉
            {
                SetPowderRule();
                return base.PrepareStart();
            }
            SetOPDRule();
            SetStatRule("uds3200");
            SetBatchRule("uds9100");
            return base.PrepareStart();
        }

        public override void Converter()
        {
            _format = _format ?? new FMT_KuangTien(_daJia);
            var result = _format.DepartmentShunt();
            Result(result, true);
        }

        public override MainUILayoutModel SetUILayout(MainUILayoutModel UI)
        {
            bool oncube = SettingModel.Format == eFormat.光田醫院_大甲OC || SettingModel.Format == eFormat.光田醫院_沙鹿OC;
            UI.Title = SettingModel.Format == eFormat.光田醫院_大甲OC ? "光田醫院 大甲" : "光田醫院 沙鹿";
            UI.IP1Title = oncube ? "門診" : UI.IP1Title;
            UI.IP2Title = oncube ? UI.IP2Title : "磨粉";
            UI.IP1Enabled = oncube;
            UI.IP2Enabled = !oncube;
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
