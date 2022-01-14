using FCP.MVVM.Models;
using FCP.src.Enum;
using Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FCP.src.FormatControl
{
    class FMT_JVServer_XML : FormatCollection
    {
        private List<string> _FilterPatientNo = new List<string>() { "0058145","0051572", "0094562", "0069489", "0080950", "0051640", "0085193",
        "0057404", "0096220"};
        private JVServerXMLOPDBasic _Basic { get; set; }
        private List<JVServerXMLOPD> _OPD = new List<JVServerXMLOPD>();
        private List<string> _JVServerRandom = new List<string>();
        private List<string> _OnCubeRandom = new List<string>();

        public override bool ProcessOPD()
        {
            if (!File.Exists(FilePath))
            {
                Log.Write(FilePath + "忽略");
                ReturnsResult.Shunt(eConvertResult.全數過濾, null);
                return false;
            }
            try
            {
                ClearList();
                XMLHelper.Load(FilePath);
                DateTime startDate = DateTimeHelper.Convert(XMLHelper.GetParameterUseString("case", 0, "date"), "yyyyMMdd");

                //檔名開頭為XX時病患名稱設為空白
                _Basic.PatientName = XMLHelper.GetParameterUseString("case/profile/person", 0, "name");
                _Basic.Type = XMLHelper.GetParameterUseString("case/insurance", 0, "insurance_type_text") == "自費診" ? "自費-" : "健保-";
                _Basic.Type += XMLHelper.GetParameterUseString("case/insurance", 0, "labeno_type");
                _Basic.Type = _Basic.Type.Replace("*", "+")
                    .Replace("\\", "＼");
                string pattern = "[\\[\\]\\^\\**×―(^)$%!@#$…&%￥—=<>《》/|!！??？:：•'`·、。，；,;\"‘’“”-]";
                _Basic.Type = Regex.Replace(_Basic.Type, pattern, "");
                _Basic.LocationName = XMLHelper.GetParameterUseString("case/study", 0, "doctor_id") == "3539031772" ? "蔡鼎族診所" : "鄭小兒診所";
                _Basic.HospitalName = "華盛頓虎尾藥局";
                DateTime maxEndDate = startDate;
                foreach (System.Xml.XmlNode node in XMLHelper.GetNodeList("case/orders/item"))
                {
                    string days = XMLHelper.GetParameterUseString("case/orders/item", 0, "days");
                    DateTime endDate = startDate.AddDays(Convert.ToInt32(days) - 1);
                    string medicineCode = node.Attributes["local_code"].Value;
                    string adminCode = node.Attributes["freq"].Value;
                    adminCode = adminCode == "2" ? "BID" : adminCode == "3" ? "TID" : adminCode == "4" ? "QID" : adminCode;
                    string medicineName = node.Attributes["desc"].Value.Replace("&quot;", "''");
                    bool isNeedToPack = Convert.ToSingle(node.Attributes["divided_dose"].Value) % 0.5 == 0 || medicineCode == "BRO";  //單次劑量被0.5整除則包出，否則不包
                    if (!isNeedToPack)
                    {
                        ReturnsResult.Shunt(eConvertResult.全數過濾, null);
                        return false;
                    }
                    bool isPowder = node.Attributes["memo"].Value == "P";  //P為磨粉
                    if (IsFilterMedicineCode(medicineCode) || IsFilterAdminCode(adminCode) || isPowder || NeedFilterMedicineCode(medicineCode))
                        continue;
                    if (!IsExistsMultiAdminCode(adminCode))
                    {
                        Log.Write($"{FilePath} 在OnCube中未建置此餐包頻率 {adminCode}");
                        ReturnsResult.Shunt(eConvertResult.沒有餐包頻率, adminCode);
                        return false;
                    }
                    float perQty = Convert.ToSingle(node.Attributes["divided_dose"].Value);
                    int memoQty = node.Attributes["memo"].Value.ToString().Length == 0 ? 0 : Int32.TryParse(node.Attributes["memo"].Value, out int qty) ? qty : 0;
                    if (_Basic.LocationName == "鄭小兒診所" && memoQty > 0)
                    {
                        List<string> adminCodeList = GetMultiAdminCodeTimes(adminCode);
                        days = (memoQty / adminCodeList.Count / perQty).ToString();
                        endDate = startDate.AddDays(Convert.ToInt32(days) - 1);
                    }

                    #region 符合輸出空白病患名稱的病歷號名單並且頻率為TID 或 檔名開頭為XX，則CorrectPatientName為空白

                    string correctPatientName = (_FilterPatientNo.Contains(XMLHelper.GetParameterUseString("case", 0, "local_id")) &&
                        adminCode == "TID") || Path.GetFileNameWithoutExtension(FilePath).StartsWith("XX") ? string.Empty : XMLHelper.GetParameterUseString("case/profile/person", 0, "name");

                    #endregion

                    _OPD.Add(new JVServerXMLOPD()
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
                _OPD.ForEach(x => x.EndDay = maxEndDate.ToString("yyMMdd"));  //取結束日期天數最大者
                #region 若藥品代碼為MEQ0 or U單獨出現並頻率為BID，或是兩者同時出現並頻率為BID，則過濾
                var v1 = (from x in _OPD
                          where x.AdminCode == "BID"
                          select x.MedicineCode).ToList();
                if (v1.Count == 1 && (v1.Contains("U") || v1.Contains("MEQ0") || v1.Contains("AC42526100")))
                {
                    _OPD.RemoveAll(x => x.MedicineCode == v1[0] && x.AdminCode == "BID");
                }
                else if (v1.Count == 2 && v1.Contains("U") && (v1.Contains("MEQ0") || v1.Contains("AC42526100")))
                {
                    _OPD.RemoveAll(x => v1.Contains(x.MedicineCode) && x.AdminCode == "BID");
                }
                #endregion
                var v2 = (from x in _OPD
                          where (x.MedicineCode == "CIW0" || x.MedicineCode == "AC29798100") && x.AdminCode == "BID"
                          select x).Count();
                if (v2 > 0 && _OPD.Where(x => x.AdminCode == "BID").Count() <= 1)
                {
                    _OPD = _OPD.Where(x => x.AdminCode != "BID").Select(x => x).ToList();
                }

                if (_OPD.Count == 0 || ComparePrescription.IsPrescriptionRepeat(FilePath, _Basic, _OPD))
                {
                    ReturnsResult.Shunt(eConvertResult.全數過濾, null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(eConvertResult.讀取檔案失敗, ex.ToString());
                return false;
            }
        }

        public override bool LogicOPD()
        {
            for (int x = 0; x <= 14; x++)
                _OnCubeRandom.Add("");
            if (!string.IsNullOrEmpty(SettingsModel.ExtraRandom))  //將JVServer的Random放入OnCube的Radnom
            {
                int head;
                int middle;
                string[] randomList = SettingsModel.ExtraRandom.Split(',');
                foreach (string s in randomList)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        head = s.IndexOf(':');
                        middle = s.IndexOf("&");
                        _OnCubeRandom[Int32.Parse(s.Substring(middle + 1, s.Length - middle - 1)) - 1] = _JVServerRandom[Int32.Parse(s.Substring(head + 1, middle - head - 1))];
                    }
                }
            }

            #region 鄭小兒的處方如果只有vk一個品項則過濾

            if (_Basic.LocationName.Contains("鄭小兒"))
            {
                bool result = (from a in _OPD
                               where a.MedicineCode == "VK"
                               select a).Count() > 0;
                if (result && _OPD.Count == 1)
                {
                    ReturnsResult.Shunt(eConvertResult.全數過濾, null);
                    return false;
                }
            }

            #endregion

            string filePathOutput = $@"{OutputPath}\{_Basic.PatientName}-{_Basic.Type}_{CurrentSeconds}.txt";
            try
            {
                OP_OnCube.JVServerXML(_Basic, _OPD, filePathOutput, Path.GetFileNameWithoutExtension(FilePath));
                return true;
            }
            catch (Exception ex)
            {
                Log.Write($"{FilePath} {ex}");
                ReturnsResult.Shunt(eConvertResult.產生OCS失敗, ex.ToString());
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

        private void ClearList()
        {
            _JVServerRandom.Clear();
            _OnCubeRandom.Clear();
        }

        public override ReturnsResultFormat MethodShunt()
        {
            _Basic = null;
            _Basic = new JVServerXMLOPDBasic();
            _OPD.Clear();
            return base.MethodShunt();
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
