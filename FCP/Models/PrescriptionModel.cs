using System;

namespace FCP.Models
{
    class PrescriptionModel
    {
        /// <summary>
        /// 病患名稱
        /// </summary>
        public string PatientName { get; set; } = string.Empty;
        /// <summary>
        /// 病歷號
        /// </summary>
        public string PatientNo { get; set; } = string.Empty;
        /// <summary>
        /// 處方籤號
        /// </summary>
        public string PrescriptionNo { get; set; } = string.Empty;
        /// <summary>
        /// 領藥號
        /// </summary>
        public string GetMedicineNo { get; set; } = string.Empty;
        /// <summary>
        /// 床號
        /// </summary>
        public string BedNo { get; set; } = string.Empty;
        /// <summary>
        /// 房號
        /// </summary>
        public string RoomNo { get; set; } = string.Empty;
        /// <summary>
        /// 生日
        /// </summary>
        public string BirthDate { get; set; } = "2000-01-01";
        /// <summary>
        /// 位置名稱
        /// </summary>
        public string LocationName { get; set; } = string.Empty;
        /// <summary>
        /// 醫院名稱
        /// </summary>
        public string HospitalName { get; set; } = string.Empty;
        /// <summary>
        /// 科別
        /// </summary>
        public string Class { get; set; } = string.Empty;
        /// <summary>
        /// 頻率
        /// </summary>
        public string AdminCode { get; set; } = string.Empty;
        /// <summary>
        /// 頻率敘述
        /// </summary>
        public string AdminCodeDescription { get; set; } = string.Empty;
        /// <summary>
        /// 藥品代碼
        /// </summary>
        public string MedicineCode { get; set; } = string.Empty;
        /// <summary>
        /// 藥品名稱
        /// </summary>
        public string MedicineName { get; set; } = string.Empty;
        /// <summary>
        /// 天數
        /// </summary>
        public int Days { get; set; } = 0;
        /// <summary>
        /// 單次劑量
        /// </summary>
        public float PerQty { get; set; } = 0;
        /// <summary>
        /// 總量
        /// </summary>
        public float SumQty { get; set; } = 0;
        /// <summary>
        /// 開始日期
        /// </summary>
        public DateTime StartDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        /// <summary>
        /// 結束日期
        /// </summary>
        public DateTime EndDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        /// <summary>
        /// 藥品單位
        /// </summary>
        public string Unit { get; set; } = string.Empty;
        public string Other1 { get; set; } = string.Empty;
        public string Other2 { get; set; } = string.Empty;
        public string Other3 { get; set; } = string.Empty;
        public string Other4 { get; set; } = string.Empty;
        public string Other5 { get; set; } = string.Empty;
        public string Other6 { get; set; } = string.Empty;
        public string Other7 { get; set; } = string.Empty;
        public string Other8 { get; set; } = string.Empty;
        public string Other9 { get; set; } = string.Empty;
        public string Other10 { get; set; } = string.Empty;
    }
}
