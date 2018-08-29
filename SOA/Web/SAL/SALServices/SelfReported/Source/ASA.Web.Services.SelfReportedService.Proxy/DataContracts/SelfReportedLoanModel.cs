using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Services.Common;
using System.Runtime.Serialization;
using ASA.Web.WTF.Integration.MVC3.Validation;

namespace ASA.Web.Services.SelfReportedService.Proxy.DataContracts
{
    public class SelfReportedLoanModel : BaseWebModel
    {
        public SelfReportedLoanModel()
        {

        }

        //public SelfReportedLoanModel(SerializationInfo info, StreamingContext context)
        //{
        //    this.IndividualId = info.GetString("a07_ind_cst_key");
        //    this.LoanTypeId = info.GetString("a07_loan_type");
        //    this.LoanStatusId = info.GetString("a07_loan_status");
        //    this.AccountNickname = info.GetString("a07_account_nickname");
        //    this.HolderName = info.GetString("a07_holder_name");
        //    this.SchoolName = info.GetString("a07_school_name");
        //    this.ServicerName = info.GetString("a07_servicer_name");
        //    this.ServicerWebAddress = info.GetString("a07_servicer_url");
        //    this.PrincipalBalanceOutstandingAmount = info.GetDecimal("a07_principal_balance_outstanding_amount");
        //    this.PaymentDueAmount = info.GetDecimal("a07_payment_due_amount");
        //    this.NextPaymentDueAmount = info.GetDecimal("a07_next_payment_due_amount");
        //    this.NextPaymentDueDate = info.GetDateTime("a07_next_payment_due_date");
        //    this.IsActive = //"a07_active_flag",
        //    this.InterestRate = info.GetDouble("a07_interest_rate");
        //    this.DateReceived = info.GetDateTime("a07_received_date");
        //    this.OriginalLoanAmount = info.GetDecimal("a07_original_loan_amount");
        //    this.LoanTerm = info.GetInt32("a07_loan_term");
        //    //"a07_delete_flag"
        //}

        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("a07_ind_cst_key", this.IndividualId);
        //    info.AddValue("a07_loan_type", this.LoanTypeId);
        //    info.AddValue("a07_loan_status", this.LoanStatusId);
        //    info.AddValue("a07_account_nickname", this.AccountNickname);
        //    info.AddValue("a07_holder_name", this.HolderName);
        //    info.AddValue("a07_school_name", this.SchoolName);
        //    info.AddValue("a07_servicer_name", this.ServicerName);
        //    info.AddValue("a07_servicer_url", this.ServicerWebAddress );
        //    info.AddValue("a07_principal_balance_outstanding_amount", this.PrincipalBalanceOutstandingAmount);
        //    info.AddValue("a07_payment_due_amount", this.PaymentDueAmount);
        //    info.AddValue("a07_next_payment_due_amount", this.NextPaymentDueAmount);
        //    info.AddValue("a07_next_payment_due_date", this.NextPaymentDueDate);
        //    //"a07_active_flag",
        //    info.AddValue("a07_interest_rate", this.InterestRate);
        //    info.AddValue("a07_received_date", this.DateReceived);
        //    info.AddValue("a07_original_loan_amount", this.OriginalLoanAmount);
        //    info.AddValue("a07_loan_term", this.LoanTerm);
        //    //"a07_delete_flag"
        //}


        [DisplayName("ID")]
        [DefaultValue(0)]
        [RegularExpression(RegexStrings.GUID)]
        public string LoanSelfReportedEntryId { get; set; } //a07_key

        [Required]
        [RegularExpression(RegexStrings.GUID)]
        public string IndividualId { get; set; } //a07_ind_cst_key

        [DisplayName("Loan Status")]
        [StringLength(2)] //from DB column length
        public string LoanStatusId { get; set; } //a07_loan_status

        [Required]
        [DisplayName("Loan Type")]
        [StringLength(50)] //from DB column length
        public string LoanTypeId { get; set; } //a07_loan_type

        [DisplayName("Account Nickname")]
        [StringLength(50)] //from DB column length
        public string AccountNickname { get; set; } //a07_account_nickname

        [DisplayName("Holder")]
        [StringLength(80)] //from DB column length
        public string HolderName { get; set; } //a07_holder_name

        [DisplayName("School")]
        [StringLength(80)] //from DB column length
        public string SchoolName { get; set; } //a07_school_name

        [DisplayName("Servicer")]
        [StringLength(80)] //from DB column length
        public string ServicerName { get; set; } //a07_servicer_name

        [DisplayName("Servicer Web Address")]
        [StringLength(120)] //from DB column length
        public string ServicerWebAddress { get; set; } //a07_servicer_url

        [Required]
        [DisplayName("Interest Rate")]
        [Range(0.0,100.0, ErrorMessage="Interest rate must be greater than 0 and less than 100")]
        public double? InterestRate { get; set; } //a07_interest_rate

        [Required]
        [DisplayName("Current Balance")]
        [RegularExpression(RegexStrings.CURRENCY, ErrorMessage = "Must be in US currency format 0.99")]
        public decimal? PrincipalBalanceOutstandingAmount { get; set; } //a07_principal_balance_outstanding_amount

        [DisplayName("Payment Due")]
        [RegularExpression(RegexStrings.CURRENCY, ErrorMessage = "Must be in US currency format 0.99")]
        public decimal? PaymentDueAmount { get; set; } //a07_payment_due_amount

        [DisplayName("Next Payment Due Amount")]
        [RegularExpression(RegexStrings.CURRENCY, ErrorMessage = "Must be in US currency format 0.99")]
        public decimal? NextPaymentDueAmount { get; set; } //a07_next_payment_due_amount

        [DisplayName("Next Payment Due Date")]
        public DateTime? NextPaymentDueDate { get; set; } //a07_next_payment_due_date

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; } //a07_active_flag

        [Required]
        [DisplayName("Received Year")]
        [RegularExpression("^(0|[1-9][0-9]*)$")]
        public int ReceivedYear { get; set; } //a07_received_year

        [Required]
        [DefaultValue(0)]
        [DisplayName("Original Loan Amount")]
        [RegularExpression(RegexStrings.CURRENCY, ErrorMessage = "Must be in US currency format 0.99")]
        public decimal? OriginalLoanAmount { get; set; } //a07_original_loan_amount

        [Required]
        [DefaultValue(10)]
        [DisplayName("Loan Term")]
        [Range(1,100)]
        public int LoanTerm { get; set; } //a07_loan_term

        public DateTime DateAdded { get; set; }

       // [Required]
       // [StringLength(15, MinimumLength = 1, ErrorMessage = "Must have a source")]
        public string LoanSource { get; set; }
    }
}
