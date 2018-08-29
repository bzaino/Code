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
using System.ComponentModel.DataAnnotations;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class RefProfileQuestion : DomainObject<int> 
    {
        public RefProfileQuestion()
        {
            this.RefProfileAnswers = new HashSet<RefProfileAnswer>();
        }
    
        public int RefProfileQuestionID { get; set; }
        public string ProfileQuestionName { get; set; }
        public string ProfileQuestionDescription { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public byte ProfileQuestionExternalID { get; set; }
        public Nullable<int> RefProfileQuestionTypeID { get; set; }
        public bool IsProfileQuestionActive { get; set; }
        public Nullable<short> ProfileQuestionPriority { get; set; }
        public bool IsInLineProfileQuestion { get; set; }
    
        public virtual ICollection<RefProfileAnswer> RefProfileAnswers { get; set; }
        public virtual RefProfileQuestionType RefProfileQuestionType { get; set; }
    }
    
}
