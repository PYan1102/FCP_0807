using System;
using FCP.src.Enum;
using FCP.src.FormatInit;

namespace FCP.src.Factory
{
    static class FormatFactory
    {
        private static FunctionCollections _Format { get; set; }
        private static eFormat _CurrentFormat { get; set; }

        public static FunctionCollections GenerateFormat(eFormat format)
        {
            if (_CurrentFormat != format)
            {
                _Format = GetFormat(format);
                _CurrentFormat = format;
            }
            if (_Format == null)
                _Format = GetFormat(format);
            return _Format;
        }

        private static FunctionCollections GetFormat(eFormat format)
        {
            switch(format)
            {
                case eFormat.JVS:
                    return new BASE_JVServer();
                case eFormat.創聖系統TOC:
                    return new BASE_ChuangSheng();
                case eFormat.醫聖系統TOC:
                    return new BASE_YiSheng();
                case eFormat.小港醫院TOC:
                    return new BASE_XiaoGang();
                case eFormat.光田醫院TOC:
                    return new BASE_KuangTien();
                case eFormat.光田醫院TJVS:
                    return new BASE_KuangTien();
                case eFormat.民生醫院TOC:
                    return new BASE_MinSheng();
                case eFormat.宏彥診所TOC:
                    return new BASE_HongYen();
                case eFormat.義大醫院TOC:
                    return new BASE_E_DA();
                case eFormat.長庚磨粉TJVS:
                    return new BASE_ChangGung_POWDER();
                case eFormat.長庚醫院TOC:
                    return new BASE_ChangGung();
                case eFormat.台北看守所TOC:
                    return new BASE_TaipeiDetention();
                case eFormat.仁康醫院TOC:
                    return new BASE_JenKang();
                case eFormat.方鼎系統TOC:
                    return new BASE_FangDing();
                case eFormat.成祐中醫診所TOC:
                    return new BASE_ChengYu();
                case eFormat.OnCubeTOC:
                    return new BASE_OnCube();
                case eFormat.JVS_XML:
                    return new BASE_JVServer_XML();
                default:
                    throw new Exception($"沒有找到適當的格式 {format}");
            }
        }
    }
}
