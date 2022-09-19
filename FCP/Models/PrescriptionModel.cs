using System;

namespace FCP.Models
{
    class PrescriptionModel
    {
        public string PatientName { get; set; } = string.Empty;
        public string PatientNo { get; set; } = string.Empty;
        public string PrescriptionNo { get; set; } = string.Empty;
        public string BedNo { get; set; } = string.Empty;
        public string RoomNo { get; set; } = string.Empty;
        public string BirthDate { get; set; } = "2000-01-01";
        public string LocationName { get; set; } = string.Empty;
        public string HospitalName { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string AdminCode { get; set; } = string.Empty;
        public string AdminCodeDescription { get; set; } = string.Empty;
        public string MedicineCode { get; set; } = string.Empty;
        public string MedicineName { get; set; } = string.Empty;
        public int Days { get; set; } = 0;
        public float PerQty { get; set; } = 0;
        public float SumQty { get; set; } = 0;
        public DateTime StartDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        public DateTime EndDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
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
