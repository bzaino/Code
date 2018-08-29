using System;

namespace ASA.Web.Services.LessonsService.DataContracts.Lesson3
{
    public class LowerPayment 
    {
        /// <summary>
        /// Gets or sets the type of the plan.
        /// </summary>
        /// <value>
        /// The type of the plan.
        /// </value>
        public string PlanType { get; set; }

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
        public int UserId { get; set; }

      
    }
}