using System.Collections.Generic;
using System.Linq;
using Asa.Salt.Web.Common.Types.Constants;

namespace Asa.Salt.Web.Services.Domain.Lessons.Lesson3
{
    public class Lesson3
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Lesson3"/> class.
        /// </summary>
        public Lesson3()
        {
            DefermentOptions = new Deferment();
            StandardRepaymentOptions = new StandardRepayment();
            LowerPaymentOptions = new LowerPayment();
            FasterRepaymentOptions = new FasterRepayment();
        }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public MemberLesson User { get; set; }

        /// <summary>
        /// Gets or sets the loan types.
        /// </summary>
        /// <value>
        /// The loan types.
        /// </value>
        public IList<LoanType> LoanTypes { get; set; }

        /// <summary>
        /// Gets or sets faster repayment options achieved by paying more each month.
        /// </summary>
        /// <value>
        /// Faster repayment options.
        /// </value>
        public FasterRepayment FasterRepaymentOptions { get; set; }

        /// <summary>
        /// Gets or sets repayment deferment options.
        /// </summary>
        /// <value>
        /// Payment deferment options.
        /// </value>
        public Deferment DefermentOptions { get; set; }

        /// <summary>
        /// Gets or sets the lower payments.
        /// </summary>
        /// <value>
        /// The lower payments.
        /// </value>
        public LowerPayment LowerPaymentOptions { get; set; }

        /// <summary>
        /// Gets or sets the standard repayment.
        /// </summary>
        /// <value>
        /// The standard repayment.
        /// </value>
        public StandardRepayment StandardRepaymentOptions { get; set; }

        /// <summary>
        /// Gets or sets the favorite repayment plans.
        /// </summary>
        /// <value>
        /// The favorite repayment plans.
        /// </value>
        private IList<FavoriteRepaymentPlan> _favoriteRepaymentPlans;
        public IList<FavoriteRepaymentPlan> FavoriteRepaymentPlans
        {
            get { return _favoriteRepaymentPlans; }
            set
            {
                _favoriteRepaymentPlans = value;

                if (DefermentOptions != null) DefermentOptions.FavoriteRepaymentPlans = _favoriteRepaymentPlans != null && _favoriteRepaymentPlans.Any(p => p.RepaymentPlan.Contains(RepaymentOptionTypes.HardshipBasedDeferment) || p.RepaymentPlan.Contains(RepaymentOptionTypes.Deferment)) ? _favoriteRepaymentPlans.Where(p => p.RepaymentPlan.Contains(RepaymentOptionTypes.Deferment) || p.RepaymentPlan.Contains(RepaymentOptionTypes.HardshipBasedDeferment)).Select(p => p.RepaymentPlan).ToList() : new List<string>();
                if (StandardRepaymentOptions != null) StandardRepaymentOptions.FavoriteRepaymentPlans = _favoriteRepaymentPlans != null && _favoriteRepaymentPlans.Any(p => p.RepaymentPlan.Contains(RepaymentOptionTypes.StandardRepayment)) ? _favoriteRepaymentPlans.Where(p => p.RepaymentPlan.Contains(RepaymentOptionTypes.StandardRepayment)).Select(p => p.RepaymentPlan).ToList() : new List<string>();
                if (FasterRepaymentOptions != null) FasterRepaymentOptions.FavoriteRepaymentPlans = _favoriteRepaymentPlans != null && _favoriteRepaymentPlans.Any(p => p.RepaymentPlan.Contains(RepaymentOptionTypes.PayMoreEachMonth) || p.RepaymentPlan.Contains(RepaymentOptionTypes.BetterInterestRate) || p.RepaymentPlan.Contains(RepaymentOptionTypes.SetYourOwnTimeline)) ? _favoriteRepaymentPlans.Where(p => p.RepaymentPlan.Contains(RepaymentOptionTypes.PayMoreEachMonth) || p.RepaymentPlan.Contains(RepaymentOptionTypes.BetterInterestRate) || p.RepaymentPlan.Contains(RepaymentOptionTypes.SetYourOwnTimeline)).Select(p => p.RepaymentPlan).ToList() : new List<string>();
                if (LowerPaymentOptions!=null) LowerPaymentOptions.FavoriteRepaymentPlans = _favoriteRepaymentPlans != null && _favoriteRepaymentPlans.Any(p => p.RepaymentPlan.Contains(RepaymentOptionTypes.GraduatedRepayment) || p.RepaymentPlan.Contains(RepaymentOptionTypes.IncomeBased) || p.RepaymentPlan.Contains(RepaymentOptionTypes.IncomeSensitive) || p.RepaymentPlan.Contains(RepaymentOptionTypes.ExtendedRepayment)) ? _favoriteRepaymentPlans.Where(p => p.RepaymentPlan.Contains(RepaymentOptionTypes.GraduatedRepayment) || p.RepaymentPlan.Contains(RepaymentOptionTypes.IncomeBased) || p.RepaymentPlan.Contains(RepaymentOptionTypes.IncomeSensitive) || p.RepaymentPlan.Contains(RepaymentOptionTypes.ExtendedRepayment)).Select(p => p.RepaymentPlan).ToList() : new List<string>();
            }
        }
    }
}
