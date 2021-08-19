using System;
using System.Collections.Generic;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.Models
{
    public class SettingsModel
    {
        public string InputPath1 { get; set; }
        public string InputPath2 { get; set; }
        public string InputPath3 { get; set; }
        public string OutputPath1 { get; set; }
        public string DeputyFileName { get; set; }
        public bool EN_AutoStart { get; set; }
        public Format Mode { get; set; }
        public int Speed { get; set; }
        public PackMode PackMode { get; set; }
        public List<string> AdminCodeFilter { get; set; }
        public List<string> AdminCodeUse { get; set; }
        public string ExtraRandom { get; set; }
        public DoseType DoseType { get; set; }
        public List<string> OppositeAdminCode { get; set; }
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
    }
}
