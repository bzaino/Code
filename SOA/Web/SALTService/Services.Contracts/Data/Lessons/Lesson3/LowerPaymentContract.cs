using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson3
{
    public class LowerPaymentContract
    {

        /// <summary>
        /// Gets or sets the income based repayment.
        /// </summary>
        /// <value>
        /// The income based repayment.
        /// </value>
        [DataMember]
        public IncomeBasedRepaymentContract IncomeBasedRepayment { get; set; }

        /// <summary>
        /// Gets or sets the extended repayment.
        /// </summary>
        /// <value>
        /// The extended repayment.
        /// </value>
        [DataMember]
        public ExtendedRepaymentContract ExtendedRepayment { get; set; }

        /// <summary>
        /// Gets or sets the income sensitive repayment.
        /// </summary>
        /// <value>
        /// The income sensitive repayment.
        /// </value>
        [DataMember]
        public IncomeSensitiveRepaymentContract IncomeSensitiveRepayment { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        [DataMember]
        public int LessonUserId { get; set; }

    }

    public class IncomeBasedRepaymentContract
    {
        /// <summary>
        /// Gets or sets the income based yearly income.
        /// </summary>
        /// <value>
        /// The income based yearly income.
        /// </value>
        [DataMember]
        public Decimal AnnualIncome { get; set; }

        /// <summary>
        /// Gets or sets the financial dependents.
        /// </summary>
        /// <value>
        /// The financial dependents.
        /// </value>
        [DataMember]
        public int FinancialDependents { get; set; }
    }

    public class ExtendedRepaymentContract
    {
        /// <summary>
        /// Gets or sets the length of the extended.
        /// </summary>
        /// <value>
        /// The length of the extended.
        /// </value>
        [DataMember]
        public int ExtensionYears { get; set; }
    }

    public class IncomeSensitiveRepaymentContract
    {
        /// <summary>
        /// Gets or sets the income sensitive yearly income.
        /// </summary>
        /// <value>
        /// The income sensitive yearly income.
        /// </value>
        [DataMember]
        public Decimal AnnualIncome { get; set; }

        /// <summary>
        /// Gets or sets the income sensitive income percentage.
        /// </summary>
        /// <value>
        /// The income sensitive income percentage.
        /// </value>
        [DataMember]
        public Double IncomePercentage { get; set; }
    }
}