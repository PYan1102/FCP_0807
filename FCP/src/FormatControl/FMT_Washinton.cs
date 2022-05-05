using FCP.Models;
using FCP.src.Enum;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace FCP.src.FormatControl
{
    class FMT_Washinton : FormatCollection
    {
        //有些客戶會包大量拿去外面賣，因此不希望藥包上有名子
        private List<string> _filterPatientNo = new List<string>() { "0058145","0051572", "0094562", "0069489", "0080950", "0051640", "0085193",
        "0057404", "0096220"};
        private JVServerXMLOPDBasic _basic = new JVServerXMLOPDBasic();
        private List<JVServerXMLOPD> _opd = new List<JVServerXMLOPD>();
        private List<string> _jvsRandom = new List<string>();
        private List<string> _oncubeRandom = new List<string>();

        public override void ProcessOPD()
        {
            try
            {
                XMLHelper.Load(SourceFilePath);
                DateTime startDate = DateTimeHelper.Convert(XMLHelper.GetParameterUseString("case", 0, "date"), "yyyyMMdd");

                //檔名開頭為XX時病患名稱設為空白
                _basic.PatientName = XMLHelper.GetParameterUseString("case/profile/person", 0, "name");
                _basic.Type = XMLHelper.GetParameterUseString("case/insurance", 0, "insurance_type_text") == "自費診" ? "自費-" : "健保-";
                _basic.Type += XMLHelper.GetParameterUseString("case/insurance", 0, "labeno_type");
                _basic.Type = _basic.Type.Replace("*", "+")
                    .Replace("\\", "＼");
                string pattern = "[\\[\\]\\^\\**×―(^)$%!@#$…&%￥—=<>《》/|!！??？:：•'`·、。，；,;\"‘’“”-]";
                _basic.Type = Regex.Replace(_basic.Type, pattern, "");
                _basic.LocationName = XMLHelper.GetParameterUseString("case/study", 0, "doctor_id") == "3539031772" ? "蔡鼎族診所" : "鄭小兒診所";
                _basic.HospitalName = "華盛頓虎尾藥局";
                DateTime maxEndDate = startDate;
                foreach (XmlNode node in XMLHelper.GetNodeList("case/orders/item"))
                {
                    string days = XMLHelper.GetParameterUseString("case/orders/item", 0, "days");
                    DateTime endDate = startDate.AddDays(Convert.ToInt32(days) - 1);
                    string medicineCode = node.Attributes["local_code"].Value;
                    string adminCode = node.Attributes["freq"].Value;
                    adminCode = adminCode == "2" ? "BID" : adminCode == "3" ? "TID" : adminCode == "4" ? "QID" : adminCode;
                    string medicineName = node.Attributes["desc"].Value.Replace("&quot;", "''");
                    bool needToPack = Convert.ToSingle(node.Attributes["divided_dose"].Value) % 0.5 == 0 || medicineCode == "BRO";  //單次劑量被0.5整除則包出，否則不包
                    if (!needToPack)
                    {
                        Pass();
                        return;
                    }
                    bool powder = node.Attributes["memo"].Value == "P";  //P為磨粉
                    if (IsFilterMedicineCode(medicineCode) || IsFilterAdminCode(adminCode) || powder || NeedFilterMedicineCode(medicineCode))
                        continue;
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        LostMultiAdminCode(adminCode);
                        return;
                    }
                    float perQty = Convert.ToSingle(node.Attributes["divided_dose"].Value);
                    int memoQty = node.Attributes["memo"].Value.ToString().Length == 0 ? 0 : Int32.TryParse(node.Attributes["memo"].Value, out int qty) ? qty : 0;
                    if (_basic.LocationName == "鄭小兒診所" && memoQty > 0)
                    {
                        List<string> adminCodeList = GetMultiAdminCodeTimes(adminCode);
                        days = (memoQty / adminCodeList.Count / perQty).ToString();
                        endDate = startDate.AddDays(Convert.ToInt32(days) - 1);
                    }

                    #region 符合輸出空白病患名稱的病歷號名單並且頻率為TID 或 檔名開頭為XX，則CorrectPatientName為空白

                    string correctPatientName = (_filterPatientNo.Contains(XMLHelper.GetParameterUseString("case", 0, "local_id")) &&
                        adminCode == "TID") || SourceFileNameWithoutExtension.StartsWith("XX") ? string.Empty : XMLHelper.GetParameterUseString("case/profile/person", 0, "name");

                    #endregion

                    _opd.Add(new JVServerXMLOPD()
                    {
                        MedicineCode = medicineCode,
                        MedicineName = medicineName,
                        PerQty = perQty.ToString("0.###"),
                        AdminCode = adminCode,
                        Days = days,
                        StartDay = startDate.ToString("yyMMdd"),
                        EndDay = endDate.ToString("yyMMdd"),
                        SumQty = Convert.ToSingle(node.Attributes["total_dose"].Value).ToString("0.###"),
                        CorrectPatientName = correctPatientName,
                        Memo = node.Attributes["memo"].Value
                    });
                    if (DateTime.Compare(maxEndDate, endDate) == -1)
                    {
                        maxEndDate = endDate;
                    }
                }
                _opd.ForEach(x => x.EndDay = maxEndDate.ToString("yyMMdd"));  //取結束日期天數最大者

                #region 若藥品代碼為 MEQ0 or U or AC42526100 單獨出現並頻率為BID，或是兩者同時出現並頻率為BID，則過濾

                var v1 = (from x in _opd
                          where x.AdminCode == "BID"
                          select x.MedicineCode).ToList();
                _opd.RemoveAll(x => v1.Count == 1 && (v1.Contains("U") || v1.Contains("MEQ0") || v1.Contains("AC42526100")) && x.AdminCode == "BID");
                _opd.RemoveAll(x => v1.Count == 2 && v1.Contains("U") && (v1.Contains("MEQ0") || v1.Contains("AC42526100")) && x.AdminCode == "BID");

                #endregion

                #region 若藥品代碼為 CIW0 or AC29798100 ，頻率為BID，符合的筆數 > 0 ，並整筆處方內頻率為 BID 的筆數 <= 1 ，則過濾

                var v2 = (from x in _opd
                          where (x.MedicineCode == "CIW0" || x.MedicineCode == "AC29798100") && x.AdminCode == "BID"
                          select x).Count();
                int bidCount = _opd.Where(x => x.AdminCode == "BID").Count();
                _opd.RemoveAll(x => v2 > 0 && bidCount <= 1 && x.AdminCode == "BID");

                #endregion

                if (_opd.Count == 0 || ComparePrescription.IsPrescriptionRepeat(SourceFilePath, _basic, _opd))
                {
                    Pass();
                    return;
                }
                Success();
            }
            catch (Exception ex)
            {
                ReadFileFail(ex);
            }
        }

        public override void LogicOPD()
        {
            for (int x = 0; x <= 14; x++)
                _oncubeRandom.Add("");
            if (ExtraRandom.Count != 0)  //將JVServer的Random放入OnCube的Radnom
            {
                foreach (var random in ExtraRandom)
                {
                    _oncubeRandom[Convert.ToInt32(random.OnCube)] = _jvsRandom[Convert.ToInt32(random.JVServer)];
                }
            }

            #region 鄭小兒的處方如果只有vk一個品項則過濾

            if (_basic.LocationName.Contains("鄭小兒"))
            {
                bool result = (from a in _opd
                               where a.MedicineCode == "VK"
                               select a).Count() > 0;
                if (result && _opd.Count == 1)
                {
                    Pass();
                    return;
                }
            }

            #endregion

            string outputDirectory = $@"{OutputDirectory}\{_basic.PatientName}-{_basic.Type}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.JVServerXML(_basic, _opd, outputDirectory, SourceFileNameWithoutExtension);
                Success();
            }
            catch (Exception ex)
            {
                GenerateOCSFileFail(ex);
            }
        }

        public override void ProcessUDBatch()
        {
            throw new NotImplementedException();
        }

        public override void LogicUDBatch()
        {
            throw new NotImplementedException();
        }

        public override void ProcessUDStat()
        {
            throw new NotImplementedException();
        }

        public override void LogicUDStat()
        {
            throw new NotImplementedException();
        }

        public override void ProcessPowder()
        {
            throw new NotImplementedException();
        }

        public override void LogicPowder()
        {
            throw new NotImplementedException();
        }

        public override void ProcessOther()
        {
            throw new NotImplementedException();
        }

        public override void LogicOther()
        {
            throw new NotImplementedException();
        }

        public override void ProcessCare()
        {
            throw new NotImplementedException();
        }

        public override void LogicCare()
        {
            throw new NotImplementedException();
        }

        private void ClearList()
        {
            _jvsRandom.Clear();
            _oncubeRandom.Clear();
        }

        public override ReturnsResultModel DepartmentShunt()
        {
            _basic = null;
            _basic = new JVServerXMLOPDBasic();
            _opd.Clear();
            ClearList();
            return base.DepartmentShunt();
        }
    }

    internal class JVServerXMLOPDBasic
    {
        public string PatientName { get; set; }
        public string LocationName { get; set; }
        public string HospitalName { get; set; }
        public string Type { get; set; }
    }

    internal class JVServerXMLOPD
    {
        public string MedicineCode { get; set; }
        public string MedicineName { get; set; }
        public string PerQty { get; set; }
        public string AdminCode { get; set; }
        public string Days { get; set; }
        public string CorrectPatientName { get; set; }
        public string SumQty { get; set; }
        public string StartDay { get; set; }
        public string EndDay { get; set; }
        public string Memo { get; set; }
    }
}
