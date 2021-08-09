using System;
using FCP.MVVM.Models;

namespace FCP.MVVM.Factory
{
    static class ConvertInformationFactory
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
