using FCP.Models;
using FCP.src.Enum;
using System.Collections.Generic;

namespace FCP.src.Interface
{
    interface IGetFile
    {
        string GetFile(List<string> extensionNames, List<MatchModel> matchModel);
        eDepartment GetDepartment { get; }
    }
}