using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace FCP
{
    public class Log
    {
        string Path = @"D:\FCP\LOG";
        string Date;
        public void Check()
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd");
            if (!Directory.Exists($@"{Path}\{Date}"))
                Directory.CreateDirectory($@"{Path}\{Date}");
        }
        public void Write(string wrong)
        {
            string WrongDate;
            WrongDate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss.ffff");
            Check();
            File.AppendAllText($@"{Path}\{Date}\Error_Log.txt", string.Format("[{0}] >> {1}\n\n", WrongDate, wrong));
        }
        public void Prescription(string filename, string name, string prescriptionnumber, List<string> medicinecode, List<string> medicinename, List<string> admintime, List<string> dosage, List<string> startendday)
        {
            try
            {
                File.AppendAllText($@"{Path}\{Date}\Error_Log.txt", "\n檔案名稱：" + filename + "\n");
                string aa;
                string bb;
                if (admintime.Count >= 1)
                    aa = "\n";
                else
                    aa = "\n\n";
                File.AppendAllText($@"{Path}\{Date}\Error_Log.txt", string.Format("姓名：{0}  病歷號：{1}{2}", name, prescriptionnumber, aa));
                for (int x = 0; x <= admintime.Count - 1; x++)
                {
                    if (x <= admintime.Count - 2)
                        bb = "\n";
                    else
                        bb = "\n\n";
                    File.AppendAllText($@"{Path}\{Date}\Error_Log.txt", string.Format("健保碼：{0}  藥品名：{1}  頻率：{2}  劑量：{3}  起訖日：{4}{5}", medicinecode[x], medicinename[x], string.Format("{0,-14}", admintime[x]), dosage[x], startendday[x], bb));
                }
            }
            catch(Exception a)
            {
                MessageBox.Show(a.ToString());
            }
        }

        public void Prescription(string filename, List<string> name, List<string> prescriptionno, List<string> medicinecode, List<string> medicinename, List<string> admintime, List<string> dosage, List<string> totaldosage, List<string> startendday)
        {
            try
            {
                File.AppendAllText($@"{Path}\{Date}\Error_Log.txt", $"產生處方籤時發生錯誤\n檔案名稱：{filename}\n");
                string aa;
                for (int x = 0; x <= admintime.Count - 1; x++)
                {
                    aa = x <= admintime.Count - 2 ? "\n" : "\n\n";
                    File.AppendAllText($@"{Path}\{Date}\Error_Log.txt", $"姓名：{string.Format("{0, -10}", name[x])} 病歷號：{prescriptionno[x]}  藥品代碼：{string.Format("{0, -10}", medicinecode[x])} 藥品名稱：{string.Format("{0, -50}", medicinename[x])} 頻率：{string.Format("{0, -10}", admintime[x])} 劑量：{dosage[x]} 總量：{totaldosage[x]} 開始日期：{startendday[x]}{aa}");
                }
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString());
            }
        }
    }
}
