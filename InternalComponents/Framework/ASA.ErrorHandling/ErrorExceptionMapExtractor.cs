using System;
using System.Collections.Generic;
using System.Text;

//ASA Data Access required namespaces
using ASA.Common;
using log4net;
//ASA specific
using ASA.BusinessEntities.SEL;
using ASA.DataAccess.SEL.Interfaces;

namespace ASA.ErrorHandling
{
    public class ErrorExceptionMapExtractor
    {
        static ISELDao ehProxy = (ISELDao)ContextHelper.GetContextObject("SELDAO");

        public void get_ExceptionErrorMap(ref Dictionary<string, ExceptionData> exceptionREF_ExceptionError, ref Dictionary<string, string> exceptionREF_ExErrorForLookup)
        {
            try
            {
                List<RefExceptionError> retListExceptionError = new List<RefExceptionError>();
                ehProxy.GetReferenceList(ASA.Common.SortDirection.Descending, "ExceptionErrorid", ref retListExceptionError);

                exceptionREF_ExceptionError = new Dictionary<string, ExceptionData>();
                exceptionREF_ExErrorForLookup = new Dictionary<string, string>();

                for (int i = 0; i < retListExceptionError.Count; i++)
                {
                    ExceptionData exceptionError = new ExceptionData();

                    string sErrorId = retListExceptionError[i].ExceptionErrorid;
                    exceptionError.ExceptionError_id = retListExceptionError[i].ExceptionErrorid;
                    exceptionError.ExceptionType = retListExceptionError[i].CustomExceptionType;
                    exceptionError.ShortDescription = retListExceptionError[i].ErrorShortDescription;
                    exceptionError.CodeDescription = retListExceptionError[i].SystemDescription;
                    if (retListExceptionError[i].BusinessDescription == "")
                    {
                        exceptionError.BusinessDescription = default(string);
                    }
                    else
                    {
                        exceptionError.BusinessDescription = retListExceptionError[i].BusinessDescription;
                    }

                    //create 2 dictionary objects.  One for use with ErrorMap (exceptionREF_ExceptionError) 
                    //and one to look up and description based on Error Code (exceptionREF_ExErrorForLookup)
                    exceptionREF_ExceptionError.Add(sErrorId, exceptionError);
                    exceptionREF_ExErrorForLookup.Add(sErrorId, exceptionError.BusinessDescription);

                }
            }
            catch (Exception exception)
            {
                ASAException translatedException = handleTablesNotAvailable(exception);
                throw translatedException;
            }
        }

        public void get_ErrorMap(ref Dictionary<string, string> exceptionRef_ErrorMap)
        {
            try
            {
                List<ExpErrorMap> retListErrorMap = new List<ExpErrorMap>();
                ehProxy.GetReferenceList(ASA.Common.SortDirection.Descending, "ErrorMapid", ref retListErrorMap);

                exceptionRef_ErrorMap = new Dictionary<string, string>();

                for (int i = 0; i < retListErrorMap.Count; i++)
                {
                    string sysErrorCode = retListErrorMap[i].SystemExceptionType.ToString();
                    string ASAErrorCode = retListErrorMap[i].ExceptionErrorid;

                    exceptionRef_ErrorMap.Add(sysErrorCode, ASAErrorCode);
                }
            }

            catch (Exception exception)
            {
                ASAException translatedException = handleTablesNotAvailable(exception);
                throw translatedException;
            }
        }

        /// <summary>
        /// Method to build translated error to throw when having issue retrieving ref data 
        /// </summary>
        /// <param name="ex">Original Exception</param>
        /// <returns>ASAException object that will be translated to SOAP fault</returns>
        private ASAException handleTablesNotAvailable(Exception ex)
        {
            ASAException translatedException = new ASAException();

            translatedException.BusinessDescription = "ASA Translation Tables are unavailable";
            translatedException.CodeDescription = "ASA Translation Tables are unavailable";
            translatedException.ExceptionType = "ASA.ExcErrCodeUnavail";
            translatedException.ShortDescription = "ASA Translation Tables are unavailable";

            translatedException.ExceptionError_id = "GEN0000002";

            translatedException.Original_Error_Type = ex.GetType().ToString();
            translatedException.Original_Message = ex.Message;
            translatedException.Error_call_stack = ex.StackTrace;
            translatedException.Error_Source = ex.Source;

            translatedException.Original_Error_Type = ex.GetType().ToString();

            return translatedException;
        }
    }

}

