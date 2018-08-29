///////////////////////////////////////////////
//  WorkFile Name: ASABaseDataContract.cs in ASA.WCFExtensions
//  Description:        
//      Base datacontract class
//            ASA Proprietary Information
///////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ASA.WCFExtensions
{
    [DataContract(Namespace = "http://amsa.com/contract/basedatacontract/v1.0", Name = "ASABaseDataContract")]
    [CLSCompliantAttribute(true)]
    public class ASABaseDataContract : IExtensibleDataObject
    {
        #region IExtensibleDataObject Members
        private ExtensionDataObject _ExtensionData;

        /// <summary>
        /// Gets or sets the Extension Data Object
        /// </summary>
        /// <value>ExtensionData</value>
        public ExtensionDataObject ExtensionData
        {
            get
            {
                return (_ExtensionData);
            }
            set
            {
                _ExtensionData = value;
            }
        }

        #endregion
    }
}

