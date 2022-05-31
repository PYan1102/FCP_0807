using FCP.Models;
using FCP.src.Enum;
using FCP.src.FormatControl;
using FCP.src.MessageManager;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace FCP.src.FormatInit
{
    class BASE_ChuangSheng : FormatBase
    {
        private FMT_ChuangSheng _format;

        public override void Init()
        {
            base.Init();
            WeakReferenceMessenger.Default.Send(new SetMainWindowToogleCheckedChangeMessage(new MainWindowModel.ToogleModel() { Toogle1 = true }));
        }

        public override ActionResult PrepareStart()
        {
            SetFileSearchMode(eFileSearchMode.根據檔名開頭);
            SetOPDRule();
            return base.PrepareStart();
        }

        public override void Converter()
        {
            _format = _format ?? new FMT_ChuangSheng();
            var result = _format.DepartmentShunt();
            Result(result, true);
        }
    }
}
