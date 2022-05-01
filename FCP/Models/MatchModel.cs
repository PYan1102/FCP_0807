using FCP.src.Enum;

namespace FCP.Models
{
    public sealed class MatchModel
    {
        public eDepartment Department { get; set; }
        public string Rule { get; set; }
        public bool Enabled { get; set; } = false;
        public string InputDirectory { get; set; }
    }
}
