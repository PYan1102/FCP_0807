using FCP.Models;
using FCP.src.Enum;
using FCP.src.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FCP.Services.FileSearchService
{
    class FileNameHead : IGetFile
    {
        public eDepartment GetDepartment { get => _department; }
        private eDepartment _department;

        public string GetFile(List<string> extensionNames, List<MatchModel> matchModel)
        {
            _department = eDepartment.OPD;
            foreach (var model in matchModel)
            {
                if (!model.Enabled)
                {
                    continue;
                }
                string[] files = Directory.GetFiles(model.InputDirectory, "*.*").Where(x =>
                {
                    bool match = extensionNames.Where(y => x.ToLower().EndsWith(y.ToLower())).Count() > 0;
                    return match;
                }).ToArray();
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
