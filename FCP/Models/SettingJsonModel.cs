using System.Collections.Generic;
using FCP.src.Enum;

namespace FCP.Models
{
    public sealed class SettingJsonModel
    {
        public string InputDirectory1 { get; set; } = string.Empty;
        public string InputDirectory2 { get; set; } = string.Empty;
        public string InputDirectory3 { get; set; } = string.Empty;
        public string InputDirectory4 { get; set; } = string.Empty;
        public string InputDirectory5 { get; set; } = string.Empty;
        public string InputDirectory6 { get; set; } = string.Empty;
        public string OutputDirectory { get; set; } = string.Empty;
        public List<string> FileExtensionNames { get; set; } = new List<string>() { "txt" };
        public bool AutoStart { get; set; } = false;
        public eFormat Format { get; set; } = eFormat.JVS;
        public int Speed { get; set; } = 500;
        public ePackMode PackMode { get; set; } = ePackMode.正常;
        public List<string> NeedToFilterAdminCode { get; set; } = new List<string>();
        public eDoseType DoseType { get; set; } = eDoseType.餐包;
        public string OutputSpecialAdminCode { get; set; } = string.Empty;
        public eDepartment StatOrBatch { get; set; } = eDepartment.Stat;
        public string CrossDayAdminCode { get; set; } = string.Empty;
        public List<string> NeedToFilterMedicineCode { get; set; } = new List<string>();
        public bool UseStatAndBatchOption { get; set; } = false;
        public bool MinimizeWindowWhenProgramStart { get; set; } = false;
        public bool ShowCloseAndMinimizeButton { get; set; } = false;
        public bool ShowXY { get; set; } = false;
        public bool FilterMedicineCode { get; set; } = false;
        public bool OnlyCanisterIn { get; set; } = false;
        public bool WhenCompeletedMoveFile { get; set; } = true;
        public bool WhenCompeletedStop { get; set; } = false;
        public bool IgnoreAdminCodeIfNotInOnCube { get; set; } = false;
        public List<ETCInfo> ETCData { get; set; } = new List<ETCInfo>();
    }

    public sealed class RandomInfo
    {
        public string No { get; set; }
        public string JVServer { get; set; }
        public string OnCube { get; set; }
    }

    public sealed class ETCInfo
    {
        public int ETCIndex { get; set; }
        public int PrescriptionParameterIndex { get; set; }
        public string Format { get; set; }
    }
}
