using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCP.MVVM.Models.Enum
{
    public enum ConvertResult
    {
        成功,
        產生OCS失敗,
        讀取檔案失敗,
        處理邏輯失敗,
        全數過濾,
        沒有餐包頻率,
        沒有種包頻率
    }
}
