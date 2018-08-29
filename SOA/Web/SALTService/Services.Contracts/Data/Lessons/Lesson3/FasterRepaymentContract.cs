using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson3
{
    public class FasterRepaymentContract
    {
        /// <summary>
        /// Gets or sets the additional monthly payment.
        /// </summary>
        /// <value>
        /// The additional monthly payment.
        /// </value>
        [DataMember]
        public AdditionalPaymentContract AdditionalMonthlyPayment { get; set; }

        /// <summary>
        /// Gets or sets the loan payment timeline.
        /// </summary>
        /// <value>
        /// The loan payment timeline.
        /// </value>
        [DataMember]
        public CustomTimelineContract LoanPaymentTimeline { get; set; }

        /// <summary>
        /// Gets or sets the better interest rate.
        /// </summary>
        /// <value>
        /// The better interest rate.
        /// </value>
        [DataMember]
        public BetterInterestRateContract BetterInterestRate { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        [DataMember]
        public int LessonUserId { get; set; }

    }

    public class AdditionalPaymentContract
    {
        /// <summary>
        /// Gets or sets the additional monthly payment.
        /// </summary>
        /// <value>
        /// The additional monthly payment.
        /// </value>
        public Decimal MonthlyPayment { get; set; }
    }

    public class CustomTimelineContract
    {
        /// <summary>
        /// Gets or sets the shorter timeline.
        /// </summary>
        /// <value>
        /// The shorter timeline.
        /// </value>
        public int Timeline { get; set; }
    }

    public class BetterInterestRateContract
    {
        /// <summary>
        /// Gets or sets the lower interest rate.
        /// </summary>
        /// <value>
        /// The lower interest rate.
        /// </value>
        public decimal LowerInterestRate { get; set; }
    }


}