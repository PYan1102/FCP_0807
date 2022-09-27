using FCP.src.Enum;

namespace FCP.Models
{
    public sealed class MatchModel
    {
        public eDepartment Department { get; set; }
        public string Rule { get; set; }
        public bool Enabled { get; set; } = false;
        public string InputDirectory { get; set; }

        public override string ToString()
        {
            return $@"
{nameof(Department)}: {Department}
{nameof(Rule)}: {Rule}
{nameof(Enabled)}: {Enabled}
{nameof(InputDirectory)}: {InputDirectory}";
        }
    }
}
