using FCP.src.FormatControl;
using FCP.src.Enum;
using Microsoft.Toolkit.Mvvm.Messaging;
using FCP.src.MessageManager;
using FCP.Models;

namespace FCP.src.FormatInit
{
    class BASE_JVServer : FormatBase
    {
        private FMT_JVServer _format;

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
            _format = _format ?? new FMT_JVServer();
            var result = _format.DepartmentShunt();
            Result(result, true);
        }
    }
}
