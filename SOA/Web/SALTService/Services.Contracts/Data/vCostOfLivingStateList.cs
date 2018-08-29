using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    public class vCostOfLivingStateListContract : IEntity 
    {
        [DataMember]
        public int RefStateID { get; set; }
        [DataMember]
        public string StateCode { get; set; }
        [DataMember]
        public string StateName { get; set; }
    }
}
