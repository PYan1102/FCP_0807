using FCP.src.Enum;
using System.Collections.Generic;

namespace FCP.Models
{
    public class MatchModel
    {
        public List<string> InputDirectoryList { get; set; } = new List<string>();
        public eDepartment Department { get; set; }
        public string Rule { get; set; }
        public bool Enabled { get; set; } = false;
    }
}
