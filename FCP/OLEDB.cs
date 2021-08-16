using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FCP
{
    static class OLEDB
    {
        public static List<MinShengUDBatch> GetMingSheng_UD(string filePath, string fileName, int index)
        {
            List<MinShengUDBatch> _UDBatch = new List<MinShengUDBatch>();
            OleDbConnection conn = new OleDbConnection($@"Provider=vfpoledb;Data Source={filePath};Collating Sequence=machine;");
            conn.Open();
            OleDbCommand com = new OleDbCommand($"SELECT *, RecNo() AS RowNum FROM {fileName} WHERE RecNo() > {index}", conn);
            OleDbDataReader dr = com.ExecuteReader();
            while (dr.Read())
            {
                DateTime.TryParseExact((Convert.ToInt32($"{dr["BEDATE"]}".Trim()) + 19110000).ToString(), "yyyyMMdd", null, DateTimeStyles.None, out DateTime Start);
                string StartDate = Start.ToString("yyMMdd");
                _UDBatch.Add(new MinShengUDBatch
                {
                    RecNo = int.Parse($"{dr["RowNum"]}".Trim()),
                    PrescriptionNo = $"{dr["MEDNO"]}".Trim(),
                    BedNo = $"{dr["BEDNO"]}".Trim(),
                    PatientName = $"{dr["MHNAME"]}".Trim().Replace('?', ' '),
                    MedicineCode = $"{dr["DIACODE"]}".Trim(),
                    MedicineName = $"{dr["DIANAME"]}".Trim(),
                    PerQty = $"{dr["PER_QTY"]}".Trim(),
                    AdminCode = $"{dr["USENO"]}".Trim(),
                    Description = $"{dr["USENONAME"]}".Trim(),
                    SumQty = $"{dr["SUMQTY"]}".Trim(),
                    StartDay = StartDate,
                    BeginTime = $"{dr["BETIME"]}".Trim(),
                    Unit = $"{dr["UNITS"]}".Trim()
                });
            }
            dr.Close();
            com.Dispose();
            conn.Close();
            return _UDBatch;
        }

        public static List<MinShengOPD> GetMingSheng_OPD(string filePath, string fileName, int index)
        {
            try
            {
                List<MinShengOPD> _OPD = new List<MinShengOPD>();
                OleDbConnection conn = new OleDbConnection($@"Provider=vfpoledb;Data Source={filePath};Collating Sequence=machine;");
                conn.Open();
                OleDbCommand com = new OleDbCommand($@"SELECT
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
                                                       FROM {fileName}
                                                       WHERE RecNo() > {index}", conn);
                OleDbDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["PROCDATE"].ToString().Trim() == "XXX")
                        continue;
                    DateTime.TryParseExact((Convert.ToInt32($"{dr["PROCDATE"]}".Trim()) + 19110000).ToString(), "yyyyMMdd", null, DateTimeStyles.None, out DateTime Start);
                    string StartDate = Start.ToString("yyMMdd");
                    _OPD.Add(new MinShengOPD
                    {
                        RecNo = int.Parse($"{dr["RowNum"]}".Trim()),
                        PrescriptionNo = $"{dr["MEDNO"]}".Trim(),
                        PatientName = $"{dr["MHNAME"]}".Trim().Replace('?', ' '),
                        Age = $"{dr["AGE"]}".Trim(),
                        DrugNo = $"{dr["DRUGNO"]}".Trim(),
                        MedicineCode = $"{dr["DIACODE"]}".Trim(),
                        MedicineName = $"{dr["DIANAME"]}".Trim(),
                        PerQty = $"{dr["PER_QTY"]}".Trim(),
                        Unit = $"{dr["Unit"]}".Trim(),
                        AdminCode = $"{dr["USENO"]}".Trim(),
                        Days = $"{dr["DAYS"]}".Trim(),
                        SumQty = $"{dr["SUMQTY"]}".Trim(),
                        StartDay = StartDate,
                        BeginTime = $"{dr["PROCTIME"]}".Trim(),
                        EndDay = Start.AddDays(int.Parse($"{dr["DAYS"]}".Trim()) - 1).ToString("yyMMdd")
                    });
                }
                dr.Close();
                //olecom = new OleDbCommand($"INSERT INTO {FileName} VALUES('00000','1100706','侯名哲','1','024','0000','34','750','OCELE2','(200 mg) Celebrex 200 mg',1,'CAP','PO','QD','28',28,'CAP','N','2','N','1100706','1400')", olecon);
                //olecom.ExecuteNonQuery();
                com.Dispose();
                conn.Close();
                return _OPD;
            }
            catch (Exception)
            {
                return new List<MinShengOPD>();
            }
        }
    }
}
