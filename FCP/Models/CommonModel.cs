using FCP.src.Enum;
using SqlHelper.Interface;
using System.Windows.Forms;

namespace FCP.Models
{
    public static class CommonModel
    {
        public static string OnCubeSqlConnection = "Server=.;DataBase=OnCube;User ID=sa;Password=jvm5822511";
        public static string JVServerSqlConnection = @"Server=127.0.0.1\SQLExpress;DataBase=AdminCode;User ID=sa;Password=JVM";
        public static string FileBackupRootDirectory = @"D:\Converter_Backup";
        public static NotifyIcon NotifyIcon { get; set; }
        public static ISql SqlHelper { get; set; }
        public static eDepartment CurrentDepartment { get; set; }
        public static eWindowType WindowType { get; set; } = eWindowType.MainWindow;
    }
}
