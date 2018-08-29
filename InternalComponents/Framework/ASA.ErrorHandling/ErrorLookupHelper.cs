using System;
using System.Collections.Generic;
using System.Text;
using ASA.Log.ServiceLogger;
using ASA.Common;

namespace ASA.ErrorHandling
{
    public class ErrorLookupHelper : IErrorLookupHelper
    {
        #region Fields

        public IASALog Log = ASALogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static private Dictionary<string, ExceptionData> exceptionREF_ExceptionError = null;
        static private Dictionary<string, string> exceptionRef_ErrorMap = null;
        static private Dictionary<string, string> exceptionREF_ExErrorForLookup = null;

        static private Boolean isInitialized = false;
        static ExceptionData exceptionData = null;

        //used in ASAExceptionTranslator class
        public bool bHelperAvailable = false;

        #endregion

        public ErrorLookupHelper()
        {

        }

        public ExceptionData GetExceptionData(string exceptionType)
        {
            //Check to see if maps initialized.  If not, do so
            if (!isInitialized)
            {
                Initialize();
            }

            bHelperAvailable = true;

            if (exceptionRef_ErrorMap != null && exceptionRef_ErrorMap.ContainsKey(exceptionType))
            {
                string sError_id = exceptionRef_ErrorMap[exceptionType];
                
                if (sError_id.StartsWith("GEN") != true)
                {
                    if (exceptionREF_ExceptionError != null && exceptionREF_ExceptionError.ContainsKey(sError_id))
                    {
                        exceptionData = new ExceptionData();
                        exceptionData.BusinessDescription = exceptionREF_ExceptionError[sError_id].BusinessDescription;
                        exceptionData.CodeDescription = exceptionREF_ExceptionError[sError_id].CodeDescription;
                        exceptionData.ExceptionError_id = exceptionREF_ExceptionError[sError_id].ExceptionError_id;
                        exceptionData.ExceptionType = exceptionREF_ExceptionError[sError_id].ExceptionType;
                        exceptionData.ShortDescription = exceptionREF_ExceptionError[sError_id].ShortDescription;
                    }
                }
            }
            if (exceptionData != null)
            {
                return exceptionData;
            }
            else
                return null;
        }

        /// <summary>
        /// Retrieve a single record from REF_ExceptionError table 
        /// </summary>
        /// <param name="errorCode">Error code whose info should be returned</param>
        /// <returns>Business description from Ref_ExceptionError</returns>
        public string GetExceptionDescriptionByErrorCode(string errorCode)
        {
            string description = "";

            //Check to see if maps initialized.  If not, do so
            if (!isInitialized)
            {
                Initialize();
            }

            if (exceptionREF_ExErrorForLookup == null)
            {
                description = "ASA Translation Tables are Unavailable";
            }
            else if (exceptionREF_ExErrorForLookup.ContainsKey(errorCode))
            {
                description = exceptionREF_ExErrorForLookup[errorCode].ToString();
            }
            else
            {
                description = "No Description Found";
            }

            return description;
        }
        /// <summary>
        /// QC Issue # 2123
        /// will retrieve the Ref_ExceptionError.BusinessDescription column value given the Ref_Exception.ExceptionErrorCode 
        /// from the logging database and add it to the List.
        /// </summary>
        /// <param name="repsonseMessageList">List that the exception error will be added to.</param>
        /// <param name="messageDetails">string which contains the message details to be added to the response message list</param>
        /// <returns>Boolean - true/message added, false/failed to add message to list, instead "No Description Found" error was added </returns>
        public bool AddMessageDetails(ResponseMessageList responseMessageList, string messageDetails)
        {
            Log.Debug("Entering AddMessage() ...");
            bool success = true;
            ResponseMessage responseMessage;
            if (messageDetails ==String.Empty)
            {
                Log.Error("message Details Argument is empty.");
                string Msg = String.Format("Message Details argument is empty. ");
                responseMessage = new ResponseMessage(Msg);
                success = false;
            }
            else
                responseMessage = new ResponseMessage(messageDetails);


            responseMessageList.Add(responseMessage);

            return success;
        }


