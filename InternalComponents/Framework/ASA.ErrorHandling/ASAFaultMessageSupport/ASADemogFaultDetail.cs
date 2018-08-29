using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;

namespace ASA.ErrorHandling
{
    /// <summary>
    /// Fault contract used to return custom exception ASADemogBusinessException.  QC 1690-1693
    /// </summary>
    [DataContract(Namespace = "http://amsa.com/contract/errorHandling/v1.0", Name = "ASADemogFaultDetail")]
    public class ASADemogFaultDetail
    {
        private System.String _MessageField;

        public ASADemogFaultDetail()
            : this(string.Empty, Guid.Empty)
        {

        }

        public ASADemogFaultDetail(string sMessage, Guid gID)
        {
            _MessageField = sMessage;
            _IdField = gID;
        }

        [DataMember(IsRequired = true, Name = "Message", Order = 0)]
        public System.String Message
        {
            get { return _MessageField; }
            set { _MessageField = value; }
        }

        private System.Guid _IdField;

        [DataMember(IsRequired = true, Name = "Id", Order = 1)]
        public System.Guid Id
        {
            get { return _IdField; }
            set { _IdField = value; }
        }
    }
}