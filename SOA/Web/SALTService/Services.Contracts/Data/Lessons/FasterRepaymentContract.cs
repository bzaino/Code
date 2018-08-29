using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons
{
    public class FasterRepaymentContract
    {
        /// <summary>
        /// Gets or sets the faster repayment id.
        /// </summary>
        /// <value>
        /// The faster repayment id.
        /// </value>
        [DataMember]
        public Guid FasterRepaymentId { get; set; }

        /// <summary>
        /// Gets or sets the additional monthly payment.
        /// </summary>
        /// <value>
        /// The additional monthly payment.
        /// </value>
        [DataMember]
        public Decimal AdditionalMonthlyPayment { get; set; }

        /// <summary>
        /// Gets or sets the shorter timeline.
        /// </summary>
        /// <value>
        /// The shorter timeline.
        /// </value>
        [DataMember]
        public int ShorterTimeline { get; set; }

        /// <summary>
        /// Gets or sets the lower interest rate.
        /// </summary>
        /// <value>
        /// The lower interest rate.
        /// </value>
        [DataMember]
        public decimal LowerInterestRate { get; set; }



        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        [DataMember]
        public int LessonUserId { get; set; }
    }
}