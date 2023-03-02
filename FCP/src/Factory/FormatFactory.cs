using FCP.src.Enum;
using FCP.src.FormatInit;
using System;

namespace FCP.src.Factory
{
    static class FormatFactory
    {
        private static ConvertBase _format;
        private static eFormat _currentFormat;

        public static ConvertBase GenerateNewFormat(eFormat format)
        {
            if (_currentFormat != format)
            {
                _format = GetFormat(format);
                _currentFormat = format;
            }
            _format = _format ?? GetFormat(format);
            return _format;
        }

        private static ConvertBase GetFormat(eFormat format)
        {
            switch (format)
            {
                case eFormat.JVS:
                    return new BASE_JVServer();
                case eFormat.創聖系統OC:
                    return new BASE_ChuangSheng();
                case eFormat.醫聖系統OC:
                    return new BASE_YiSheng();
                case eFormat.光田醫院_沙鹿OC:
                    return new BASE_KuangTien(false);
                case eFormat.光田醫院_大甲OC:
                    return new BASE_KuangTien(true);
                case eFormat.光田醫院JVS:
                    return new BASE_KuangTien(null);
                case eFormat.民生醫院OC:
                    return new BASE_MinSheng();
                case eFormat.宏彥診所OC:
                    return new BASE_HongYen();
                case eFormat.義大醫院OC:
                    return new BASE_E_DA();
                case eFormat.台北看守所OC:
                    return new BASE_TaipeiDetention();
                case eFormat.仁康醫院OC:
                    return new BASE_JenKang();
                case eFormat.方鼎系統OC:
                    return new BASE_FangDing();
                case eFormat.成祐中醫診所OC:
                    return new BASE_ChengYu();
                case eFormat.OC:
                    return new BASE_OnCube();
                case eFormat.華盛頓藥局OC:
                    return new BASE_Washinton();
                case eFormat.小熊藥局OC:
                    return new BASE_LittleBear();
                case eFormat.吉安醫院OC:
                    return new BASE_JiAn();
                case eFormat.立群診所OC:
                    return new BASE_LiChiun();
                case eFormat.金鶯診所OC:
                    return new BASE_Elite();
                case eFormat.健通藥局OC:
                    return new BASE_JianTong();
                default:
                    throw new Exception($"沒有找到適配的格式 {format}");
            }
        }
    }
}