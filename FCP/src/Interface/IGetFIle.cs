using FCP.Models;
using FCP.src.Enum;
using System.Collections.Generic;

namespace FCP.src.Interface
{
    interface IGetFile
    {
        void GetFile(string extensionName, List<MatchModel> matchModel, List<string> inputDirectory);
    }
}
