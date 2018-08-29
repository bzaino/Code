using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson3
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(MemberLessonContract))]
    [KnownType(typeof(ExpenseContract))]
    [KnownType(typeof(LoanTypeContract))]
    [KnownType(typeof(RepaymentOptionsContract))]
    [KnownType(typeof(DefermentContract))]
    [KnownType(typeof(FasterRepaymentContract))]
    [KnownType(typeof(LowerPaymentContract))]
    [KnownType(typeof(StandardRepaymentContract))]
    [KnownType(typeof(FavoriteRepaymentPlanContract))]
    public class Lesson3Contract
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [DataMember]
        public MemberLessonContract User { get; set; }

        /// <summary>
        /// Gets or sets the loan types.
        /// </summary>
        /// <value>
        /// The loan types.
        /// </value>
        [DataMember]
        public IList<LoanTypeContract> LoanTypes { get; set; }

        /// <summary>
        /// Gets or sets faster repayment options achieved by paying more each month.
        /// </summary>
        /// <value>
        /// Faster repayment options.
        /// </value>
        [DataMember]
        public FasterRepaymentContract FasterRepaymentOptions { get; set; }

        /// <summary>
        /// Gets or sets repayment deferment options.
        /// </summary>
        /// <value>
        /// Payment deferment options.
        /// </value>
        [DataMember]
        public DefermentContract DefermentOptions { get; set; }

        /// <summary>
        /// Gets or sets the lower payments.
        /// </summary>
        /// <value>
        /// The lower payments.
        /// </value>
        [DataMember]
        public LowerPaymentContract LowerPaymentOptions { get; set; }

        /// <summary>
        /// Gets or sets the standard repayment.
        /// </summary>
        /// <value>
        /// The standard repayment.
        /// </value>
        [DataMember]
        public StandardRepaymentContract StandardRepaymentOptions { get; set; }

        /// <summary>
        /// Gets or sets the favorite repayment plans.
        /// </summary>
        /// <value>
        /// The favorite repayment plans.
        /// </value>
        [DataMember]
        public IList<FavoriteRepaymentPlanContract> FavoriteRepaymentPlans { get; set; }

    }
}
