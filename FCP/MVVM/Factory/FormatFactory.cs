using System;
using FCP.MVVM.Models.Enum;
using FCP.MVVM.FormatInit;

namespace FCP.MVVM.Factory
{
    public static class FormatFactory
    {
        private static FunctionCollections _Format { get; set; }
        private static Format _CurrentFormat { get; set; }

        public static FunctionCollections GenerateFormat(Format format)
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

        private static FunctionCollections GetFormat(Format format)
        {
            switch(format)
            {
                case Format.JVS:
                    return new BASE_JVServer();
                case Format.創聖系統TOC:
                    return new BASE_ChuangSheng();
                case Format.醫聖系統TOC:
                    return new BASE_YiSheng();
                case Format.小港醫院TOC:
                    return new BASE_XiaoGang();
                case Format.光田醫院TOC:
                    return new BASE_KuangTien();
                case Format.光田醫院TJVS:
                    return new BASE_KuangTien();
                case Format.民生醫院TOC:
                    return new BASE_MinSheng();
                case Format.宏彥診所TOC:
                    return new BASE_HongYen();
                case Format.義大醫院TOC:
                    return new BASE_E_DA();
                case Format.長庚磨粉TJVS:
                    return new BASE_ChangGung_POWDER();
                case Format.長庚醫院TOC:
                    return new BASE_ChangGung();
                case Format.台北看守所TOC:
                    return new BASE_TaipeiDetention();
                case Format.仁康醫院TOC:
                    return new BASE_JenKang();
                case Format.方鼎系統TOC:
                    return new BASE_FangDing();
                case Format.成祐中醫診所TOC:
                    return new BASE_ChengYu();
                case Format.OnCubeTOC:
                    return new BASE_OnCube();
                default:
                    throw new Exception($"沒有找到適當的格式 {format}");
            }
        }
    }
}
