using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Collections.ObjectModel;
using System.Globalization;
using FCP.src.FormatControl;

namespace FCP.src.SQL
{
    static class OLEDB
    {
        public static List<MinShengUDBatch> GetMingSheng_UD(string oledbFilePath, string fileName, int index)
        {
            try
            {
                List<MinShengUDBatch> batch = new List<MinShengUDBatch>();
                OleDbConnection cnn = new OleDbConnection($@"Provider=vfpoledb;Data Source={oledbFilePath};Collating Sequence=machine;");
                cnn.Open();
                OleDbCommand com = new OleDbCommand($"SELECT *, RecNo() AS RowNum FROM {fileName} WHERE RecNo() > {index}", cnn);
                OleDbDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    DateTime.TryParseExact((Convert.ToInt32($"{dr["BEDATE"]}".Trim()) + 19110000).ToString(), "yyyyMMdd", null, DateTimeStyles.None, out DateTime startDate);
                    batch.Add(new MinShengUDBatch
                    {
                        RecNo = int.Parse($"{dr["RowNum"]}".Trim()),
                        PrescriptionNo = $"{dr["MEDNO"]}".Trim(),
                        BedNo = $"{dr["BEDNO"]}".Trim(),
                        PatientName = $"{dr["MHNAME"]}".Trim().Replace('?', ' '),
                        MedicineCode = $"{dr["DIACODE"]}".Trim(),
                        MedicineName = $"{dr["DIANAME"]}".Trim(),
                        PerQty = Convert.ToSingle($"{dr["PER_QTY"]}".Trim()),
                        AdminCode = $"{dr["USENO"]}".Trim(),
                        Description = $"{dr["USENONAME"]}".Trim(),
                        SumQty = Convert.ToSingle($"{dr["SUMQTY"]}".Trim()),
                        StartDate = startDate,
                        BeginTime = $"{dr["BETIME"]}".Trim(),
                        Unit = $"{dr["UNITS"]}".Trim()
                    });
                }
                dr.Close();
                com.Dispose();
                cnn.Close();
                return batch;
            }
            catch
            {
                throw;
            }
        }

        public static List<MinShengOPD> GetMingSheng_OPD(string oledbFilePath, string fileName, int index)
        {
            try
            {
                List<MinShengOPD> _opd = new List<MinShengOPD>();
                OleDbConnection cnn = new OleDbConnection($@"Provider=vfpoledb;Data Source={oledbFilePath};Collating Sequence=machine;");
                cnn.Open();
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
                                                       WHERE RecNo() > {index}", cnn);
                OleDbDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["PROCDATE"].ToString().Trim() == "XXX")
                        continue;
                    DateTime.TryParseExact((Convert.ToInt32($"{dr["PROCDATE"]}".Trim()) + 19110000).ToString(), "yyyyMMdd", null, DateTimeStyles.None, out DateTime startDate);
                    _opd.Add(new MinShengOPD
                    {
                        RecNo = int.Parse($"{dr["RowNum"]}".Trim()),
                        PrescriptionNo = $"{dr["MEDNO"]}".Trim(),
                        PatientName = $"{dr["MHNAME"]}".Trim().Replace('?', ' '),
                        Age = $"{dr["AGE"]}".Trim(),
                        DrugNo = $"{dr["DRUGNO"]}".Trim(),
                        MedicineCode = $"{dr["DIACODE"]}".Trim(),
                        MedicineName = $"{dr["DIANAME"]}".Trim(),
                        PerQty = Convert.ToSingle($"{dr["PER_QTY"]}".Trim()),
                        Unit = $"{dr["Unit"]}".Trim(),
                        AdminCode = $"{dr["USENO"]}".Trim(),
                        Days = Convert.ToInt32($"{dr["DAYS"]}".Trim()),
                        SumQty = Convert.ToSingle($"{dr["SUMQTY"]}".Trim()),
                        StartDate = startDate,
                        BeginTime = $"{dr["PROCTIME"]}".Trim(),
                        EndDate = startDate.AddDays(int.Parse($"{dr["DAYS"]}".Trim()) - 1)
                    });
                }
                dr.Close();
                //olecom = new OleDbCommand($"INSERT INTO {FileName} VALUES('00000','1100706','侯名哲','1','024','0000','34','750','OCELE2','(200 mg) Celebrex 200 mg',1,'CAP','PO','QD','28',28,'CAP','N','2','N','1100706','1400')", olecon);
                //olecom.ExecuteNonQuery();
                com.Dispose();
                cnn.Close();
                return _opd;
            }
            catch (Exception)
            {
                return new List<MinShengOPD>();
            }
        }
    }
}
