namespace FCP.Models
{
    internal class ActionResult : ModelBase
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }
}