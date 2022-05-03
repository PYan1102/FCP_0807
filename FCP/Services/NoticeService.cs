using FCP.Models;
using System.Windows.Forms;

namespace FCP.Services
{
    public class NoticeService
    {
        public static void Notice(object tipTitle, object tipContent, ToolTipIcon tipIcon)
        {
            CommonModel.NotifyIcon.ShowBalloonTip(1000, tipTitle.ToString(), tipContent.ToString(), tipIcon);
        }
    }
}
