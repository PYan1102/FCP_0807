using System;

namespace FCP.MVVM.Factory
{
    static class LogFactory
    {
        private static Log _Log { get; set; }

        public static Log GenerateLog()
        {
            if (_Log == null)
                _Log = new Log();
            return _Log;
        }
    }
}
