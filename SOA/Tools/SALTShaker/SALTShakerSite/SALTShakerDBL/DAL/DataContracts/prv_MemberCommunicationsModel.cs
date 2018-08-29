using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SALTShaker.Proxies.PROSPECTService;

namespace SALTShaker.DAL.DataContracts
{
    public class MemberCommunications 
    {
        public int ProspectMemberID { get; set; }
        public Nullable<int> SALTMemberID { get; set; }
        public bool IsEmailContactAllowed { get; set; }
        public bool IsPhoneContactAllowed { get; set; }
        public bool IsMailContactAllowed { get; set; }
        public System.DateTime CommunicationDate { get; set; }
        public string CommunicationChannel { get; set; }
        public string CommunicationDirection { get; set; }
        public string CommunicationSource { get; set; }
        public string CommunicationType { get; set; }
        public string CallResult { get; set; }
        public string CommunicationText { get; set; }
        public string CreatedBy { get; set; }
    }
}
