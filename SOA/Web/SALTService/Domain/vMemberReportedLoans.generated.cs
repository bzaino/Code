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
    public partial class vMemberReportedLoans : DomainObject<int> 
    {
        public int MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsContactAllowed { get; set; }
        public string LoanType { get; set; }
        public string LoanStatus { get; set; }
        public Nullable<decimal> PrincipalOutstandingAmount { get; set; }
        public Nullable<decimal> InterestRate { get; set; }
        public Nullable<int> ReceivedYear { get; set; }
        public Nullable<decimal> OriginalLoanAmount { get; set; }
        public Nullable<int> LoanTerm { get; set; }
        public int RefRecordSourceID { get; set; }
        public string LoanName { get; set; }
        public Nullable<System.DateTime> OriginalLoanDate { get; set; }
        public Nullable<decimal> MonthlyPaymentAmount { get; set; }
        public string ServicingOrganizationName { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string RecordSourceName { get; set; }
        public string RecordSourceDescription { get; set; }
        public string LoanTypeName { get; set; }
    }
    
}
