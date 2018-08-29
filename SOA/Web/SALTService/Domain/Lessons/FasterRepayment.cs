using System;

namespace Asa.Salt.Web.Services.Domain.Lessons
{
    public class FasterRepayment  
    {
        /// <summary>
        /// Gets or sets the faster repayment id.
        /// </summary>
        /// <value>
        /// The faster repayment id.
        /// </value>
        public Guid FasterRepaymentId { get; set; }

        /// <summary>
        /// Gets or sets the additional monthly payment.
        /// </summary>
        /// <value>
        /// The additional monthly payment.
        /// </value>
        public Decimal AdditionalMonthlyPayment { get; set; }

        /// <summary>
        /// Gets or sets the shorter timeline.
        /// </summary>
        /// <value>
        /// The shorter timeline.
        /// </value>
        public int ShorterTimeline { get; set; }

        /// <summary>
        /// Gets or sets the lower interest rate.
        /// </summary>
        /// <value>
        /// The lower interest rate.
        /// </value>
        public decimal LowerInterestRate { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int LessonUserId { get; set; }
     
    }
}