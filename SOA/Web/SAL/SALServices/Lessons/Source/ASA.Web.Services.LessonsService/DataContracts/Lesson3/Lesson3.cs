using System;
using System.Collections.Generic;

namespace ASA.Web.Services.LessonsService.DataContracts.Lesson3
{
    public class Lesson3 : Lessons
    {
        ///// <summary>
        ///// Gets or sets the user.
        ///// </summary>
        ///// <value>
        ///// The user.
        ///// </value>
        //public int UserId { get; set; }

        ///// <summary>
        ///// Gets or sets the individual id.
        ///// </summary>
        ///// <value>
        ///// The individual id.
        ///// </value>
        //public Guid IndividualId { get; set; }

        ///// <summary>
        ///// Gets or sets the user.
        ///// </summary>
        ///// <value>
        ///// The user.
        ///// </value>
        //public User User { get; set; }

        ///// <summary>
        ///// Gets or sets the loan types.
        ///// </summary>
        ///// <value>
        ///// The loan types.
        ///// </value>
        public IList<LoanType> LoanTypes { get; set; }

        /// <summary>
        /// Gets or sets faster repayment options achieved by paying more each month.
        /// </summary>
        /// <value>
        /// Faster repayment options.
        /// </value>
        public IList<FasterRepayment> FasterRepaymentOptions { get; set; }

        /// <summary>
        /// Gets or sets repayment deferment options.
        /// </summary>
        /// <value>
        /// Payment deferment options.
        /// </value>
        public IList<Deferment> DefermentOptions { get; set; }

        /// <summary>
        /// Gets or sets the lower payments.
        /// </summary>
        /// <value>
        /// The lower payments.
        /// </value>
        public IList<LowerPayment> LowerPaymentOptions { get; set; }

        /// <summary>
        /// Gets or sets the standard repayment.
        /// </summary>
        /// <value>
        /// The standard repayment.
        /// </value>
        public StandardRepayment StandardRepayment { get; set; }

        /// <summary>
        /// Gets or sets the favorite repayment plans.
        /// </summary>
        /// <value>
        /// The favorite repayment plans.
        /// </value>
        public IList<Favorite> FavoriteRepaymentPlans { get; set; }
    }
}