        /// <summary>
        /// QC Issue # 2123
        /// will retrieve the Ref_ExceptionError.BusinessDescription column value given the Ref_Exception.ExceptionErrorCode 
        /// from the logging database and add it to the List.
        /// </summary>
        /// <param name="repsonseMessageList">List that the exception error will be added to.</param>
        /// <param name="exceptionErrorCode">value corresponding to the Ref_ExceptionError.ExceptionErrorCode in the logging database</param>
        /// <returns>Boolean - true/message added, false/failed to add message to list, instead "No Description Found" error was added </returns>
        public bool AddMessage(ResponseMessageList responseMessageList, string exceptionErrorCode)
        {
            Log.Debug("Entering AddMessage() ...");
            bool success= true;
            ResponseMessage responseMessage;

            string description = GetExceptionDescriptionByErrorCode(exceptionErrorCode);
            if (description == null)
            {
                Log.Error("IErrorLookupHelper.GetExceptionDescriptionByErrorCode returned an null description.");
                string Msg = String.Format("No Description Found for exception Error Code {0} ", exceptionErrorCode);
                responseMessage = new ResponseMessage(Msg);
                success = false;
            }
            else
                responseMessage=new ResponseMessage(exceptionErrorCode,description);

            
            responseMessageList.Add(responseMessage);

            return success;
        }

        /// <summary>
        /// QC Issue # 2123
        /// will retrieve the Ref_ExceptionError.BusinessDescription column value given the Ref_Exception.ExceptionErrorCode 
        /// from the logging database and add it to the List.
        /// </summary>
        /// <param name="repsonseMessageList">List that the exception error will be added to.</param>
        /// <param name="exceptionErrorCode">value corresponding to the Ref_ExceptionError.ExceptionErrorCode in the logging database</param>
        /// <param name="messageArgumentList">String Array holding the the message mrguments </param>
        /// <returns>Boolean - true/message added, false/failed to add message to list, instead "No Description Found" error was added </returns>
        public bool AddMessage(ResponseMessageList responseMessageList,string exceptionErrorCode,params object[] messageArgumentList)
        {
            Log.Debug("Entering AddMessage() ...");
            bool success = true;
            ResponseMessage responseMessage;

            string description = GetExceptionDescriptionByErrorCode(exceptionErrorCode);
            if (description == null)
            {
                Log.Error("IErrorLookupHelper.GetExceptionDescriptionByErrorCode returned an null description.");
                string Msg = String.Format("No Description Found for exception Error Code {0} ", exceptionErrorCode);
                responseMessage = new ResponseMessage(Msg);
                
                success= false;
            }
            else
            {
                String formatMsg;
                formatMsg = String.Format(description, messageArgumentList);
                responseMessage = new ResponseMessage(exceptionErrorCode, formatMsg);
            }

            responseMessageList.Add(responseMessage);

            return success;
        }


        #region  Initialize area
        /// <summary>
        /// Initalize data access and business entity objects.
        /// </summary>
        void Initialize()
        {

            try
            {
                ErrorExceptionMapExtractor errMapExtractor = new ErrorExceptionMapExtractor();

                //set ExceptionError dictionaries
                if (exceptionREF_ExceptionError == null || exceptionREF_ExErrorForLookup == null)
                {
                    errMapExtractor.get_ExceptionErrorMap(ref exceptionREF_ExceptionError, ref exceptionREF_ExErrorForLookup);
                }

                //set ErrorMap dictionary
                if (exceptionRef_ErrorMap == null)
                {
                    errMapExtractor.get_ErrorMap(ref exceptionRef_ErrorMap);
                }

                isInitialized = true;
            }
            catch (Exception ex)
            {
                isInitialized = false;
            }

        }

        #endregion  Initialize area

    }
}
