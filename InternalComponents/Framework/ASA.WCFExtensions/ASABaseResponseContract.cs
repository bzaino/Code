///////////////////////////////////////////////
//  WorkFile Name: ASABaseResponseContract.cs in ASA.WCFExtensions
//  Description:        
//      Base response message contract class
//            ASA Proprietary Information
///////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ASA.Common;

namespace ASA.WCFExtensions
{
    /// <summary>
    /// Base Response message object, contains result code and message list which all responses
    /// should have.
    /// </summary>
    [MessageContract]
    public class ASABaseResponseContract
    {
        [MessageBodyMember(Order = 0, Name = "ResultCode")]
        private int _ResultCode;
        /// <summary>
        /// Gets or sets the result code
        /// </summary>
        /// <value>ResultCode</value>
        public int ResultCode
        {
            get { return _ResultCode; }
            set { _ResultCode = value; }
        }

       
        [MessageBodyMember(Order=1, Name="ResponseMessageList")]
        private ResponseMessageList _ResponseMessageList;

        /// <summary>
        /// Gets or sets the response message list
        /// </summary>
        /// <value>ResposneMessageList</value>
        public ResponseMessageList ResponseMessageList
        {
          get { return _ResponseMessageList; }
          set { _ResponseMessageList = value; }
        } 


    }
        
}
