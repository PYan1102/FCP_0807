using System;
using FCP.Models;
using FCP.src.Factory;
using FCP.src.Interface;
using FCP.src.Enum;
using Helper;

namespace FCP.src
{
    internal class ReturnsResult : IRetunrsResult
    {
        private ReturnsResultModel _returnsResultFormat { get; set; }
        public ReturnsResult()
        {
        }

        public void Shunt(eConvertResult convertResult, object message = null)
        {
            switch (convertResult)
            {
                case eConvertResult.成功:
                    Success();
                    break;
                case eConvertResult.全數過濾:
                    Pass();
                    break;
                case eConvertResult.缺少餐包頻率:
                    LostMultiAdminCode(message.ToString());
                    break;
                case eConvertResult.缺少種包頻率:
                    LostCombiAdminCode(message.ToString());
                    break;
                case eConvertResult.讀取檔案失敗:
                    ReadFileFail(message);
                    break;
                case eConvertResult.處理邏輯失敗:
                    ProcessFileFail(message);
                    break;
                case eConvertResult.產生OCS失敗:
                    GenerateOCSFileFail(message);
                    break;
                default:
                    throw new Exception($"未找到適配的 {convertResult}");
            }
            LogService.Info(_returnsResultFormat.Message);
        }

        public void Success()
        {
            _returnsResultFormat.Message = $"{FileInfoModel.SourceFilePath} {nameof(eConvertResult.成功)}";
            _returnsResultFormat.Result = eConvertResult.成功;
        }

        public void Pass(object message = null)
        {
            _returnsResultFormat.Message = $"{FileInfoModel.SourceFilePath} {nameof(eConvertResult.全數過濾)}";
            _returnsResultFormat.Result = eConvertResult.全數過濾;
        }

        public void LostMultiAdminCode(string adminCode)
        {
            _returnsResultFormat.Message = $"{FileInfoModel.SourceFilePath} 在OnCube中未建置此餐包頻率 {adminCode} 的頻率";
            _returnsResultFormat.Result = eConvertResult.缺少餐包頻率;
        }

        public void LostCombiAdminCode(string adminCode)
        {
            _returnsResultFormat.Message = $"{FileInfoModel.SourceFilePath} 在OnCube中未建置此種包頻率 S{adminCode} 的頻率";
            _returnsResultFormat.Result = eConvertResult.缺少種包頻率;
        }

        public void SetReturnsResultModel(ReturnsResultModel returnsResultModel)
        {
            _returnsResultFormat = returnsResultModel;
        }

        public void ReadFileFail(object message = null)
        {
            if (message != null)
                _returnsResultFormat.Message = $"{FileInfoModel.SourceFilePath} 讀取處方籤時發生問題 {message}";
            else
                _returnsResultFormat.Message = $"{FileInfoModel.SourceFilePath} 讀取處方籤時發生問題";
            _returnsResultFormat.Result = eConvertResult.讀取檔案失敗;
        }

        public void ProcessFileFail(object message = null)
        {
            if (message != null)
                _returnsResultFormat.Message = $"{FileInfoModel.SourceFilePath} 處理邏輯時發生問題 {message}";
            else
                _returnsResultFormat.Message = $"{FileInfoModel.SourceFilePath} 處理邏輯時發生問題";
            _returnsResultFormat.Result = eConvertResult.處理邏輯失敗;
        }

        public void GenerateOCSFileFail(object message = null)
        {
            _returnsResultFormat.Message = $"{FileInfoModel.SourceFilePath} 產生OCS時發生問題";
            _returnsResultFormat.Result = eConvertResult.產生OCS失敗;
        }
    }
}
