///////////////////////////////////////////////
//  WorkFile Name: Parameters.cs in ASA.Common
//  Description:    
//      This class exposed static methods for accessing parameters in the ASA config file    
//            ASA Proprietary Information
///////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;

namespace ASA.Common
{
    public sealed class Parameters
    {
        #region Thread-safe, lazy Singleton
        /// <summary>
        /// This is a thread-safe, lazy singleton.  See http://www.yoda.arachsys.com/csharp/singleton.html
        /// for more details about its implementation.
        /// </summary>
        public static Parameters Instance
        {
            get
            {
                return Nested.ParameterInstance;
            }
        }

        /// <summary>
        /// Assists with ensuring thread-safe, lazy singleton
        /// </summary>
        private class Nested
        {
            static Nested() { }
            internal static readonly Parameters ParameterInstance = new Parameters();
        }
        #endregion

        //The name of the section in the config file
        private string _SectionName = "asa";
        //Config section        
        private NameValueCollection section;

        /// <summary>
        /// Private constructor to enforce singleton
        /// </summary>
        private Parameters()
        {
            //Initialize
            section = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection(_SectionName);
        }

        /// <summary>
        /// Gets the section name
        /// </summary>
        /// <value>SectionName</value>
        public string SectionName
        {
            set
            {
                _SectionName = value;
                section = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection(_SectionName);
            }
            get
            {
                return (_SectionName);
            }
        }

        /// <summary>
        /// Gets an integer value from the config file based on the name
        /// </summary>
        /// <param name="ParamName">Name of parameter in the config file</param>
        /// <param name="DefaultValue">Default value to return if parameter not found in config file</param>
        /// <returns>Parameter value or default value if not found</returns>
        public int GetIntValue(string ParamName, int DefaultValue)
        {
            int RetValue = DefaultValue;
            if (section != null)
            {
                object temp = section[ParamName];
                if (temp != null)
                {
                    RetValue = int.Parse(temp.ToString());
                }
            }
            return (RetValue);
        }

        /// <summary>
        /// Gets a string value from the config file based on the name
        /// </summary>
        /// <param name="ParamName">Name of parameter in the config file</param>
        /// <param name="DefaultValue">Default value to return if parameter not found in config file</param>
        /// <returns>Parameter value or default value if not found</returns>
        public string GetStrValue(string ParamName, string DefaultValue)
        {
            string RetValue = DefaultValue;

            if (section != null)
            {
                object temp = section[ParamName];
                if (temp != null)
                {
                    RetValue = temp.ToString();
                }
            }
            return (RetValue);
        }

        /// <summary>
        /// Gets a boolean value from the config file based on the name
        /// </summary>
        /// <param name="ParamName">Name of parameter in the config file</param>
        /// <param name="DefaultValue">Default value to return if parameter not found in config file</param>
        /// <returns>Parameter value or default value if not found</returns>
        public bool GetBoolValue(string ParamName, bool DefaultValue)
        {
            bool RetValue = DefaultValue;

            if (section != null)
            {
                object temp = section[ParamName];
                if (temp != null)
                {
                    RetValue = bool.Parse(temp.ToString());
                }
            }
            return (RetValue);
        }

        private string _QueryTimeoutParamName = "query_timeout";
        /// <summary>
        /// Gets the name of the QueryTimeout parameter
        /// </summary>
        /// <value>QueryTimeoutParamName</value>
        public string QueryTimeoutParamName
        {
            get
            {
                return (_QueryTimeoutParamName);
            }
        }
        /// <summary>
        /// The number of seconds for a query to wait before timing out
        /// </summary>
        /// <value>The QueryTimeout</value>
        public int QueryTimeout
        {
            get
            {
                return (GetIntValue(_QueryTimeoutParamName, 300));
            }
        }

        private const string _LogMessagesParamName = "log_messages";
        /// <summary>
        /// Gets the name of the LogMessages Parameter
        /// </summary>
        /// <value>LogMessagesParamName</value>
        public string LogMessagesParamName
        {
            get
            {
                return (_LogMessagesParamName);
            }
        }
        /// <summary>
        /// Should all messages be logged
        /// </summary>
        /// <value>LogMessages</value>
        public bool LogMessages
        {
            get
            {
                return (GetBoolValue(_LogMessagesParamName, false));
            }
        }

        private const string _LogMessagePathParamName = "log_message_path";
        /// <summary>
        /// Gets the LogMessagesPath parameter name
        /// </summary>
        /// <value>LogMessagePathParamName</value>
        public string LogMessagePathParamName
        {
            get
            {
                return (_LogMessagePathParamName);
            }
        }
        /// <summary>
        /// Path to log messages to
        /// </summary>
        /// <value>LogMessagePath</value>
        public string LogMessagePath
        {
            get
            {
                return (GetStrValue(_LogMessagePathParamName, ""));
            }
        }

        private const string _BowsConnectionStringParamName = "BOWS_ConnectionString";
        /// <summary>
        /// Gets the name of the BowsConnectionString parameter
        /// </summary>
        /// <value>BowsConnectionStringParamName</value>
        public string BowsConnectionStringParamName
        {
            get
            {
                return (_BowsConnectionStringParamName);
            }
        }
        /// <summary>
        /// Gets the value of the BowsConnectionString 
        /// </summary>
        /// <value>BowsConnectionString</value>
        public string BowsConnectionString
        {
            get
            {
                return (GetStrValue(_BowsConnectionStringParamName, ""));
            }
        }

