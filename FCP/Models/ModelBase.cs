using System.Text;

namespace FCP.Models
{
    public abstract class ModelBase
    {
        public override string ToString()
        {
            var list = GetType().GetProperties();
            StringBuilder sb = new StringBuilder();
            foreach (var p in list)
            {
                sb.AppendLine($"{p.Name}: {p.GetValue(this)}");
            }
            return sb.ToString();
        }
    }
}
