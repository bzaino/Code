///////////////////////////////////////////////
//  WorkFile Name: TID.cs in ASA.TID
//  Description:        
//      TID implementation class for version 01.00.000
//            ASA Proprietary Information
///////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ASA.TID
{
    [DataContract(Namespace = "http://amsa.com/contract/tid/v1.0", Name = "TID")]
    public class TID : TIDBase
    {
        #region Properties
        // Properties
        const string tidVersion = "01.00.000";
        string messageVersion;
        string serviceName;
        string operationName;
        string userName;
        string messageId;
        string correlationId;
        DateTime originationDT;
        DateTime sentDT;
        DateTime? receivedDT;
        DateTime expireDT;
        int messageSeqNumber;
        string source;
        string destination;
        bool synch;
        bool reply;
        Dictionary<string, string> paramList;
        #endregion

        #region Property Methods
        [DataMember]
        public string TidVersion
        {
            get { return tidVersion; }
            set { }
        }
        [DataMember]
        public string MessageVersion
        {
            get { return messageVersion; }
            set { messageVersion = value; }
        }
        [DataMember]
        public string ServiceName
        {
            get { return serviceName; }
            set { serviceName = value; }
        }
        [DataMember]
        public string OperationName
        {
            get { return operationName; }
            set { operationName = value; }
        }
        [DataMember]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        [DataMember]
        public string MessageId
        {
            get { return messageId; }
            set { messageId = value; }
        }
        [DataMember]
        public string CorrelationId
        {
            get { return correlationId; }
            set { correlationId = value; }
        }
        [DataMember]
        public DateTime OriginationDT
        {
            get { return originationDT; }
            set { originationDT = value; }
        }
        [DataMember]
        public DateTime SentDT
        {
            get { return sentDT; }
            set { sentDT = value; }
        }
        [DataMember]
        public DateTime? ReceivedDT
        {
            get { return receivedDT; }
            set { receivedDT = value; }
        }
        [DataMember]
        public DateTime ExpireDT
        {
            get { return expireDT; }
            set { expireDT = value; }
        }
        [DataMember]
        public int MessageSeqNumber
        {
            get { return messageSeqNumber; }
            set { messageSeqNumber = value; }
        }
        [DataMember]
        public string Source
        {
            get { return source; }
            set { source = value; }
        }
        [DataMember]
        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }
        [DataMember]
        public bool Synch
        {
            get { return synch; }
            set { synch = value; }
        }
        [DataMember]
        public bool Reply
        {
            get { return reply; }
            set { reply = value; }
        }
        [DataMember]
        Dictionary<string, string> ParamList
        {
            get { return paramList; }
            set { paramList = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// The TID() default constructor is used to initailize the fields in a new TID object.
        /// The TID() default constructor will set the appropriate property and Hashtable entries.
        /// </summary>
        /// <param name="tidIn"></param>
        /// <returns></returns>
        public TID()
            : base()
        {
            mapProps[TIDField.CORRELATIONID] = correlationId = "";
            mapProps[TIDField.DESTINATION] = destination = "";
            mapProps[TIDField.EXPIREDT] = expireDT = DateTime.MaxValue;
            mapProps[TIDField.MESSAGEID] = messageId = "";
            mapProps[TIDField.MESSAGEVERSION] = messageVersion = "";
            mapProps[TIDField.OPERATIONNAME] = operationName = "";
            mapProps[TIDField.ORIGINATIONDT] = originationDT = DateTime.MinValue;
            mapProps[TIDField.PARAMLIST] = paramList = new Dictionary<string, string>();
            mapProps[TIDField.SERVICENAME] = serviceName = "";
            mapProps[TIDField.SOURCE] = source = "";
            mapProps[TIDField.USERNAME] = userName = "";
            mapProps[TIDField.MESSAGESEQNUMBER] = messageSeqNumber = 0;
            mapProps[TIDField.RECEIVEDDT] = receivedDT = null;
            mapProps[TIDField.REPLY] = reply = false;
            mapProps[TIDField.SENTDT] = sentDT = DateTime.MinValue;
            mapProps[TIDField.SYNCH] = synch = true;
            mapProps[TIDField.TIDVERSION] = tidVersion;
        }

        /// <summary>
        /// The TID() copy constructor is used to initialize a new TID object based on fields set in an existing TID object upon object creation.
        /// The TID() copy constructor will set the appropriate property and Hashtable entries.
        /// </summary>
        /// <param name="tidIn"></param>
        /// <returns></returns>
        public TID(TID tidIn)
            : base(tidIn)
        {
            mapProps[TIDField.CORRELATIONID] = correlationId = tidIn.correlationId;
            mapProps[TIDField.DESTINATION] = destination = tidIn.destination;
            mapProps[TIDField.EXPIREDT] = expireDT = tidIn.expireDT;
            mapProps[TIDField.MESSAGEID] = messageId = tidIn.messageId;
            mapProps[TIDField.MESSAGEVERSION] = messageVersion = tidIn.messageVersion;
            mapProps[TIDField.OPERATIONNAME] = operationName = tidIn.operationName;
            mapProps[TIDField.ORIGINATIONDT] = originationDT = tidIn.originationDT;
            mapProps[TIDField.PARAMLIST] = paramList = (tidIn.paramList != null) ? new Dictionary<string, string>(tidIn.paramList) : new Dictionary<string, string>();
            mapProps[TIDField.SERVICENAME] = serviceName = tidIn.serviceName;
            mapProps[TIDField.SOURCE] = source = tidIn.source;
            mapProps[TIDField.USERNAME] = userName = tidIn.userName;
            mapProps[TIDField.MESSAGESEQNUMBER] = messageSeqNumber = tidIn.messageSeqNumber;
            mapProps[TIDField.RECEIVEDDT] = receivedDT = tidIn.receivedDT;
            mapProps[TIDField.REPLY] = reply = tidIn.reply;
            mapProps[TIDField.SENTDT] = sentDT = tidIn.sentDT;
            mapProps[TIDField.SYNCH] = synch = tidIn.synch;
            mapProps[TIDField.TIDVERSION] = tidVersion;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Call SetExpireTimeoutMS() to set the ExpireDT field within the TID object based on a known timeout in milliseconds.
        /// SetExpireTimeoutMS() will update the ExpireDT property and Hashtable entry.
        /// SetExpireTimeoutMS() returns false if it fails to update the ExpireDT field.
        /// </summary>
        /// <param name="expireMSIn"></param>
        /// <returns>bool</returns>
        public virtual bool SetExpireTimeoutMS(int expireMSIn)
        {
            bool bRet = false;
            if (OriginationDT > DateTime.MinValue && OriginationDT < DateTime.MaxValue)
            {
                DateTime newExpireDT = OriginationDT;
                newExpireDT.AddMilliseconds(expireMSIn);
                bRet = Set(TIDField.EXPIREDT, newExpireDT);
            }
            return bRet;
        }

        /// <summary>
        /// Call CreateTID() to update the fields within the TID object when a new message is created.
        /// CreateTID() will update the appropriate property and Hashtable entries.
        /// Update/override any TID fields by specifying those field/values within the passed Hashtable argument.
        /// The abstract CreateTID() method must be implemented in any TIDBase-derived class.
        /// CreateTID() returns false if a field name being set is not allowed.
        /// </summary>
        /// <param name="mapPropsIn"></param>
        /// <returns>bool</returns>
        public override bool CreateTID(Hashtable mapPropsIn)
        {
            bool bRet = true;

            bRet = (Set(TIDField.CORRELATIONID, Guid.NewGuid().ToString()) && bRet);
            bRet = (Set(TIDField.DESTINATION, "") && bRet);
            bRet = (Set(TIDField.EXPIREDT, DateTime.MaxValue.ToUniversalTime()) && bRet);
            bRet = (Set(TIDField.MESSAGEID, "") && bRet);
            bRet = (Set(TIDField.MESSAGESEQNUMBER, 0) && bRet);
            bRet = (Set(TIDField.MESSAGEVERSION, "") && bRet);
            bRet = (Set(TIDField.OPERATIONNAME, "") && bRet);
            bRet = (Set(TIDField.ORIGINATIONDT, DateTime.UtcNow) && bRet);
            bRet = (Set(TIDField.RECEIVEDDT, null) && bRet);
            bRet = (Set(TIDField.REPLY, false) && bRet);
            bRet = (Set(TIDField.SENTDT, DateTime.MinValue.ToUniversalTime()) && bRet);
            bRet = (Set(TIDField.SERVICENAME, "") && bRet);
            bRet = (Set(TIDField.SOURCE, "") && bRet);
            bRet = (Set(TIDField.SYNCH, true) && bRet);
            bRet = (mapPropsIn.ContainsKey(TIDField.USERNAME) && bRet);

            // Override any above set fields passed in mapPropsIn
            bRet = (SetFields(mapPropsIn) && bRet);

            return bRet;
        }

        /// <summary>
        /// Call SendTID() to update the fields within the TID object when a message is being sent.
        /// SendTID() will update the appropriate property and Hashtable entries.
        /// The ITID argument passed must be the interface to the Request received TID (not Reply received TID).
        /// Update/override any TID fields by specifying those field/values within the passed Hashtable argument.
        /// The abstract RecvTID() method must be implemented in any TIDBase-derived class.
        /// </summary>
        /// <param name="tidIn"></param>
        /// <param name="mapPropsIn"></param>
        /// <returns>bool</returns>
        public override bool SendTID(ITID tidIn, Hashtable mapPropsIn)
        {
            bool bRet = true;

            // Ensure reply field is set
            bRet = (mapPropsIn.ContainsKey(TIDField.REPLY) && bRet);

            // Get reply flag -- if not specified (although it is required), assume this is a request message
            bool isReply = (bRet) ? (bool)mapPropsIn[TIDField.REPLY] : false;

            // Get new ParamList if specified by caller in mapPropsIn
            Dictionary<string, string> paramListIn = (mapPropsIn.ContainsKey(TIDField.PARAMLIST))
                ? (Dictionary<string, string>)mapPropsIn[TIDField.PARAMLIST]
                : null;

            // Processing for reply messages
            if (isReply)
            {
                if (paramListIn == null)
                {
                    // If message is a reply and caller did not specify a ParamList in mapPropsIn, use ParamList from received tid (tidIn) instead
                    paramListIn = (Dictionary<string, string>)tidIn.Get(TIDField.PARAMLIST);
                }

                // Reply messages must specify the MESSAGESEQNUMBER when sending messages 
                //  since we only have the MESSAGESEQNUMBER from the request received TID (not the reply received TID)
                bRet = (mapPropsIn.ContainsKey(TIDField.MESSAGESEQNUMBER) && bRet);
            }
            // Processing for request messsages
            else
            {
                // Request messages will set the next 
                bRet = (Set(TIDField.MESSAGESEQNUMBER, (int)(tidIn.Get(TIDField.MESSAGESEQNUMBER)) + 1) && bRet);
            }

            bRet = (Set(TIDField.CORRELATIONID, tidIn.Get(TIDField.CORRELATIONID)) && bRet);
            bRet = (mapPropsIn.ContainsKey(TIDField.DESTINATION) && bRet);
            bRet = (Set(TIDField.EXPIREDT, tidIn.Get(TIDField.EXPIREDT)) && bRet);
            bRet = (Set(TIDField.MESSAGEID, Guid.NewGuid().ToString()) && bRet);
            bRet = (mapPropsIn.ContainsKey(TIDField.MESSAGEVERSION) && bRet);
            bRet = (mapPropsIn.ContainsKey(TIDField.OPERATIONNAME) && bRet);
            bRet = (Set(TIDField.ORIGINATIONDT, tidIn.Get(TIDField.ORIGINATIONDT)) && bRet);
            bRet = (Set(TIDField.PARAMLIST, paramListIn) && bRet);
            bRet = (Set(TIDField.RECEIVEDDT, null) && bRet);
            bRet = (mapPropsIn.ContainsKey(TIDField.SOURCE) && bRet);
            bRet = (mapPropsIn.ContainsKey(TIDField.SERVICENAME) && bRet);
            bRet = (Set(TIDField.SYNCH, tidIn.Get(TIDField.SYNCH)) && bRet);
            bRet = (Set(TIDField.USERNAME, tidIn.Get(TIDField.USERNAME)) && bRet);
            bRet = (Set(TIDField.SENTDT, DateTime.UtcNow) && bRet);

            // Override any above set fields passed in mapPropsIn
            bRet = (SetFields(mapPropsIn) && bRet);

            return bRet;
        }

        /// <summary>
        /// Call RecvTID() to update the fields within the TID object when a message is received.
        /// RecvTID() will update the appropriate property and Hashtable entries.
        /// Update/override any TID fields by specifying those field/values within the passed Hashtable argument.
        /// The abstract RecvTID() method must be implemented in any TIDBase-derived class.
        /// RecvTID() returns false if a field name being set is not allowed.
        /// </summary>
        /// <param name="mapPropsIn"></param>
        /// <returns>bool</returns>
        public override bool RecvTID(Hashtable mapPropsIn)
        {
            bool bRet = true;
            do
            {
                bRet = (Set(TIDField.RECEIVEDDT, DateTime.UtcNow) && bRet);
                bRet = (Set(TIDField.MESSAGESEQNUMBER, (int)Get(TIDField.MESSAGESEQNUMBER)) && bRet);

                // Override any above set fields passed in mapPropsIn
                bRet = (SetFields(mapPropsIn) && bRet);

            } while (false);

            return bRet;
        }

        /// <summary>
        /// Always use Set() to set a field within the TID object.
        /// Set() will update the appropriate property and Hashtable entry.
        /// The abstract Set() method must be implemented in any TIDBase-derived class.
        /// Set() returns false if the field name passed in is not allowed.
        /// </summary>
        /// <param name="fieldNameIn"></param>
        /// <param name="valueIn"></param>
        /// <returns>bool</returns>
        public override bool Set(string fieldNameIn, object valueIn)
        {
            bool isSet = true;
            string fieldName = fieldNameIn.ToUpper();

            if (fieldName == TIDField.CORRELATIONID)
            {
                mapProps[fieldName] = CorrelationId = (string)valueIn;
            }
            else if (fieldName == TIDField.DESTINATION)
            {
                mapProps[fieldName] = Destination = (string)valueIn;
            }
            else if (fieldName == TIDField.EXPIREDT)
            {
                mapProps[fieldName] = ExpireDT = (DateTime)valueIn;
            }
            else if (fieldName == TIDField.EXPIRETIMEOUTMS)
            {
                isSet = SetExpireTimeoutMS((int)valueIn);
            }
            else if (fieldName == TIDField.MESSAGEID)
            {
                mapProps[fieldName] = MessageId = (string)valueIn;
            }
            else if (fieldName == TIDField.MESSAGESEQNUMBER)
            {
                mapProps[fieldName] = MessageSeqNumber = (int)valueIn;
            }
            else if (fieldName == TIDField.MESSAGEVERSION)
            {
                mapProps[fieldName] = MessageVersion = (string)valueIn;
            }
            else if (fieldName == TIDField.OPERATIONNAME)
            {
                mapProps[fieldName] = OperationName = (string)valueIn;
            }
            else if (fieldName == TIDField.ORIGINATIONDT)
            {
                mapProps[fieldName] = OriginationDT = (DateTime)valueIn;
            }
            else if (fieldName == TIDField.PARAMLIST)
            {
                mapProps[fieldName] = ParamList = (valueIn != null) ? new Dictionary<string, string>((Dictionary<string, string>)valueIn) : null;
            }
            else if (fieldName == TIDField.RECEIVEDDT)
            {
                mapProps[fieldName] = ReceivedDT = (DateTime?)valueIn;
            }
            else if (fieldName == TIDField.REPLY)
            {
                mapProps[fieldName] = Reply = (bool)valueIn;
            }
            else if (fieldName == TIDField.SENTDT)
            {
                mapProps[fieldName] = SentDT = (DateTime)valueIn;
            }
            else if (fieldName == TIDField.SERVICENAME)
            {
                mapProps[fieldName] = ServiceName = (string)valueIn;
            }
            else if (fieldName == TIDField.SOURCE)
            {
                mapProps[fieldName] = Source = (string)valueIn;
            }
            else if (fieldName == TIDField.SUCCESS)
            {
                // Ignore SUCCESS fields
            }
            else if (fieldName == TIDField.SYNCH)
            {
                mapProps[fieldName] = Synch = (bool)valueIn;
            }
            else if (fieldName == TIDField.USERNAME)
            {
                mapProps[fieldName] = UserName = (string)valueIn;
            }
            else
            {
                isSet = false;
            }

            return isSet;
        }

        /// <summary>
        /// Call GetVersion() to retrieve the TID Version.
        /// The abstract GetVersion() method must be implemented in any TIDBase-derived class.
        /// </summary>
        /// <returns>string</returns>
        public override string GetVersion()
        {
            return TidVersion;
        }
        #endregion
    }
}
