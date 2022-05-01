using FCP.Models;
using System.Collections.Generic;

namespace FCP.src.Interface
{
    interface IGetFile
    {
        string GetFile(string extensionName, List<MatchModel> matchModel);
    }
}