using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FCP
{
    class WorkSchedule
    {
        List<string> HospitalWorkSchedule = new List<string>() { "門診", "住院" };
        List<string> HospitalUDWorkSchedule = new List<string>() { "門診", "住院即時", "住院長期" };
        List<string> NormalWorkSchedule = new List<string>() { "開始轉檔" };
        List<string> PowderWorkSchedule = new List<string>() { "磨粉" };
        Dictionary<string, string> DayOfWeekDic = new Dictionary<string, string>();
        ObservableCollection<WorkScheduleclass> WSclass = new ObservableCollection<WorkScheduleclass>();

        public List<string> JudgeWorkScheduleSources(string ConvertFormatType, string OnTimeAndBatch)
        {
            List<string> ResultList = new List<string>();
            if (ConvertFormatType == "小港ToOnCube" | ConvertFormatType == "光田ToOnCube" | ConvertFormatType  == "凱旋ToOnCube")
            {
                if (OnTimeAndBatch == "True")
                    ResultList = HospitalUDWorkSchedule;
                else
                    ResultList = HospitalWorkSchedule;
            }
            else if (ConvertFormatType == "光田ToJVServer")
                ResultList = PowderWorkSchedule;
            else
                ResultList = NormalWorkSchedule;
            return ResultList;
        }

        public int ComapreTime()
        {
            DateTime Time = DateTime.Now;
            foreach(var v in WSclass)
            {
                DateTime Start = v.StartTime;
                DateTime End = v.EndTime;
                DateTime.TryParseExact(Time.ToString("HH:mm"), "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime now);
                if (DateTime.Compare(Start, Time) == -1 & DateTime.Compare(End, Time) == 1)
                {
                    if (DayOfWeekDic.Count == 0)
                        PutWeek();
                    string[] Day = v.Cycle.Split(',');
                    if (!DayOfWeekDic.TryGetValue(Time.DayOfWeek.ToString(), out string Today))
                        break;
                    if (Day.ToList().Contains(Today))
                    {
                        return v.Action;
                    }
                }
            }
            return -1;
        }

        public void GetData(string WorkScheduleContent, string ConvertFormatType, string OnTimeAndBatch)
        {
            WSclass.Clear();
            string[] WorkScheduleList = WorkScheduleContent.Split(';');
            List<string> ResultList = JudgeWorkScheduleSources(ConvertFormatType, OnTimeAndBatch);
            for (int x = 0; x <= WorkScheduleList.ToList().Count - 1; x++)
            {
                string Content = WorkScheduleList.ToList()[x];
                WSclass.Add(new WorkScheduleclass
                {
                    StartTime = Convert.ToDateTime(Content.Substring(2, 5)),
                    EndTime = Convert.ToDateTime(Content.Substring(10, 5)),
                    ActionSources = ResultList,
                    Action = Convert.ToInt32(Content.Substring(18, 1)),
                    Cycle = Content.Substring(22, Content.Length - 22)
                });
            }
        }

        public void PutWeek()
        {
            DayOfWeekDic.Add("Monday", "1");
            DayOfWeekDic.Add("Tuesday", "2");
            DayOfWeekDic.Add("Wednesday", "3");
            DayOfWeekDic.Add("Thirsday", "4");
            DayOfWeekDic.Add("Friday", "5");
            DayOfWeekDic.Add("Saturday", "6");
            DayOfWeekDic.Add("Sunday", "7");
        }

        public class WorkScheduleclass
        {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public List<string> ActionSources { get; set; }
            public int Action { get; set; }
            public string Cycle { get; set; }
        }
    }
}
