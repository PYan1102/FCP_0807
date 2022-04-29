using System;
using System.Collections.Generic;
using FCP.src.Enum;

namespace FCP.Models
{
    public class SettingModel
    {
        public string InputDirectory1 { get; set; } = string.Empty;
        public string InputDirectory2 { get; set; } = string.Empty;
        public string InputDirectory3 { get; set; } = string.Empty;
        public string InputDirectory4 { get; set; } = string.Empty;
        public string InputDirectory5 { get; set; } = string.Empty;
        public string InputDirectory6 { get; set; } = string.Empty;
        public string OutputDirectory { get; set; } = string.Empty;
        public string FileExtensionName { get; set; } = "txt";
        public bool AutoStart { get; set; } = false;
        public eFormat Format { get; set; } = eFormat.JVS;
        public int Speed { get; set; } = 500;
        public ePackMode PackMode { get; set; } = ePackMode.正常;
        public List<string> FilterAdminCode { get; set; } = new List<string>();
        public List<RandomInfo> ExtraRandom { get; set; } = new List<RandomInfo>();
        public eDoseType DoseType { get; set; } = eDoseType.餐包;
        public string OutputSpecialAdminCode { get; set; } = string.Empty;
        public eDepartment StatOrBatch { get; set; } = eDepartment.Stat;
        public string CutTime { get; set; } = string.Empty;
        public string CrossDayAdminCode { get; set; } = string.Empty;
        public List<string> FilterMedicineCode { get; set; } = new List<string>();
        public bool UseStatOrBatch { get; set; } = false;
        public bool WindowMinimize { get; set; } = false;
        public bool ShowWindowOperationButton { get; set; } = false;
        public bool ShowXYParameter { get; set; } = false;
        public bool UseFilterMedicineCode { get; set; } = false;
        public bool FilterNoCanister { get; set; } = false;
        public bool MoveSourceFileToBackupDirectoryWhenDone { get; set; } = true;
        public bool StopWhenDone { get; set; } = false;
        public bool FiterNoCanister { get; internal set; }
    }

    public class RandomInfo
    {
        public string No { get; set; }
        public string JVServer { get; set; }
        public string OnCube { get; set; }
    }
}
