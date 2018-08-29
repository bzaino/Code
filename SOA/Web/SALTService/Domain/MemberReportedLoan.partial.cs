using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Asa.Salt.Web.Common.Types.Enums;

namespace Asa.Salt.Web.Services.Domain
{
    public partial class MemberReportedLoan
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public override int Id
        {
            get { return this.MemberReportedLoanId; }
        }

        /// <summary>
        /// Gets or sets the type of the interest rate.
        /// </summary>
        /// <value>
        /// The type of the interest rate.
        /// </value>
        public LoanInterestRateType InterestRateType { get; set; }

        /// <summary>
        /// Validates the specified validation context.
        /// </summary>
        /// <returns></returns>
        public override IList<ValidationResult> Validate()
        {
            var validationDescriptors = new AssociatedMetadataTypeTypeDescriptionProvider(typeof (MemberReportedLoan),typeof (MemberReportedLoanValidation));

            TypeDescriptor.AddProviderTransparent(validationDescriptors, typeof(MemberReportedLoan));

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);

            Validator.TryValidateObject(this, validationContext, results, true);

            TypeDescriptor.RemoveProviderTransparent(validationDescriptors, typeof(MemberReportedLoan));

            return results.ToList();
        }

    }

    /// <summary>
    /// Validation rules for user reported loans.
    /// </summary>
    public class MemberReportedLoanValidation
    {
        private const string CurrencyRegularExpression = @"^(\d{1,9})(\.\d{1,2})?$";

        [Required]
        public int MemberId { get; set; }

        [StringLength(2)]
        public string LoanStatus { get; set; }

        [Required]
        [StringLength(100)]
        public string LoanType { get; set; }

        [Required]
        [Range(0.0, 100.0, ErrorMessage = "Interest rate must be greater than 0 and less than 100")]
        public decimal? InterestRate { get; set; }

        [Required]
        [RegularExpression(CurrencyRegularExpression, ErrorMessage = "Must be in US currency format 0.99")]
        public decimal? PrincipalOutstandingAmount { get; set; }

        [Required]
        [RegularExpression("^(0|[1-9][0-9]*)$")]
        public int? ReceivedYear { get; set; }

        [Required]
        [RegularExpression(CurrencyRegularExpression, ErrorMessage = "Must be in US currency format 0.99")]
        public decimal? OriginalLoanAmount { get; set; }

        [Required]
        [Range(1, 100)]
        public int? LoanTerm { get; set; }

        [Required]
        [Range(1, 4)]
        public int? RecordSourceId { get; set; }

        [StringLength(100)]
        public string LoanName { get; set; }

        public DateTime? OriginalLoanDate { get; set; }

        public decimal? MonthlyPaymentAmount { get; set; }

        //[Required]
        [StringLength(65)]
        public string ServicingOrganizationName { get; set; }
    }
}
