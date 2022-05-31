using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FCP.src.Enum;
using SqlHelper;
using SqlHelper.Interface;

namespace FCP.Models
{
    public static class CommonModel
    {
        public static string SqlConnection = "Server=.;DataBase=OnCube;User ID=sa;Password=jvm5822511";
        public static string FileBackupRootDirectory = @"D:\Converter_Backup";
        public static NotifyIcon NotifyIcon { get; set; }
        public static ISql SqlHelper { get; set; }
        public static eDepartment CurrentDepartment { get; set; }
        public static eWindowType WindowType { get; set; } = eWindowType.MainWindow;
    }
}
