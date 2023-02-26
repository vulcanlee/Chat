using CommonDomain.Models;
using CommonDomainLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Factories;

public static class VerifyRecordResultFactory
{
    /// <summary>
    /// 使用文字內容來回應
    /// </summary>
    /// <param name="success"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static VerifyRecordResult<T> Build<T>(string message, Exception exception = null)
    {
        VerifyRecordResult<T> verifyRecordResult = new()
        {
            Success = false,
            Message = message,
            Exception = exception,
        };
        return verifyRecordResult;
    }

    public static VerifyRecordResult<T> Build<T>(T result)
    {
        VerifyRecordResult<T> verifyRecordResult = new()
        {
            Success = true,
             Result = result,
        };
        return verifyRecordResult;
    }
}
