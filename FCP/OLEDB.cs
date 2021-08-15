using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.OleDb;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FCP
{
    class OLEDB
    {
        ObservableCollection<MinSheng_UD> MS_UD = new ObservableCollection<MinSheng_UD>();
        ObservableCollection<MinSheng_OPD> MS_OPD = new ObservableCollection<MinSheng_OPD>();

        public ObservableCollection<MinSheng_UD> GetMingSheng_UD(string FilePath, string FileName, int Index)
        {
            MS_UD.Clear();
            OleDbConnection olecon = new OleDbConnection($@"Provider=vfpoledb;Data Source={FilePath};Collating Sequence=machine;");
            olecon.Open();
            OleDbCommand olecom = new OleDbCommand($"SELECT *, RecNo() AS RowNum FROM {FileName} WHERE RecNo() > {Index}", olecon);
            OleDbDataReader sr = olecom.ExecuteReader();
            while (sr.Read())
            {
                DateTime.TryParseExact((Convert.ToInt32($"{sr["BEDATE"]}".Trim()) + 19110000).ToString(), "yyyyMMdd", null, DateTimeStyles.None, out DateTime Start);
                string StartDate = Start.ToString("yyMMdd");
                MS_UD.Add(new MinSheng_UD
                {
                    RecNo = int.Parse($"{sr["RowNum"]}".Trim()),
                    PrescriptionNo = $"{sr["MEDNO"]}".Trim(),
                    BedNo = $"{sr["BEDNO"]}".Trim(),
                    PatientName = $"{sr["MHNAME"]}".Trim().Replace('?', ' '),
                    MedicineCode = $"{sr["DIACODE"]}".Trim(),
                    MedicineName = $"{sr["DIANAME"]}".Trim(),
                    PerQty = $"{sr["PER_QTY"]}".Trim(),
                    AdminCode = $"{sr["USENO"]}".Trim(),
                    Description = $"{sr["USENONAME"]}".Trim(),
                    SumQty = $"{sr["SUMQTY"]}".Trim(),
                    StartDay = StartDate,
                    BeginTime = $"{sr["BETIME"]}".Trim(),
                    Unit = $"{sr["UNITS"]}".Trim()
                });
            }
            sr.Close();
            olecom.Dispose();
            olecon.Dispose();
            return MS_UD;
        }

        public ObservableCollection<MinSheng_OPD> GetMingSheng_OPD(string FilePath, string FileName, int Index)
        {
            try
            {
                //Debug.WriteLine($"The OLEDB currently index is {Index}");
                MS_OPD.Clear();
                OleDbConnection olecon = new OleDbConnection($@"Provider=vfpoledb;Data Source={FilePath};Collating Sequence=machine;");
                olecon.Open();
                OleDbCommand olecom = new OleDbCommand($@"SELECT
                                                          RecNo() AS RowNum
                                                        , MEDNO
                                                        , MHNAME
                                                        , AGE
                                                        , DRUGNO
                                                        , DIACODE
                                                        , DIANAME
                                                        , FLOAT(PER_QTY) AS PER_QTY
                                                        , Unit
                                                        , USENO
                                                        , DAYS
                                                        , FLOAT(SUMQTY) AS SUMQTY
                                                        , PROCDATE
                                                        , PROCTIME
                                                       FROM {FileName}
                                                       WHERE RecNo() > {Index}", olecon);
                OleDbDataReader sr = olecom.ExecuteReader();
                while (sr.Read())
                {
                    if (sr["PROCDATE"].ToString().Trim() == "XXX")
                        continue;
                    DateTime.TryParseExact((Convert.ToInt32($"{sr["PROCDATE"]}".Trim()) + 19110000).ToString(), "yyyyMMdd", null, DateTimeStyles.None, out DateTime Start);
                    string StartDate = Start.ToString("yyMMdd");
                    MS_OPD.Add(new MinSheng_OPD
                    {
                        RecNo = int.Parse($"{sr["RowNum"]}".Trim()),
                        PrescriptionNo = $"{sr["MEDNO"]}".Trim(),
                        PatientName = $"{sr["MHNAME"]}".Trim().Replace('?', ' '),
                        Age = $"{sr["AGE"]}".Trim(),
                        DrugNo = $"{sr["DRUGNO"]}".Trim(),
                        MedicineCode = $"{sr["DIACODE"]}".Trim(),
                        MedicineName = $"{sr["DIANAME"]}".Trim(),
                        PerQty = $"{sr["PER_QTY"]}".Trim(),
                        Unit = $"{sr["Unit"]}".Trim(),
                        AdminCode = $"{sr["USENO"]}".Trim(),
                        Days = $"{sr["DAYS"]}".Trim(),
                        SumQty = $"{sr["SUMQTY"]}".Trim(),
                        StartDay = StartDate,
                        BeginTime = $"{sr["PROCTIME"]}".Trim(),
                        EndDay = Start.AddDays(int.Parse($"{sr["DAYS"]}".Trim()) - 1).ToString("yyMMdd")
                    });
                }
                sr.Close();
                //olecom = new OleDbCommand($"INSERT INTO {FileName} VALUES('00000','1100706','侯名哲','1','024','0000','34','750','OCELE2','(200 mg) Celebrex 200 mg',1,'CAP','PO','QD','28',28,'CAP','N','2','N','1100706','1400')", olecon);
                //olecom.ExecuteNonQuery();
                olecom.Dispose();
                olecon.Dispose();
                return MS_OPD;
            }
            catch (Exception)
            {
                //Debug.WriteLine(ex.ToString());
                return new ObservableCollection<MinSheng_OPD>();
            }
        }

        public class MinSheng_UD
        {
            public int RecNo { get; set; }
            public string PrescriptionNo { get; set; }
            public string BedNo { get; set; }
            public string PatientName { get; set; }
            public string MedicineCode { get; set; }
            public string MedicineName { get; set; }
            public string PerQty { get; set; }
            public string AdminCode { get; set; }
            public string Description { get; set; }
            public string SumQty { get; set; }
            public string StartDay { get; set; }
            public string BeginTime { get; set; }
            public string Unit { get; set; }
        }

        public class MinSheng_OPD
        {
            public int RecNo { get; set; }
            public string PrescriptionNo { get; set; }
            public string PatientName { get; set; }
            public string Age { get; set; }
            public string DrugNo { get; set; }
            public string MedicineCode { get; set; }
            public string MedicineName { get; set; }
            public string PerQty { get; set; }
            public string Unit { get; set; }
            public string AdminCode { get; set; }
            public string Days { get; set; }
            public string SumQty { get; set; }
            public string StartDay { get; set; }
            public string BeginTime { get; set; }
            public string EndDay { get; set; }
        }
    }
}
