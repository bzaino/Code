#region Imports

using System;
using System.Runtime.Serialization;
using System.Text;
#endregion

namespace ASA.ErrorHandling
{
    /// <summary>
    /// Root of the hierarchy of ASA exceptions
    /// </summary>
    /// <author>OS</author>

    [Serializable]
    public class ASAException : Exception
    {
        //error type of the original exception
        private String original_Error_Type;

        //error message of the original exception
        private String original_Message;

        //string that will be returned as the fault strng in the FaultException
        private string sFaultString;

        //string that will be returned as the Message in the FaultException detail
        private string sDetailMessage;

        //the message for the exception:equal to ASA System Message if ASA Error Code available
        //otherwise message of original exception 
        private String sCodeDescription;

        //business message for the exception:equal to ASA Business Message if ASA Error Code available
        //otherwise null
        private String sBusinessDescription;
        private String sExceptionType;
        private String sShortDescription;
        private String sExceptionError_id;

        //the name of the application or the object that causes the error.
        private String error_Source;

        //the method that throws the current exception
        private String error_TargetSite;

        //string representation of the frames on the call stack 
        private String error_call_stack;

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="ASAException"/> class.
        /// </summary>
        public ASAException() : base() { }


        public ASAException(string sObjectID, string sRelatedObjectType, string sObjectTypeName, string sFunctionName, string sServiceName)
        {
            //sCodeDescription = CreateDetailMessage("", sObjectID, sRelatedObjectType, sObjectTypeName, sFunctionName);
            sDetailMessage = CreateDetailMessage("", sObjectID, sRelatedObjectType, sObjectTypeName, sFunctionName);
            sFaultString = CreateFaultString(sServiceName);
        }

        public ASAException(string message, string code, string sServiceName)
        {
            //sCodeDescription = message;
            sDetailMessage = message;
            sFaultString = CreateFaultString(sServiceName);
            sExceptionError_id = code;
        }

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="ASAException"/> class.
        /// </summary>
        /// <param name="message">
        /// A message about the exception.
        /// </param>
        public ASAException(string message)
        {
            //sCodeDescription = message;
            sDetailMessage = message;
            sFaultString = CreateFaultString("");
        }

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="ASAException"/> class.
        /// </summary>
        /// <param name="message">
        /// A message about the exception.
        /// </param>
        /// <param name="sObjectID">
        /// ID of object being processed
        /// </param>
        public ASAException(string message, string sServiceName)
        {
            //sCodeDescription = message;
            sDetailMessage = message;
            sFaultString = CreateFaultString(sServiceName);
        }

        /// <summary>
        /// Creates a new instance of the
        /// ASAException class.
        /// </summary>
        /// <param name="message">
        /// A message about the exception.
        /// </param>
        /// <param name="sObjectID">
        /// ID of object being processed
        /// </param>
        /// <param name="rootCause">
        /// The root exception (from the underlying data access API, such as ADO.NET).
        /// </param>
        protected ASAException(string message, string sServiceName, Exception rootCause)
            : base(message, rootCause)
        {
            sDetailMessage = message;
            sFaultString = CreateFaultString(sServiceName);
        }

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="ASAException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/>
        /// that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/>
        /// that contains contextual information about the source or destination.
        /// </param>
        protected ASAException(
            SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        #region Public Properties

        public string Original_Error_Type
        {
            set { original_Error_Type = value; }
            get { return original_Error_Type; }
        }

        public string Original_Message
        {
            set { original_Message = value; }
            get { return original_Message; }
        }

        public string ExceptionType
        {
            get { return sExceptionType; }
            set { sExceptionType = value; }
        }

        public string ShortDescription
        {
            get { return sShortDescription; }
            set { sShortDescription = value; }
        }

        public string BusinessDescription
        {
            get { return sBusinessDescription; }
            set { sBusinessDescription = value; }
        }

        public string CodeDescription
        {
            get { return sCodeDescription; }
            set { sCodeDescription = value; }
        }

        public string Error_FaultString
        {
            get { return sFaultString; }
            set { sFaultString = value; }
        }

        public string Error_DetailMessage
        {
            get { return sDetailMessage; }
            set { sDetailMessage = value; }
        }

        public string ExceptionError_id
        {
            get { return sExceptionError_id; }
            set { sExceptionError_id = value; }
        }

        public string Error_Source
        {
            get { return error_Source; }
            set { error_Source = value; }
        }

        public string Error_TargetSite
        {
            get { return error_TargetSite; }
            set { error_TargetSite = value; }
        }

        public string Error_call_stack
        {
            get { return error_call_stack; }
            set { error_call_stack = value; }
        }

        #endregion

        protected static string CreateDetailMessage(string sMsgPrefix, string sObjectID, string sRelatedObjectType, string sObjectName, string sFunctionName)
        {

            StringBuilder sBuilder = new StringBuilder();
            if (sMsgPrefix == string.Empty)
            {
                sMsgPrefix = "Unable to process object";
            }

            sBuilder.Append(sMsgPrefix);

            if (sObjectName.Length > 0)
                sBuilder.AppendFormat(" of type {0}", sObjectName);

            if (sFunctionName.Length > 0)
                sBuilder.AppendFormat(" in function {0}", sFunctionName);

            if (sRelatedObjectType != null && sRelatedObjectType.Length > 0)
                sBuilder.AppendFormat(" related to object of type {0}", sRelatedObjectType);

            if (sObjectID.Length > 0)
                sBuilder.AppendFormat(" with ID: {0}", sObjectID);

            return sBuilder.ToString();

        }

        protected static string CreateFaultString(string sServiceName)
        {

            StringBuilder sBuilder = new StringBuilder();

            string sMsgPrefix = "Service Error - Unable to process object";

            sBuilder.Append(sMsgPrefix);

            if (sServiceName.Length > 0)
                sBuilder.Insert(0, sServiceName + " ");

            return sBuilder.ToString();

        }

    }

    public class ExceptionData
    {
        #region Fields

        protected string sExceptionType;
        protected string sShortDescription;
        protected string sBusinessDescription;
        protected string sCodeDescription;
        protected string sExceptionErrorid;

        #endregion

        #region Properties

        public string ExceptionType
        {
            get { return sExceptionType; }
            set { sExceptionType = value; }
        }

        public string ShortDescription
        {
            get { return sShortDescription; }
            set { sShortDescription = value; }
        }

        public string BusinessDescription
        {
            get { return sBusinessDescription; }
            set { sBusinessDescription = value; }
        }

        public string CodeDescription
        {
            get { return sCodeDescription; }
            set { sCodeDescription = value; }
        }
        public string ExceptionError_id
        {
            get { return sExceptionErrorid; }
            set { sExceptionErrorid = value; }
        }


        #endregion

        #region Constructor (s)

        public ExceptionData()
        {
        }

        public ExceptionData(string ExceptionType, string ShortDescription, 
                        string BusinessDescription, string CodeDescription, 
                        string ExceptionError_id)
        {

            this.ExceptionType = ExceptionType;
            this.ShortDescription = ShortDescription;
            this.BusinessDescription = BusinessDescription;
            this.CodeDescription = CodeDescription;
            this.ExceptionError_id = ExceptionError_id;
        }

        #endregion
    }

}
