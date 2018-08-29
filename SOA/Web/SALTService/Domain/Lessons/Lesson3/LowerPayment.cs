using System;
using System.Collections.Generic;

namespace Asa.Salt.Web.Services.Domain.Lessons.Lesson3
{
    public class LowerPayment 
    {

        /// <summary>
        /// Gets or sets the income based repayment.
        /// </summary>
        /// <value>
        /// The income based repayment.
        /// </value>
        public IncomeBasedRepayment IncomeBasedRepayment { get; set; }

        /// <summary>
        /// Gets or sets the extended repayment.
        /// </summary>
        /// <value>
        /// The extended repayment.
        /// </value>
        public ExtendedRepayment ExtendedRepayment { get; set; }

        /// <summary>
        /// Gets or sets the income sensitive repayment.
        /// </summary>
        /// <value>
        /// The income sensitive repayment.
        /// </value>
        public IncomeSensitiveRepayment IncomeSensitiveRepayment { get; set; }

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

    public class IncomeBasedRepayment
    {
        /// <summary>
        /// Gets or sets the income based yearly income.
        /// </summary>
        /// <value>
        /// The income based yearly income.
        /// </value>
        public Decimal AnnualIncome { get; set; }

        /// <summary>
        /// Gets or sets the financial dependents.
        /// </summary>
        /// <value>
        /// The financial dependents.
        /// </value>
        public int FinancialDependents { get; set; }
    }

    public class ExtendedRepayment
    {
        /// <summary>
        /// Gets or sets the length of the extended.
        /// </summary>
        /// <value>
        /// The length of the extended.
        /// </value>
        public int ExtensionYears { get; set; }
    }

    public class IncomeSensitiveRepayment
    {
        /// <summary>
        /// Gets or sets the income sensitive yearly income.
        /// </summary>
        /// <value>
        /// The income sensitive yearly income.
        /// </value>
        public Decimal AnnualIncome { get; set; }

        /// <summary>
        /// Gets or sets the income sensitive income percentage.
        /// </summary>
        /// <value>
        /// The income sensitive income percentage.
        /// </value>
        public Double IncomePercentage { get; set; }
    }
}