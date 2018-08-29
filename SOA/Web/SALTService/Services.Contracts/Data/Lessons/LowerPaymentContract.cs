using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons
{
    public class LowerPaymentContract
    {
        /// <summary>
        /// Gets or sets the income based yearly income.
        /// </summary>
        /// <value>
        /// The income based yearly income.
        /// </value>
        [DataMember]
        public Decimal IncomeBasedYearlyIncome { get; set; }

        /// <summary>
        /// Gets or sets the financial dependents.
        /// </summary>
        /// <value>
        /// The financial dependents.
        /// </value>
        [DataMember]
        public int FinancialDependents { get; set; }

        /// <summary>
        /// Gets or sets the length of the extended.
        /// </summary>
        /// <value>
        /// The length of the extended.
        /// </value>
        [DataMember]
        public int ExtendedLength { get; set; }

        /// <summary>
        /// Gets or sets the income sensitive yearly income.
        /// </summary>
        /// <value>
        /// The income sensitive yearly income.
        /// </value>
        [DataMember]
        public Decimal IncomeSensitiveYearlyIncome { get; set; }

        /// <summary>
        /// Gets or sets the income sensitive income percentage.
        /// </summary>
        /// <value>
        /// The income sensitive income percentage.
        /// </value>
        [DataMember]
        public Double IncomeSensitiveIncomePercentage { get; set; }



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