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
    public partial class MemberOrganization : DomainObject<int> 
    {
        public int MemberOrganizationID { get; set; }
        public int MemberID { get; set; }
        public int RefOrganizationID { get; set; }
        public Nullable<int> ExpectedGraduationYear { get; set; }
        public System.DateTime EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string SchoolReportingID { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual RefOrganization RefOrganization { get; set; }
    }
    
}
