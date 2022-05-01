using FCP.Models;
using FCP.src.Enum;
using FCP.src.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCP.Services.FileSearchService
{
    class FileNameHead : IGetFile
    {
        public string GetFile(string extensionName, List<MatchModel> matchModel)
        {
            foreach (var model in matchModel)
            {
                if (!model.Enabled)
                {
                    continue;
                }
                string[] files = Directory.GetFiles(model.InputDirectory, $"*.{extensionName}");
                if (files.Length > 0)
                {
                    return files[0];
                }
            }
            return string.Empty;
        }
    }
}
