using System;
using FCP.Models;
using FCP.src.Enum;

namespace FCP.src.Interface
{
    interface IRetunrsResult
    {
        void Shunt(eConvertResult result, object message = null);
        void SetReturnsResultModel(ReturnsResultModel format);
        void Success();
        void Pass(object message);
        void LostMultiAdminCode(string adminCode);
        void LostCombiAdminCode(string adminCode);
        void ReadFileFail(object message);
        void GenerateOCSFileFail(object message);
        void ProcessFileFail(object message);
    }
}
