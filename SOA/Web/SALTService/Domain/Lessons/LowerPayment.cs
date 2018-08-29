using System;

namespace Asa.Salt.Web.Services.Domain.Lessons
{
    public class LowerPayment 
    {
        /// <summary>
        /// Gets or sets the income based yearly income.
        /// </summary>
        /// <value>
        /// The income based yearly income.
        /// </value>
        public Decimal IncomeBasedYearlyIncome { get; set; }

        /// <summary>
        /// Gets or sets the financial dependents.
        /// </summary>
        /// <value>
        /// The financial dependents.
        /// </value>
        public int FinancialDependents { get; set; }

        /// <summary>
        /// Gets or sets the length of the extended.
        /// </summary>
        /// <value>
        /// The length of the extended.
        /// </value>
        public int ExtendedLength { get; set; }

        /// <summary>
        /// Gets or sets the income sensitive yearly income.
        /// </summary>
        /// <value>
        /// The income sensitive yearly income.
        /// </value>
        public Decimal IncomeSensitiveYearlyIncome { get; set; }

        /// <summary>
        /// Gets or sets the income sensitive income percentage.
        /// </summary>
        /// <value>
        /// The income sensitive income percentage.
        /// </value>
        public Double IncomeSensitiveIncomePercentage { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int LessonUserId { get; set; }
      
    }
}