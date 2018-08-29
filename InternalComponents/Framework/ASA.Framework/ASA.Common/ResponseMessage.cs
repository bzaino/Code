using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ASA.Common
{
    [DataContract(Namespace = "http://amsa.com/contract/ASA.Common.ResponseMessage/v1.0", Name = "ResponseMessage")]    
    public class ResponseMessage
    {
        public ResponseMessage(string responseCode, string targetEntityName, string targetEntityPrimaryKey, string messageDetails)
        {
            this.ResponseCode = responseCode;
            this.TargetEntityName = targetEntityName;
            this.TargetEntityPrimaryKey = targetEntityPrimaryKey;
            this.MessageDetails = messageDetails;
        }

        public ResponseMessage(string responseCode, string messageDetails)
        {
            this.ResponseCode = responseCode;
            this.TargetEntityName = String.Empty;
            this.TargetEntityPrimaryKey = String.Empty;
            this.MessageDetails = messageDetails;
        }

        public ResponseMessage(string messageDetails)
        {
            this.ResponseCode = String.Empty;
            this.TargetEntityName = String.Empty;
            this.TargetEntityPrimaryKey = String.Empty;
            this.MessageDetails = messageDetails;
        }

        string responseCode;

        [DataMember]
        public string ResponseCode
        {
            get { return responseCode; }
            set { responseCode = value; }
        }
        string targetEntityName;

        [DataMember]
        public string TargetEntityName
        {
            get { return targetEntityName; }
            set { targetEntityName = value; }
        }
        string targetEntityPrimaryKey;

        [DataMember]
        public string TargetEntityPrimaryKey
        {
            get { return targetEntityPrimaryKey; }
            set { targetEntityPrimaryKey = value; }
        }
        string messageDetails;

        [DataMember]
        public string MessageDetails
        {
            get { return messageDetails; }
            set { messageDetails = value; }
        }
        
        public void CopyFrom(ResponseMessage sourceResponseMessage)
        {
            this.MessageDetails=sourceResponseMessage.MessageDetails;
            this.ResponseCode=sourceResponseMessage.ResponseCode;
            this.TargetEntityName=sourceResponseMessage.TargetEntityName;
            this.targetEntityPrimaryKey = sourceResponseMessage.targetEntityPrimaryKey;
        
        }
        
    }

    [CollectionDataContract(Namespace = "http://amsa.com/contract/ASA.Common.ResponseMessage/v1.0", Name = "ResponseMessageList")]
    public sealed class ResponseMessageList : List<ResponseMessage>
    {
        string searchTargetResponseCode=String.Empty;
        string searchTargetMessageDetails = String.Empty;
      
        public bool ContainsResponseMessageBasedOnResponseCode( string searchTargetResponseCode)
        {
            bool bFound = false;

            this.searchTargetResponseCode = searchTargetResponseCode;
            if (this.Find(ResponseCodeSearchPredicate) != null)
                bFound = true;
            return bFound;
            
        }

        public bool ContainsResponseMessageBasedOnMessageDetails(string searchTargetMessageDetails)
        {
            bool bFound = false;

            this.searchTargetMessageDetails = searchTargetMessageDetails;
            if (this.Find(MessageDetailsSearchPredicate) != null)
                bFound = true;
            return bFound;

        }

        public void Add(string responseCode,string messageDetails)
        {
            ResponseMessage responseMessage = new ResponseMessage(responseCode,messageDetails);
            this.Add(responseMessage);
        }

        public void Add(string messageDetails)
        {
            ResponseMessage responseMessage = new ResponseMessage(messageDetails);
            this.Add(responseMessage);
        }
                
        private bool ResponseCodeSearchPredicate (ResponseMessage responseMessage)
        {
            return (this.searchTargetResponseCode == responseMessage.ResponseCode);
        }

        private bool MessageDetailsSearchPredicate(ResponseMessage responseMessage)
        {
            return (this.searchTargetMessageDetails == responseMessage.MessageDetails);
        }
    }

}
