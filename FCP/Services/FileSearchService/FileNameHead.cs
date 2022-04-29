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
        public void GetFile(string extensionName, List<MatchModel> matchModel, List<string> inputDirectory)
        {
            foreach (var v in inputDirectory)
            {
                //string[] files = Directory.GetFiles(v.Value, $"*.{extensionName}");
                //if (files.Length > 0)
                //{
                //    FileInfoModel.SourceFilePath = files[0];
                //    FileInfoModel.InputDirectory = v.Value;
                //    FileInfoModel.CurrentDateTime = DateTime.Now;
                //    FileInfoModel.Department = v.Key;
                //    return;
                //}
            }
        }
    }
}
