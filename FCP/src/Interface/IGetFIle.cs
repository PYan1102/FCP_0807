using FCP.Models;
using FCP.src.Enum;
using System.Collections.Generic;

namespace FCP.src.Interface
{
    interface IGetFile
    {
        string GetFile(string extensionName, List<MatchModel> matchModel);
        eDepartment GetDepartment { get; }
    }
}