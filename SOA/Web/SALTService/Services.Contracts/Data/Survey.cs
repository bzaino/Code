//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Asa.Salt.Web.Services.Contracts.Data
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(SurveyOptionContract))]
    public class SurveyContract : IEntity 
    {
        public SurveyContract()
        {
            this.SurveyOptions = new HashSet<SurveyOptionContract>();
        }
    
        [DataMember]
        public int SurveyId { get; set; }
        [DataMember]
        public string SurveyName { get; set; }
        [DataMember]
        public System.DateTime SurveyStartDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> SurveyEndDate { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public System.DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DataMember]
        public string SurveyQuestion { get; set; }
        [DataMember]
        public string SurveyDescription { get; set; }
        [DataMember]
        public string SurveyPurpose { get; set; }
        [DataMember]
        public string ListOfValues { get; set; }
    
        [DataMember]
        public virtual ICollection<SurveyOptionContract> SurveyOptions { get; set; }
    }
    
}
