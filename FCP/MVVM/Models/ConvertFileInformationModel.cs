using System;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.Models
{
    public class ConvertFileInformtaionModel
    {
        public string GetInputPath { get => _InputPath; }
        public string GetOutputPath { get => _OutputPath; }
        public string GetFilePath { get => _FilePath; }
        public string GetCurrentSeconds { get => _CurrentSeconds; }
        public DepartmentEnum GetDepartment { get => _Department; }
        private string _InputPath { get; set; }
        private string _OutputPath { get; set; }
        private string _FilePath { get; set; }
        private string _CurrentSeconds { get; set; }
        private DepartmentEnum _Department { get; set; }

        public ConvertFileInformtaionModel SetInputPath(string outputPath)
        {
            _InputPath = outputPath;
            return this;
        }

        public ConvertFileInformtaionModel SetOutputPath(string inputPath)
        {
            _OutputPath = inputPath;
            return this;
        }

        public ConvertFileInformtaionModel SetFilePath(string filePath)
        {
            _FilePath = filePath;
            return this;
        }

        public ConvertFileInformtaionModel SetCurrentSeconds(string currentSeconds)
        {
            _CurrentSeconds = currentSeconds;
            return this;
        }

        public ConvertFileInformtaionModel SetDepartment(DepartmentEnum department)
        {
            _Department = department;
            return this;
        }
    }
}