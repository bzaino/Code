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
    public partial class RefRegistrationSourceType : DomainObject<int> 
    {
        public RefRegistrationSourceType()
        {
            this.RefRegistrationSources = new HashSet<RefRegistrationSource>();
        }
    
        public int RefRegistrationSourceTypeID { get; set; }
        public string RegistrationSourceTypeName { get; set; }
        public string RegistrationSourceTypeDescription { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        public virtual ICollection<RefRegistrationSource> RefRegistrationSources { get; set; }
    }
    
}
