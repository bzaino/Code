///////////////////////////////////////////////
//  WorkFile Name: TIDDefs.cs in ASA.TID
//  Description:        
//      static classes containing things like the 
//      names of TID fields.
//            ASA Proprietary Information
///////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;

namespace ASA.TID
{
    public static class TIDField
    {
        // Valid hash keys
        public const string CORRELATIONID = "CORRELATIONID";
        public const string DESTINATION = "DESTINATION";
        public const string EXPIREDT = "EXPIREDT";
        public const string MESSAGEID = "MESSAGEID";
        public const string MESSAGESEQNUMBER = "MESSAGESEQNUMBER";
        public const string MESSAGEVERSION = "MESSAGEVERSION";
        public const string OPERATIONNAME = "OPERATIONNAME";
        public const string ORIGINATIONDT = "ORIGINATIONDT";
        public const string PARAMLIST = "PARAMLIST";
        public const string RECEIVEDDT = "RECEIVEDDT";
        public const string REPLY = "REPLY";
        public const string SENTDT = "SENTDT";
        public const string SERVICENAME = "SERVICENAME";
        public const string SOURCE = "SOURCE";
        public const string SUCCESS = "SUCCESS";
        public const string SYNCH = "SYNCH";
        public const string TIDVERSION = "TIDVERSION";
        public const string USERNAME = "USERNAME";
        public const string EXPIRETIMEOUTMS = "ExpireTimeoutMS";
    }
}
