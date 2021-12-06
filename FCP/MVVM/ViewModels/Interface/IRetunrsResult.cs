using System;
using FCP.MVVM.Models;
using FCP.src.Enum;

namespace FCP.MVVM.ViewModels.Interface
{
    interface IRetunrsResult
    {
        void Shunt(eConvertResult result, string message);
        void SetReturnsResultFormat(ReturnsResultFormat format);
        void Success();
        void Filter(string exception);
        void NoMultiAdminCode(string adminCode);
        void NoCombiAdminCode(string adminCode);
        void ReadFileFail(string exception);
        void GenerateOCSFileFail(string exception);
        void ProcessFileFail(string exception);
    }
}
