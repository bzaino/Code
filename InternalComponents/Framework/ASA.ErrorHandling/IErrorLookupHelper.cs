using System;
using System.Collections.Generic;
using System.Text;
using ASA.Common;

namespace ASA.ErrorHandling
{
    public interface IErrorLookupHelper
    {
        //ExceptionData GetExceptionData(string exceptionType);

        string GetExceptionDescriptionByErrorCode(string errorCode);
        
        /// QC Issue # 2123
        bool AddMessageDetails(ResponseMessageList responseMessageList, string messageDetails);
        bool AddMessage(ResponseMessageList responseMessageList, string exceptionErrorCode);
        bool AddMessage(ResponseMessageList responseMessageList, string exceptionErrorCode, params object[] messageArgumentList);
    }
}
