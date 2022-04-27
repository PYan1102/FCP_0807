using System;
using FCP.Models;
using FCP.src.Factory;
using FCP.ViewModels.Interface;
using FCP.src.Enum;

namespace FCP.src
{
    class ReturnsResult : IRetunrsResult
    {
        private ReturnsResultFormat _ReturnsResultFormat { get; set; }
        private ConvertFileInformtaionModel _ConvertFileInformation { get; set; }
        public ReturnsResult()
        {
            _ConvertFileInformation = ConvertInfoactory.GenerateConvertFileInformation();
        }

        public void Shunt(eConvertResult result, string message)
        {
            switch (result)
            {
                case eConvertResult.成功:
                    Success();
                    break;
                case eConvertResult.全數過濾:
                    Filter();
                    break;
                case eConvertResult.沒有餐包頻率:
                    NoMultiAdminCode(message);
                    break;
                case eConvertResult.沒有種包頻率:
                    NoCombiAdminCode(message);
                    break;
                case eConvertResult.產生OCS失敗:
                    GenerateOCSFileFail(message);
                    break;
                case eConvertResult.讀取檔案失敗:
                    ReadFileFail(message);
                    break;
                case eConvertResult.處理邏輯失敗:
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
            _ReturnsResultFormat.Result = eConvertResult.讀取檔案失敗;
        }

        public void Filter(string exception = null)
        {
            _ReturnsResultFormat.Message = string.Empty;
            _ReturnsResultFormat.Result = eConvertResult.全數過濾;
        }

        public void NoMultiAdminCode(string adminCode)
        {
            _ReturnsResultFormat.Message = $"{_ConvertFileInformation.GetFilePath} 在OnCube中未建置此餐包頻率 {adminCode} 的頻率";
            _ReturnsResultFormat.Result = eConvertResult.沒有餐包頻率;
        }

        public void Success()
        {
            _ReturnsResultFormat.Message = string.Empty;
            _ReturnsResultFormat.Result = eConvertResult.成功;
        }

        public void NoCombiAdminCode(string adminCode)
        {
            _ReturnsResultFormat.Message = $"{_ConvertFileInformation.GetFilePath} 在OnCube中未建置此種包頻率 S{adminCode} 的頻率";
            _ReturnsResultFormat.Result = eConvertResult.沒有種包頻率;
        }

        public void GenerateOCSFileFail(string exception = null)
        {
            _ReturnsResultFormat.Message = $"{_ConvertFileInformation.GetFilePath} 產生OCS時發生問題";
            _ReturnsResultFormat.Result = eConvertResult.產生OCS失敗;
        }

        public void ProcessFileFail(string exception = null)
        {
            if (exception != null)
                _ReturnsResultFormat.Message = $"{_ConvertFileInformation.GetFilePath} 處理邏輯時發生問題 {exception}";
            else
                _ReturnsResultFormat.Message = $"{_ConvertFileInformation.GetFilePath} 處理邏輯時發生問題";
            _ReturnsResultFormat.Result = eConvertResult.處理邏輯失敗;
        }
    }
}
