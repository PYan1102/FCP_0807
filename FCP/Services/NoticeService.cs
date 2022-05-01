using FCP.Models;
using System.Windows.Forms;

namespace FCP.Services
{
    public class NoticeService
    {
        public static void Notice(string tipTitle, string tipContent, ToolTipIcon tipIcon)
        {
            CommonModel.NotifyIcon.ShowBalloonTip(1000, tipTitle, tipContent, tipIcon);
        }
    }
}
