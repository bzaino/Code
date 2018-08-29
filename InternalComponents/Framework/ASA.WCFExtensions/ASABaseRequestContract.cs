///////////////////////////////////////////////
//  WorkFile Name: ASABaseRequestContract.cs in ASA.WCFExtensions
//  Description:        Request object class for ASA services
//            ASA Proprietary Information
///////////////////////////////////////////////
using System.ServiceModel;
using System.Collections.Generic;

namespace ASA.WCFExtensions
{
    /// <summary>
    /// The ASABaseMessageContract class contains a base message contract that 
    /// should be used by all requests to WCF services developed by ASA.  
    /// It contains only one member, which is the ParamList message header.  
    /// This will be the place for application developers to put any special 
    /// parameters they would like to have audited to identify their logical 
    /// transaction.  This ParamList message header object will get moved inside 
    /// the TID header object during the processing of the BeforeSendRequest 
    /// interception point in the ASAClientMessageInspector class.
    /// </summary>
    [MessageContract]
    public class ASABaseRequestContract
    {
        public const string PARAMLIST_NAMESPACE = "http://amsa.com/contract/baserequestcontract/v1.0";

        private Dictionary<string, string> _ParamList;
        [MessageHeader(
            MustUnderstand=false, 
            Name="ParamList",
            Namespace = ASA.WCFExtensions.ASABaseRequestContract.PARAMLIST_NAMESPACE, 
            Relay=false)]
        public Dictionary<string, string> ParamList
        {
            get { return _ParamList; }
            set { _ParamList = value; }
        }
    }
}


