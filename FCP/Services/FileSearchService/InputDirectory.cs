using FCP.Models;
using FCP.src.Enum;
using FCP.src.Interface;
using System.Collections.Generic;

namespace FCP.Services.FileSearchService
{
    class InputDirectory : IGetFile
    {
        public eDepartment GetDepartment { get => _department; }
        private eDepartment _department;

        public string GetFile(List<string> extensionNames, List<MatchModel> matchModel)
        {
            return string.Empty;
        }
    }
}
