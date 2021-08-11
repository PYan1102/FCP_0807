using System;
using FCP.MVVM.Models;
using FCP.MVVM.Factory;
using FCP.MVVM.ViewModels.Interface;
using FCP.MVVM.Models.Enum;

namespace FCP.MVVM.ViewModels
{
    class ReturnsResult : IRetunrsResult
    {
        private ReturnsResultFormat _ReturnsResultFormat { get; set; }
        private ConvertFileInformtaionModel _ConvertFileInformation { get; set; }
        public ReturnsResult()
        {
            _ConvertFileInformation = ConvertInformationFactory.GenerateConvertFileInformation();
        }

        public void Shunt(ConvertResult result, string message)
        {
            switch (result)
            {
                case ConvertResult.成功:
                    Success();
                    break;
                case ConvertResult.全數過濾:
                    Filter();
                    break;
                case ConvertResult.沒有餐包頻率:
                    NoMultiAdminCode(message);
                    break;
                case ConvertResult.沒有種包頻率:
                    NoCombiAdminCode(message);
                    break;
                case ConvertResult.產生OCS失敗:
                    GenerateOCSFileFail(message);
                    break;
                case ConvertResult.讀取檔案失敗:
                    ReadFileFail(message);
                    break;
                case ConvertResult.處理邏輯失敗:
                    ProcessFileFail(message);
                    break;
            }
        }

        public void SetReturnsResultFormat(ReturnsResultFormat format)
        {
            _ReturnsResultFormat = format;
        }

        public void ReadFileFail(string exception = null)
        {
            if (exception != null)
                _ReturnsResultFormat.Message = $"{_ConvertFileInformation.GetFilePath} 讀取處方籤時發生問題 {exception}";
            else
                _ReturnsResultFormat.Message = $"{_ConvertFileInformation.GetFilePath} 讀取處方籤時發生問題";
            _ReturnsResultFormat.Result = ConvertResult.讀取檔案失敗;
        }

        public void Filter(string exception = null)
        {
            _ReturnsResultFormat.Message = string.Empty;
            _ReturnsResultFormat.Result = ConvertResult.全數過濾;
        }

        public void NoMultiAdminCode(string adminCode)
        {
            _ReturnsResultFormat.Message = $"{_ConvertFileInformation.GetFilePath} 在OnCube中未建置此餐包頻率 {adminCode} 的頻率";
            _ReturnsResultFormat.Result = ConvertResult.沒有餐包頻率;
        }

        public void Success()
        {
            _ReturnsResultFormat.Message = string.Empty;
            _ReturnsResultFormat.Result = ConvertResult.成功;
        }

        public void NoCombiAdminCode(string adminCode)
        {
            _ReturnsResultFormat.Message = $"{_ConvertFileInformation.GetFilePath} 在OnCube中未建置此種包頻率 S{adminCode} 的頻率";
            _ReturnsResultFormat.Result = ConvertResult.沒有種包頻率;
        }

        public void GenerateOCSFileFail(string exception = null)
        {
            _ReturnsResultFormat.Message = $"{_ConvertFileInformation.GetFilePath} 產生OCS時發生問題";
            _ReturnsResultFormat.Result = ConvertResult.產生OCS失敗;
        }

        public void ProcessFileFail(string exception = null)
        {
            if (exception != null)
                _ReturnsResultFormat.Message = $"{_ConvertFileInformation.GetFilePath} 處理邏輯時發生問題 {exception}";
            else
                _ReturnsResultFormat.Message = $"{_ConvertFileInformation.GetFilePath} 處理邏輯時發生問題";
            _ReturnsResultFormat.Result = ConvertResult.處理邏輯失敗;
        }
    }
}
