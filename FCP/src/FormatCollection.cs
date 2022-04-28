using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using FCP.src.Factory;
using FCP.Service;
using FCP.Models;
using FCP.src.Enum;
using FCP.ViewModels.Interface;
using FCP.src.Factory.Models;
using Helper;

namespace FCP.src
{
    abstract class FormatCollection
    {
        public string ID_S = "";  //身分證字號
        public string Gender_S = "";  //性別
        public string DoctorName_S = "";  //醫生姓名
        public string HospitalName_S = "";  //院所名稱
        public string Location_S = ""; //位置
        public string Age_S = ""; //年紀
        public string BirthDate_S = "";  //生日
        public string PatientName_S = "";  //病患姓名
        public string PatientNo_S = ""; //病歷號
        public string PrescriptionNo_S = "";  //處方籤號碼
        public string AdminCode_S = "";  //頻率
        public string FileContent { get; set; }  //檔案內容
        public string Class_S;
        public string FileNameOutput_S = ""; //輸出檔案名稱
        public bool IsStat_B = false;  //是否為即時
        public int ID1_I = 0;
        public int ID2_I = 0;
        public string SQLInfo_S = "Server=.;DataBase=OnCube;user id=sa;password=jvm5822511";
        public List<string> Unit_L = new List<string>();  //單位
        public List<string> MedicineSpecification_L = new List<string>();  //藥品規格
        public List<int> ID1_L = new List<int>();  //特控1
        public List<int> ID2_L = new List<int>();  //特控2
        public List<string> PatientName_L = new List<string>();  //病患姓名
        public List<string> PatientNo_L = new List<string>();  //病歷號
        public List<string> Way_L = new List<string>();  //途徑
        public List<string> PrescriptionNo_L = new List<string>();  //處方籤號碼
        public List<string> StartDay_L = new List<string>();  //開始日期
        public List<string> EndDay_L = new List<string>();  //結束日期
        public List<string> MedicalUnit_L = new List<string>();  //醫囑單位
        public List<string> MedicineName_L = new List<string>();  //藥品名稱
        public List<string> MedicineContent_L = new List<string>();  //藥品含量
        public List<string> MedicineCode_L = new List<string>();  //藥品代碼
        public List<string> AdminCode_L = new List<string>();  //頻率
        public List<string> PerQty_L = new List<string>();  //劑量
        public List<string> BirthDate_L = new List<string>();  //生日
        public List<string> BedNo_L = new List<string>();  //床號
        public List<string> RoomNo_L = new List<string>();  //病房號
        public List<string> Description_L = new List<string>();  //敘述
        public List<string> StayDay_L = new List<string>();  //住院日
        public List<string> Class_L = new List<string>();  //科別
        public List<string> TreatmentDate_L = new List<string>();  //診療日
        public List<string> NumberDetail_L = new List<string>();  //編號細項
        public List<string> SumQty_L = new List<string>();  //總量
        public List<string> Days_L = new List<string>();  //天數
        public List<string> Times_L = new List<string>();  //每日幾次
        public List<string> FileNameOutput_L = new List<string>(); //輸出檔案名稱
        public List<string> MedicineCodeGiven_L = new List<string>();
        List<string> FilterAdminCode = new List<string>();
        public Dictionary<string, List<string>> DataDic = new Dictionary<string, List<string>>();  //切點用
        public string[][] TimesOfAdminTime_L = new string[24][];  //頻率一天次數
        public Settings Settings { get; set; }
        public SettingsModel SettingsModel { get; set; }
        public string ErrorContent { get; set; }
        public ConvertFileInformtaionModel ConvertFileInformation { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public string FilePath { get; set; }
        public string CurrentSeconds { get; set; }
        public eConvertLocation Department { get; set; }
        private ReturnsResultFormat _ReturnsResultFormat { get; set; }
        public IRetunrsResult ReturnsResult { get; set; }

        public FormatCollection()
        {
            Settings = SettingsFactory.GenerateSettingsControl();
            SettingsModel = SettingsFactory.GenerateSettingsModel();
            ConvertFileInformation = ConvertInfoactory.GenerateConvertFileInformation();
        }

        public virtual void Init()
        {
            _ReturnsResultFormat = new ReturnsResultFormat();
            ReturnsResult = new ReturnsResult();
            ReturnsResult.SetReturnsResultFormat(_ReturnsResultFormat);
            InputPath = ConvertFileInformation.GetInputPath;
            OutputPath = ConvertFileInformation.GetOutputPath;
            FilePath = ConvertFileInformation.GetFilePath;
            CurrentSeconds = ConvertFileInformation.GetCurrentSeconds;
            Department = ConvertFileInformation.GetDepartment;
            SetAdminCode();
            if (SettingsModel.Mode != eFormat.光田醫院TJVS)  //光田磨粉、長庚磨粉
                GetMedicineCode();
            ClearList();
        }

        private void ClearList()
        {
            Unit_L.Clear();
            MedicineSpecification_L.Clear();
            ID1_L.Clear();
            ID2_L.Clear();
            PatientName_L.Clear();
            PatientNo_L.Clear();
            Way_L.Clear();
            PrescriptionNo_L.Clear();
            StartDay_L.Clear();
            EndDay_L.Clear();
            MedicalUnit_L.Clear();
            MedicineName_L.Clear();
            MedicineContent_L.Clear();
            MedicineCode_L.Clear();
            AdminCode_L.Clear();
            PerQty_L.Clear();
            BirthDate_L.Clear();
            BedNo_L.Clear();
            RoomNo_L.Clear();
            Description_L.Clear();
            Class_L.Clear();
            TreatmentDate_L.Clear();
            NumberDetail_L.Clear();
            SumQty_L.Clear();
            Days_L.Clear();
            Times_L.Clear();
            FileNameOutput_L.Clear();
            StayDay_L.Clear();
        }

        //方法分流
        public virtual ReturnsResultFormat MethodShunt()
        {
            Init();
            switch (Department)
            {
                case eConvertLocation.OPD:
                    if (ProcessOPD())
                    {
                        LogicOPD();
                    }
                    break;
                case eConvertLocation.POWDER:
                    if (ProcessPOWDER())
                    {
                        LogicPOWDER();
                    }
                    break;
                case eConvertLocation.UDStat:
                    if (ProcessUDStat())
                    {
                        LogicUDStat();
                    }
                    break;
                case eConvertLocation.UDBatch:
                    if (ProcessUDBatch())
                    {
                        LogicUDBatch();
                    }
                    break;
                case eConvertLocation.Other:
                    if (ProcessOther())
                    {
                        LogicOther();
                    }
                    break;
                default:
                    if (ProcessCare())
                    {
                        LogicCare();
                    }
                    break;
            }
            return _ReturnsResultFormat;
        }

        //特控1
        public int GetID1(string medicinecode)
        {
            ID1_I = 0;
            SqlConnection con = new SqlConnection(SQLInfo_S);
            con.Open();
            SqlCommand sqlcom = new SqlCommand($"SELECT * FROM Item WHERE Mnemonic='{medicinecode}'", con);
            SqlDataReader sqlr = sqlcom.ExecuteReader();
            while (sqlr.Read())
            {
                if (sqlr["ExternalID1"].ToString() != "")
                    ID1_I = Int32.Parse(sqlr["ExternalID1"].ToString());
                else
                    ID1_I = 0;
            }
            sqlr.Close();
            sqlcom.Dispose();
            con.Dispose();
            con.Close();
            return ID1_I;
        }

        //特控2
        public int GetID2(string medicinecode)
        {
            ID2_I = 0;
            SqlConnection con = new SqlConnection(SQLInfo_S);
            con.Open();
            SqlCommand sqlcom = new SqlCommand($"SELECT * FROM Item WHERE Mnemonic='{medicinecode}'", con);
            SqlDataReader sqlr = sqlcom.ExecuteReader();
            while (sqlr.Read())
            {
                if (sqlr["ExternalID2"].ToString() != "")
                    ID2_I = Int32.Parse(sqlr["ExternalID2"].ToString());
                else
                    ID2_I = 0;
            }
            sqlr.Close();
            sqlcom.Dispose();
            con.Dispose();
            con.Close();
            return ID2_I;
        }

        //計算共有幾筆藥 for JVServer
        public List<string> SeparateString(string Info, int Length)
        {
            List<string> List = new List<string>();
            Byte[] ATemp = Encoding.Default.GetBytes(Info);
            int x = 0;
            while (true)
            {
                if (x < Encoding.Default.GetString(ATemp, 0, ATemp.Length).Length & ATemp.Length - x >= Length)
                {
                    string a = Encoding.Default.GetString(ATemp, x, Length);
                    if (!string.IsNullOrWhiteSpace(a))
                    {
                        List.Add(a);
                        x += Length;
                    }
                }
                else
                    break;
            }
            return List;
        }

        //刪除空白
        public string DeleteSpace(string FileContent)
        {
            string[] FileContentSplit = FileContent.Split('\n');
            StringBuilder sb = new StringBuilder();
            foreach (string s in FileContentSplit)
            {
                if (!string.IsNullOrEmpty(s.Trim()))
                {
                    sb.AppendLine(s);
                }
            }
            return sb.ToString();
        }

        //取得檔案內容
        public string GetContent
        {
            get
            {
                using (StreamReader sr = new StreamReader(FilePath, Encoding.Default))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        //設定使用特定頻率、過濾特定頻率的頻率
        public void SetAdminCode()
        {
            FilterAdminCode.Clear();
            foreach (string s in SettingsModel.FilterAdminCode)
            {
                if (!string.IsNullOrEmpty(s))
                    FilterAdminCode.Add(s);
            }
        }

        //判斷使用特定頻率及過濾特定頻率
        public bool IsFilterAdminCode(string code)
        {
            if (SettingsModel.PackMode == ePackMode.正常)
                return false;
            if (SettingsModel.PackMode == ePackMode.過濾特殊)
            {
                return FilterAdminCode.Contains(code);
            }
            else
            {
                return !FilterAdminCode.Contains(code);
            }
        }

        /// <summary>
        /// 判斷指定頻率頻率是否被設為需過濾
        /// </summary>
        /// <param name="Code">頻率</param>
        /// <returns>如果為 <see langword="true"/> 則為需過濾，為 <see langword="false"/> 則為不需過濾</returns>
        public bool NeedFilterMedicineCode(string Code)
        {
            return SettingsModel.FilterMedicineCode.Contains(Code);
        }

        //計算總量除以ID1的餘數
        public float CalculationID1Remainder()
        {
            float a = 0;
            List<float> re = new List<float>();
            for (int r = 0; r <= AdminCode_L.Count - 1; r++)
            {
                float id1;
                id1 = Convert.ToSingle(ID1_L[r].ToString());
                float correcttotalquanity;
                float remainder;
                if (id1 > 0)
                {
                    correcttotalquanity = Convert.ToSingle(SumQty_L[r].ToString());
                    remainder = Convert.ToSingle(Math.Floor(correcttotalquanity % id1));  //總量除以ID1的餘數
                }
                else
                    remainder = Convert.ToSingle(SumQty_L[r].ToString());
                re.Add(remainder);
            }
            foreach (float f in re)
                a += f;
            return a;
        }

        /// <summary>
        /// 取得指定頻率在餐包裡的時間點
        /// </summary>
        /// <param name="code">頻率</param>
        /// <returns>時間點的陣列 (ex:08,13,18)</returns>
        public List<string> GetMultiAdminCodeTimes(string code)
        {
            List<string> list = MSSql.RunSQL_List($@"SELECT
	                                                     LEFT(CONVERT(VARCHAR,A.StandardTime,108), 5) AS Time
                                                     ,	A.RawID
                                                     FROM AdminTime A,
                                                     (
                                                         SELECT
	                                                         RawID
                                                         FROM AdminTime A
                                                         WHERE A.AdminCode=N'{code}' AND DeletedYN=0 AND A.UpperAdminTimeID is null
                                                     ) #UpperAdminTime
                                                     WHERE A.UpperAdminTimeID=#UpperAdminTime.RawID AND A.DeletedYN=0", "Time");
            var newList = list.Where(x => x.Length != 0).Select(x => x).ToList();
            return newList;
        }

        /// <summary>
        /// 確認指定頻率是否存在餐包的設定裡
        /// </summary>
        /// <param name="code">頻率</param>
        /// <returns>存在為 <see langword="true"/> ，不存在為 <see langword="false"/></returns>
        public bool IsExistsMultiAdminCode(string code)
        {
            int result = MSSql.RunSQL_FirstInt($@"SELECT
	                                                   TOP 1 COUNT(RawID)
                                                   FROM AdminTime A
                                                   WHERE A.AdminCode=N'{code}' AND DeletedYN=0 AND A.UpperAdminTimeID is null");

            return result > 0;
        }

        /// <summary>
        /// 確認指定頻率是否存在種包的設定裡
        /// </summary>
        /// <param name="code">頻率</param>
        /// <returns>存在為 <see langword="true"/> ，不存在為 <see langword="false"/></returns>
        public bool IsExistsCombiAdminCode(string code)
        {
            int result = MSSql.RunSQL_FirstInt($@"SELECT
	                                                  TOP 1 COUNT(RawID)
                                                  FROM AdminTime A
                                                  WHERE A.AdminCode=N'S{code}' AND DeletedYN=0");
            return result > 0;
        }

        /// <summary>
        /// 判斷頻率在餐包及種包的集合
        /// </summary>
        /// <param name="code">頻率</param>
        /// <returns>都存在為 <see langword="true"/> ，有任一個不存在為 <see langword="false"/></returns>
        public bool IsExistsMultiAndCombiAdminCode(string code)
        {
            return IsExistsMultiAdminCode(code) && IsExistsCombiAdminCode(code);
        }

        public bool IsFilterMedicineCode(string code)
        {
            if (!SettingsModel.EN_FilterMedicineCode)
                return false;
            return !GetMedicineCode().Contains(code);
        }

        private List<string> GetMedicineCode()
        {
            List<string> list = SettingsModel.EN_OnlyCanisterIn ? GetMedicineCodeHasCanister() : GetAllMedicineCode();
            return list;
        }

        private List<string> GetMedicineCodeHasCanister()
        {
            List<string> list = MSSql.RunSQL_List(@"SELECT
                                                        A.Mnemonic + ',' + ISNULL(C.MultiMnemonic, '') AS Mnemonic
                                                    FROM Item A
                                                    INNER JOIN InventoryContainer B ON A.DeletedYN = 0 AND A.UseYN = 1 AND B.ContainerID IS NOT NULL AND A.RawID = B.ItemID
                                                    LEFT OUTER JOIN ItemMultiCode C ON A.RawID = C.ItemID AND C.DeletedYN = 0", "Mnemonic");
            List<string> newList = new List<string>();
            foreach (string s in list)
            {
                string[] temp = s.Split(',');
                if (temp[1].Length > 0)
                    newList.Add(temp[1]);
                newList.Add(temp[0]);
            }
            return newList;
        }

        private List<string> GetAllMedicineCode()
        {
            List<string> list = MSSql.RunSQL_List(@"SELECT
                                                        A.Mnemonic + ',' + ISNULL(B.MultiMnemonic, '') AS Mnemonic
                                                    FROM Item A
                                                    LEFT OUTER JOIN ItemMultiCode B ON A.RawID = B.ItemID AND B.DeletedYN = 0
                                                    WHERE A.DeletedYN = 0 AND A.UseYN = 1", "Mnemonic");
            List<string> newList = new List<string>();
            foreach (string s in list)
            {
                string[] temp = s.Split(',');
                if (temp[1].Length > 0)
                    newList.Add(temp[1]);
                newList.Add(temp[0]);
            }
            return newList;
        }

        public List<string> GetMedicineCodeWhenWeightIs10g()
        {
            List<string> list = MSSql.RunSQL_List(@"SELECT
	                                                    B.Mnemonic + ',' + ISNULL(D.MultiMnemonic, '') AS Mnemonic
                                                    FROM Medicine A
                                                    INNER JOIN Item B ON A.RawID=B.RawID AND A.DeletedYN=0 AND A.WeightMedicine=10
                                                    LEFT OUTER JOIN ItemMultiCode D ON B.RawID=D.ItemID AND D.DeletedYN=0", "Mnemonic");
            List<string> newList = new List<string>();
            foreach (string s in list)
            {
                string[] temp = s.Split(',');
                if (temp[1].Length > 0)
                    newList.Add(temp[1]);
                newList.Add(temp[0]);
            }
            return newList;
        }

        public abstract bool ProcessOPD();
        public abstract bool LogicOPD();
        public abstract bool ProcessUDBatch();
        public abstract bool LogicUDBatch();
        public abstract bool ProcessUDStat();
        public abstract bool LogicUDStat();
        public abstract bool ProcessPOWDER();
        public abstract bool LogicPOWDER();
        public abstract bool ProcessOther();
        public abstract bool LogicOther();
        public abstract bool ProcessCare();
        public abstract bool LogicCare();
    }
}