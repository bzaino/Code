using System;
using System.Collections.Generic;

namespace Asa.Salt.Web.Services.Domain.Lessons.Lesson3
{
    public class FasterRepayment  
    {
        /// <summary>
        /// Gets or sets the additional monthly payment.
        /// </summary>
        /// <value>
        /// The additional monthly payment.
        /// </value>
        public AdditionalPayment AdditionalMonthlyPayment { get; set; }

        /// <summary>
        /// Gets or sets the loan payment timeline.
        /// </summary>
        /// <value>
        /// The loan payment timeline.
        /// </value>
        public CustomTimeline LoanPaymentTimeline { get; set; }

        /// <summary>
        /// Gets or sets the better interest rate.
        /// </summary>
        /// <value>
        /// The better interest rate.
        /// </value>
        public BetterInterestRate BetterInterestRate { get; set; }

        /// <summary>
        /// Gets or sets the favorite repayment plan.
        /// </summary>
        /// <value>
        /// The favorite repayment plan.
        /// </value>
        public IList<string> FavoriteRepaymentPlans { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int LessonUserId { get; set; }
     
    }

    public class AdditionalPayment
    {
        /// <summary>
        /// Gets or sets the additional monthly payment.
        /// </summary>
        /// <value>
        /// The additional monthly payment.
        /// </value>
        public Decimal MonthlyPayment { get; set; }
    }

    public class CustomTimeline
    {
        /// <summary>
        /// Gets or sets the shorter timeline.
        /// </summary>
        /// <value>
        /// The shorter timeline.
        /// </value>
        public int Timeline { get; set; }
    }

    public class BetterInterestRate
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