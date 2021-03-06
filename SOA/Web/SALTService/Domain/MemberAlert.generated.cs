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
    public partial class MemberAlert : DomainObject<int> 
    {
        public int MemberAlertId { get; set; }
        public int MemberId { get; set; }
        public int AlertTypeId { get; set; }
        public string AlertTitle { get; set; }
        public string AlertLogoUrl { get; set; }
        public string AlertMessage { get; set; }
        public Nullable<System.DateTime> AlertIssueDate { get; set; }
        public bool IsAlertViewed { get; set; }
        public string AlertLink { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual AlertType AlertType { get; set; }
    }
    
}
