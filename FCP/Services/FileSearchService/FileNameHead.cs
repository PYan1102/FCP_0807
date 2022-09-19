using FCP.Models;
using FCP.src.Enum;
using FCP.src.Interface;
using System.Collections.Generic;
using System.IO;

namespace FCP.Services.FileSearchService
{
    class FileNameHead : IGetFile
    {
        public eDepartment GetDepartment { get => _department; }
        private eDepartment _department;

        public string GetFile(string extensionName, List<MatchModel> matchModel)
        {
            _department = eDepartment.OPD;
            foreach (var model in matchModel)
            {
                if (!model.Enabled)
                {
                    continue;
                }
                string[] files = Directory.GetFiles(model.InputDirectory, $"*.{extensionName}");
                foreach (var file in files)
                {
                    if (Path.GetDirectoryName(file) == model.InputDirectory && (string.IsNullOrEmpty(model.Rule) ||
                        Path.GetFileNameWithoutExtension(file).StartsWith(model.Rule)))
                    {
                        _department = model.Department;
                        return file;
                    }
                }
            }
            return string.Empty;
        }
    }
}
