using FCP.Models;
using FCP.src.Interface;
using System.Collections.Generic;
using System.IO;

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