        private const string _EnterpriseConnectionStringParamName = "Enterprise_ConnectionString";
        /// <summary>
        /// Gets the name of the EnterpriseConnectionString parameter
        /// </summary>
        /// <value>EnterpriseConnectionStringParamName</value>
        public string EnterpriseConnectionStringParamName
        {
            get
            {
                return (_EnterpriseConnectionStringParamName);
            }
        }
        /// <summary>
        /// Gets the value of the EnterpriseConnectionString 
        /// </summary>
        /// <value>EnterpriseConnectionString</value>
        public string EnterpriseConnectionString
        {
            get
            {
                return (GetStrValue(_EnterpriseConnectionStringParamName, ""));
            }
        }

        #region Client audit
        private const string _ClientRequestAuditParamName = "Enable_ClientRequestAudit";
        /// <summary>
        /// Gets the name of the Client Request Audit parameter
        /// </summary>
        /// <value>ClientRequestAuditParamName</value>
        public string ClientRequestAuditParamName
        {
            get
            {
                return (_ClientRequestAuditParamName);
            }
        }

        /// <summary>
        /// Gets a flag indicating if the auditing of request messages sent
        /// by the client is enabled
        /// </summary>
        /// <value>EnableClientRequestAudit</value>
        public bool EnableClientRequestAudit
        {
            get
            {

                return (GetBoolValue(_ClientRequestAuditParamName, true));
            }
        }

        private const string _ClientResponseAuditParamName = "Enable_ClientResponseAudit";
        /// <summary>
        /// Gets the name of the Client Response Audit parameter
        /// </summary>
        /// <value>ClientResponseAuditParamName</value>
        public string ClientResponseAuditParamName
        {
            get
            {
                return (_ClientResponseAuditParamName);
            }
        }

        /// <summary>
        /// Gets a flag indicating if the auditing of response messages returned
        /// to the client is enabled
        /// </summary>
        /// <value>EnableClientResponseAudit</value>
        public bool EnableClientResponseAudit
        {
            get
            {

                return (GetBoolValue(_ClientResponseAuditParamName, true));
            }
        }
        #endregion

        #region Service audit
        private const string _ServiceRequestAuditParamName = "Enable_ServiceRequestAudit";
        /// <summary>
        /// Gets the name of the Service Request Audit parameter
        /// </summary>
        /// <value>ServiceRequestAuditParamName</value>
        public string ServiceRequestAuditParamName
        {
            get
            {
                return (_ServiceRequestAuditParamName);
            }
        }

        /// <summary>
        /// Gets a flag indicating if the auditing of request messages received
        /// by the service is enabled
        /// </summary>
        /// <value>EnableServiceRequestAudit</value>
        public bool EnableServiceRequestAudit
        {
            get
            {

                return (GetBoolValue(_ServiceRequestAuditParamName, true));
            }
        }

        private const string _ServiceResponseAuditParamName = "Enable_ServiceResponseAudit";
        /// <summary>
        /// Gets the name of the Service Response Audit parameter
        /// </summary>
        /// <value>ServiceResponseAuditParamName</value>
        public string ServiceResponseAuditParamName
        {
            get
            {
                return (_ServiceResponseAuditParamName);
            }
        }

        /// <summary>
        /// Gets a flag indicating if the auditing of response messages returned
        /// by the service is enabled
        /// </summary>
        /// <value>EnableServiceResponseAudit</value>
        public bool EnableServiceResponseAudit
        {
            get
            {

                return (GetBoolValue(_ServiceResponseAuditParamName, true));
            }
        }
        #endregion

        #region Error/Fault handling
        private const string _ASAFaultHandlingParamName = "Enable_ASAFaultHandling";
        /// <summary>
        /// Gets the name of the ASAFaultHandling parameter
        /// </summary>
        /// <value>ASAFaultHandlingParamName</value>
        public string ASAFaultHandlingParamName
        {
            get
            {
                return (_ASAFaultHandlingParamName);
            }
        }
        /// <summary>
        /// Gets a flag indicating if the ASAFaultHandling is enabled
        /// </summary>
        /// <value>EnableASAFaultHandling</value>
        public bool EnableASAFaultHandling
        {
            get
            {

                return (GetBoolValue(_ASAFaultHandlingParamName, true));
            }
        }
        #endregion

        
        #region Schema Validation
        private const string _ASAServiceRequestSchemaValidationParamName = "Enable_ASAServiceRequestSchemaValidation";
        private const string _ASAServiceReplySchemaValidationParamName = "Enable_ASAServiceReplySchemaValidation";
        private const string _TargetMessageContractSchemasPathParamName = "TargetMessageContractSchemasPath";

        /// <summary>
        /// Gets a flag indicating if the ASAServiceRequestSchemaValidation is enabled
        /// </summary>
        /// <value>EnableASAServiceRequestSchemaValidation</value>
        /// 



        public bool EnableASAServiceRequestSchemaValidation
        {
            get
            {

                return (GetBoolValue(_ASAServiceRequestSchemaValidationParamName, false));
            }
        }

        /// <summary>
        /// Gets a flag indicating if the EnableASAServiceReplySchemaValidation is enabled
        /// </summary>
        /// <value>EnableASAServiceReplySchemaValidation</value>
        /// 
        public bool EnableASAServiceReplySchemaValidation
        {
            get
            {

                return (GetBoolValue(_ASAServiceReplySchemaValidationParamName,false));
            }
        }

        /// <summary>
        /// Gets a the value for TargetMessageContractSchemasPath
        /// </summary>
        /// <value>TargetMessageContractSchemasPath</value>
        /// 
        public string TargetMessageContractSchemasPath
        {
            get
            {

                return (GetStrValue(_TargetMessageContractSchemasPathParamName,""));
            }
        }
        


        #endregion
    }
}
