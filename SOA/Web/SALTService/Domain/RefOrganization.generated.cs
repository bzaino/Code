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
    public partial class RefOrganization : DomainObject<int> 
    {
        public RefOrganization()
        {
            this.MemberOrganizations = new HashSet<MemberOrganization>();
            this.OrganizationToDoLists = new HashSet<OrganizationToDoList>();
            this.RefOrganizationProducts = new HashSet<RefOrganizationProduct>();
        }
    
        public int RefOrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationDescription { get; set; }
        public bool IsContracted { get; set; }
        public string OrganizationExternalID { get; set; }
        public string OPECode { get; set; }
        public string BranchCode { get; set; }
        public string OrganizationLogoName { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string OrganizationAliases { get; set; }
        public Nullable<int> RefSALTSchoolTypeID { get; set; }
        public bool IsLookupAllowed { get; set; }
        public Nullable<int> RefStateID { get; set; }
        public int RefOrganizationTypeID { get; set; }
    
        public virtual ICollection<MemberOrganization> MemberOrganizations { get; set; }
        public virtual ICollection<OrganizationToDoList> OrganizationToDoLists { get; set; }
        public virtual RefOrganizationType RefOrganizationType { get; set; }
        public virtual RefSALTSchoolType RefSALTSchoolType { get; set; }
        public virtual RefState RefState { get; set; }
        public virtual ICollection<RefOrganizationProduct> RefOrganizationProducts { get; set; }
    }
    
}
