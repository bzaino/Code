using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Asa.Salt.Web.Common.Types.Enums;
using ASA.Web.Common.Validation;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.SelfReportedService.Proxy.DataContracts
{
    public class SelfReportedLoanModel : BaseWebModel
    {
        public SelfReportedLoanModel()
        {

        }

        [DisplayName("ID")]
        [DefaultValue(0)]
        public string LoanSelfReportedEntryId { get; set; }

        [RegularExpression(RegexStrings.GUID)]
        public string IndividualId { get; set; } 

        [Required]
        public int MemberId { get; set; }

        [DisplayName("Loan Status")]
        [StringLength(2)] 
        public string LoanStatusId { get; set; }

        [Required]
        [DisplayName("Loan Type")]
        [StringLength(100)] 
        public string LoanTypeId { get; set; } 

        [DisplayName("Interest Rate")]
        [Required]
        [Range(0.0,100.0, ErrorMessage="Interest rate must be greater than 0 and less than 100")]
        public double? InterestRate { get; set; } 

        [Required]
        [DisplayName("Current Balance")]
        [RegularExpression(RegexStrings.CURRENCY, ErrorMessage = "Must be in US currency format 0.99")]
        public decimal? PrincipalBalanceOutstandingAmount { get; set; }


        [Required]
        [DisplayName("Received Year")]
        [RegularExpression("^(0|[1-9][0-9]*)$")]
        public int ReceivedYear { get; set; } 

        [Required]
        [DefaultValue(0)]
        [DisplayName("Original Loan Amount")]
        [RegularExpression(RegexStrings.CURRENCY, ErrorMessage = "Must be in US currency format 0.99")]
        public decimal? OriginalLoanAmount { get; set; }

        [Required]
        [DefaultValue(10)]
        [DisplayName("Loan Term")]
        [Range(1,100)]
        public int LoanTerm { get; set; } 

        public DateTime DateAdded { get; set; }

        public DateTime? LastModified { get; set; }

       // [Required]
       // [StringLength(15, MinimumLength = 1, ErrorMessage = "Must have a source")]
        public string LoanSource { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public int RecordSourceId { get; set; }

        public LoanInterestRateType InterestRateType { get; set; }

        [StringLength(100)]
        public string LoanName { get; set; }

        public DateTime? OriginalLoanDate { get; set; }

        public decimal? MonthlyPaymentAmount { get; set; }

        [Required]
        [StringLength(65)]
        public string ServicingOrganizationName { get; set; }

    }
}
