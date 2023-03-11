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
        private ReturnsResultModel _returnsResultModel;

        public void SetReturnsResultModel(ReturnsResultModel returnsResultModel)
        {
            _returnsResultModel = returnsResultModel;
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
        }

        public void Success()
        {
            _returnsResultModel.Message = $"{FileInfoModel.SourceFilePath} {nameof(eConvertResult.成功)}";
            _returnsResultModel.Result = eConvertResult.成功;
            LogService.Info(_returnsResultModel.Message);
        }

        public void Pass(object message = null)
        {
            _returnsResultModel.Message = $"{FileInfoModel.SourceFilePath} {nameof(eConvertResult.全數過濾)}";
            _returnsResultModel.Result = eConvertResult.全數過濾;
            LogService.Info(_returnsResultModel.Message);
        }

        public void LostMultiAdminCode(string adminCode)
        {
            _returnsResultModel.Message = $"{FileInfoModel.SourceFilePath} 在OnCube中未建置此餐包頻率 {adminCode} 的頻率";
            _returnsResultModel.Result = eConvertResult.缺少餐包頻率;
            LogService.Exception(_returnsResultModel.Message);
        }

        public void LostCombiAdminCode(string adminCode)
        {
            _returnsResultModel.Message = $"{FileInfoModel.SourceFilePath} 在OnCube中未建置此種包頻率 S{adminCode} 的頻率";
            _returnsResultModel.Result = eConvertResult.缺少種包頻率;
            LogService.Exception(_returnsResultModel.Message);
        }

        public void ReadFileFail(object message)
        {
            Exception ex = message as Exception;
            _returnsResultModel.Message = $"{FileInfoModel.SourceFilePath} 讀取處方籤時發生問題 {ex.Message}";
            _returnsResultModel.Result = eConvertResult.讀取檔案失敗;
            LogService.Exception(ex);
        }

        public void ProcessFileFail(object message)
        {
            Exception ex = message as Exception;
            _returnsResultModel.Message = $"{FileInfoModel.SourceFilePath} 處理邏輯時發生問題 {ex.Message}";
            _returnsResultModel.Result = eConvertResult.處理邏輯失敗;
            LogService.Exception(ex);
        }

        public void GenerateOCSFileFail(object message)
        {
            Exception ex = message as Exception;
            _returnsResultModel.Message = $"{FileInfoModel.SourceFilePath} 產生OCS時發生問題 {ex.Message}";
            _returnsResultModel.Result = eConvertResult.產生OCS失敗;
            LogService.Exception(ex);
        }
    }
}
