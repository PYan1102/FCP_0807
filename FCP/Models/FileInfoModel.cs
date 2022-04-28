using System;
using FCP.src.Enum;

namespace FCP.Models
{
    public class FileInfoModel
    {
        public static string SourceFilePath { get; set; }
        public static string InputDirectory { get; set; }
        public static DateTime CurrentDateTime { get; set; }
        public static eDepartment Department { get; set; }
    }
}