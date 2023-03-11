using FCP.src.Enum;

namespace FCP.Models
{
    public sealed class ReturnsResultModel : ModelBase
    {
        public eConvertResult Result { get; set; }
        public string Message { get; set; }
    }
}
