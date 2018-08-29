using System;
using System.Collections.Generic;
using System.Text;

namespace ASA.ErrorHandling
{
    /// <summary>
    /// Implementation of IASAExceptionTranslator that analyzes provider specific
    /// error codes and translates into the DAO exception hierarchy.
    /// </summary>
    /// <remarks>This class loads the obtains error codes from
    /// the ExceptionDB  
    /// which defines error code mappings for various projects.
    /// </remarks>
    /// <author>OS</author>

    public class ASAExceptionTranslator : IASAExceptionTranslator
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeExceptionTranslator"/> class.
        /// </summary>
        private ErrorLookupHelper errorLookupHelper;
        public ASAExceptionTranslator()
        {

        }

        /// <param name="exception">
        /// The Exception encountered by the ASA Application.
        /// </param>
        /// <returns>
        public ASAException Translate(Exception exception)
        {
            errorLookupHelper = new ErrorLookupHelper();

            ASAException translatedException = new ASAException();

            ExceptionData exceptionData = null;

            string exceptionType = exception.GetType().ToString();//.Substring(0, 32);
            if (exceptionType != null)
            {
                exceptionData = errorLookupHelper.GetExceptionData(exceptionType);
            }
            if (exceptionData != null)
            {
                translatedException.BusinessDescription = exceptionData.BusinessDescription;
                translatedException.CodeDescription = exceptionData.CodeDescription;
                translatedException.ExceptionType = exceptionData.ExceptionType;
                translatedException.ShortDescription = exceptionData.ShortDescription;
                translatedException.ExceptionError_id = exceptionData.ExceptionError_id;
            }
            else
            {
                if (errorLookupHelper.bHelperAvailable)
                {
                    translatedException.BusinessDescription = "No Description Found";
                    translatedException.CodeDescription = "No Description Found";
                    translatedException.ExceptionType = "ASAUnknownException";
                    translatedException.ShortDescription = "No Description Found";
                    translatedException.ExceptionError_id = "GEN0000001";
                }
                else
                {
                    translatedException.BusinessDescription = "ASA Translation Tables are unavailable";
                    translatedException.CodeDescription = "ASA Translation Tables are unavailable";
                    translatedException.ExceptionType = "ASA.ExcErrCodeUnavail";
                    translatedException.ShortDescription = "ASA Translation Tables are unavailable";
                    translatedException.ExceptionError_id = "GEN0000002";
                }
                //translatedException.ExceptionError_id = "";
            }
            translatedException.Original_Error_Type = exception.GetType().ToString();
            translatedException.Original_Message = exception.GetBaseException().Message;
            translatedException.Error_call_stack = exception.StackTrace;
            translatedException.Error_Source = exception.Source;

            translatedException.Original_Error_Type = exception.GetType().ToString();
            return translatedException;
        }


    }

}



