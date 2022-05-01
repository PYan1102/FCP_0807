using FCP.Models;
using FCP.src.Enum;
using System;
using System.Collections.Generic;
using System.IO;

namespace FCP.Services.FileSearchService
{
    public static class MatchDepartment
    {
        public static MatchModel Macth(List<MatchModel> matchModel, string sourceFilePath)
        {
            foreach (var model in matchModel)
            {
                if (!model.Enabled)
                {
                    continue;
                }
                if (Path.GetDirectoryName(sourceFilePath) == model.InputDirectory && (string.IsNullOrEmpty(model.Rule) || Path.GetFileNameWithoutExtension(sourceFilePath).StartsWith(model.Rule)))
                {
                    return model;
                }
            }
            return null;
        }
    }
}