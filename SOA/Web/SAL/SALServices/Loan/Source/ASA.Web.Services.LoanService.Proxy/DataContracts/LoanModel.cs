using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.LoanService.Proxy.DataContracts
{
    public class LoanModel : BaseWebModel
    {
        [Required]
        [DisplayName("Loan ID")]
        public int LoanId { get; set; }

        [Required]
        [DisplayName("Customer ID")]
        public int CustomerId { get; set; }

        [Required]
        [DisplayName("External ID")]
        [DefaultValue("")]
        public string ExternalId { get; set; }

        [Required]
        [DisplayName("Loan Type ID")]
        [DefaultValue("")]
        public string LoanTypeID { get; set; }

        [Required]
        [DisplayName("Loan Type")]
        [DefaultValue("")]
        public string LoanType { get; set; }

        [Required]
        [DisplayName("Loan Status ID")]
        [DefaultValue("")]
        public string LoanStatusID { get; set; }

        [Required]
        [DisplayName("Loan Status")]
        [DefaultValue("")]
        public string LoanStatus { get; set; }

        [DisplayName("Loan Status Date")]
        [DataType(DataType.Date)]
        public DateTime? LoanStatusDate { get; set; }

        [DisplayName("Entered Repayment Date")]
        [DataType(DataType.Date)]
        public DateTime? EnteredRepaymentDate { get; set; }

        [Required]
        [DisplayName("Principal Balance")]
        [DataType(DataType.Currency)]
        [DefaultValue(0.00)]
        public decimal? PrincipalBalance { get; set; }

        [DisplayName("Principal Balance Date")]
        [DataType(DataType.Date)]
        public DateTime? PrincipalBalanceDate { get; set; }

        [DisplayName("Interest Rate")]
        [DefaultValue(0.00)]
        public double? InterestRate { get; set; }

        [DisplayName("Payment Due Date")]
        [DataType(DataType.Date)]
        public DateTime? PaymentDueDate { get; set; }

        [Required]
        [DisplayName("Payment Due")]
        [DataType(DataType.Currency)]
        [DefaultValue(0.00)]
        public decimal? PaymentDue { get; set; }

        [Required]
        [DisplayName("School")]
        [DefaultValue("")]
        public string SchoolName { get; set; }

        [Required]
        [DisplayName("Servicer")]
        [DefaultValue("")]
        public string LenderName { get; set; }
    }
}
