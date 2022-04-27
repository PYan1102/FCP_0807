using System;
using FCP.Models;

namespace FCP.src.Factory
{
    static class ConvertInfoactory
    {
        private static ConvertFileInformtaionModel _ConvertFileInformation { get; set; }
        public static ConvertFileInformtaionModel GenerateConvertFileInformation()
        {
            if (_ConvertFileInformation == null)
                _ConvertFileInformation = new ConvertFileInformtaionModel();
            return _ConvertFileInformation;
        }
    }
}
