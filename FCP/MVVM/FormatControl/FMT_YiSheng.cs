﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.Models;
using FCP.MVVM.Helper;

namespace FCP.MVVM.FormatControl
{
    class FMT_YiSheng : FormatCollection
    {
        private List<YiShengOPD> _OPD = new List<YiShengOPD>();

        public override bool ProcessOPD()
        {
            try
            {
                List<string> list = GetContent.Split('\n').ToList();
                foreach (string s in list)  //將藥品資料放入List<string>
                {
                    EncodingHelper.SetEncodingBytes(s);
                    string adminCode = EncodingHelper.GetEncodingString(137, 10).Replace("/", "");
                    string medicineCode = EncodingHelper.GetEncodingString(63, 10);
                    if (IsExistsMedicineCode(medicineCode) || IsFilterAdminCode(adminCode))
                        continue;
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置此餐包頻率 {adminCode}");
                        ReturnsResult.Shunt(ConvertResult.沒有餐包頻率, adminCode);
                        return false;
                    }
                    string dateTemp = (Convert.ToInt32(EncodingHelper.GetEncodingString(56, 7)) + 19110000).ToString();
                    DateTime.TryParseExact(dateTemp, "yyyyMMdd", null, DateTimeStyles.None, out DateTime startDate);
                    int days = Convert.ToInt32(EncodingHelper.GetEncodingString(147, 3));
                    _OPD.Add(new YiShengOPD()
                    {
                        PatientName = EncodingHelper.GetEncodingString(6, 20),
                        PatientNo = EncodingHelper.GetEncodingString(26, 10),
                        ID = EncodingHelper.GetEncodingString(36, 10),
                        Age = EncodingHelper.GetEncodingString(46, 10).Substring(0, EncodingHelper.GetEncodingString(46, 10).IndexOf(".")),
                        StartDate = startDate.ToString("yyMMdd"),
                        EndDate = startDate.AddDays(days - 1).ToString("yyMMdd"),
                        MedicineCode = medicineCode,
                        MedicineName = EncodingHelper.GetEncodingString(73, 54),
                        PerQty = EncodingHelper.GetEncodingString(127, 10),
                        AdminCode = adminCode,
                        Days = days.ToString(),
                        SumQty = EncodingHelper.GetEncodingString(150, 10),
                    });
                }
                if (_OPD.Count == 0)
                {
                    ReturnsResult.Shunt(ConvertResult.全數過濾, null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath}  {ex}");
                ReturnsResult.Shunt(ConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicOPD()
        {
            string filePathOutput = $@"{OutputPath}\{_OPD[0].PatientName}-{Path.GetFileNameWithoutExtension(FilePath)}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.YiSheng(_OPD, filePathOutput);
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath}  {ex}");
                ReturnsResult.Shunt(ConvertResult.產生OCS失敗, ex.ToString());
                return false;
            }
        }

        public override bool ProcessUDBatch()
        {
            throw new NotImplementedException();
        }

        public override bool LogicUDBatch()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessUDStat()
        {
            throw new NotImplementedException();
        }

        public override bool LogicUDStat()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessPOWDER()
        {
            throw new NotImplementedException();
        }

        public override bool LogicPOWDER()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessOther()
        {
            throw new NotImplementedException();
        }

        public override bool LogicOther()
        {
            throw new NotImplementedException();
        }

        public override bool ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override bool LogicCare()
        {
            throw new NotImplementedException();
        }

        public override ReturnsResultFormat MethodShunt()
        {
            _OPD.Clear();
            return base.MethodShunt();
        }
    }
    internal class YiShengOPD
    {
        public string PatientNo { get; set; }
        public string PatientName { get; set; }
        public string Age { get; set; }
        public string ID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string Days { get; set; }
        public string SumQty { get; set; }
    }
}
