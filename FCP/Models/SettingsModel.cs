using System;
using System.Collections.Generic;
using FCP.src.Enum;

namespace FCP.Models
{
    public class SettingsModel
    {
        public string InputPath1 { get; set; }
        public string InputPath2 { get; set; }
        public string InputPath3 { get; set; }
        public string OutputPath { get; set; }
        public string FileExtensionName { get; set; }
        public bool EN_AutoStart { get; set; }
        public eFormat Mode { get; set; }
        public int Speed { get; set; }
        public ePackMode PackMode { get; set; }
        public List<string> FilterAdminCode { get; set; }
        public string ExtraRandom { get; set; }
        public eDoseType DoseType { get; set; }
        public List<string> OutputSpecialAdminCode { get; set; }
        public string StatOrBatch { get; set; }
        public string CutTime { get; set; }
        public List<string> CrossDayAdminCode { get; set; }
        public List<string> FilterMedicineCode { get; set; }
        public bool EN_StatOrBatch { get; set; }
        public bool EN_WindowMinimumWhenOpen { get; set; }
        public bool EN_ShowControlButton { get; set; }
        public bool EN_ShowXY { get; set; }
        public bool EN_FilterMedicineCode { get; set; }
        public bool EN_OnlyCanisterIn { get; set; }
        public bool EN_WhenCompeletedMoveFile { get; set; }
        public bool EN_WhenCompeletedStop { get; set; }
    }
}
