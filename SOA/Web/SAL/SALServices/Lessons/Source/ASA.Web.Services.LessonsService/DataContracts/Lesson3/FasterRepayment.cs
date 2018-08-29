using System;

namespace ASA.Web.Services.LessonsService.DataContracts.Lesson3
{
    public class FasterRepayment  
    {
        /// <summary>
        /// Gets or sets the type of the plan.
        /// </summary>
        /// <value>
        /// The type of the plan.
        /// </value>
        public string PlanType { get; set; }

        /// <summary>
        /// Gets or sets the faster repayment id.
        /// </summary>
        /// <value>
        /// The faster repayment id.
        /// </value>
        public int FasterRepaymentId { get; set; }

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
        public int UserId { get; set; }


     
    }
}