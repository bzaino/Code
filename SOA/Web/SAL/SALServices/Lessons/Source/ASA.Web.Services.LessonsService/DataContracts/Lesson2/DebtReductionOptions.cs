using System;

namespace ASA.Web.Services.LessonsService.DataContracts.Lesson2
{
    public class DebtReductionOptions
    {
        /// <summary>
        /// Gets or sets the increase monthly payment.
        /// </summary>
        /// <value>
        /// The increase monthly payment.
        /// </value>
        public Decimal IncreaseMonthlyPayment { get; set; }

        /// <summary>
        /// Gets or sets the pay cash for recurring expenses.
        /// </summary>
        /// <value>
        /// The pay cash for recurring expenses.
        /// </value>
        public Boolean PayCashForRecurringExpenses { get; set; }

        /// <summary>
        /// Gets or sets the pay cash for one time purchases.
        /// </summary>
        /// <value>
        /// The pay cash for one time purchases.
        /// </value>
        public Boolean PayCashForOneTimePurchases { get; set; }

        /// <summary>
        /// Gets or sets the lowered interest rate.
        /// </summary>
        /// <value>
        /// The lowered interest rate.
        /// </value>
        public Decimal LoweredInterestRate { get; set; }

        /// <summary>
        /// Gets or sets the extra payment amount.
        /// </summary>
        /// <value>
        /// The extra payment amount.
        /// </value>
        public Decimal ExtraPaymentAmount { get; set; }

        /// <summary>
        /// Gets or sets the extra payment month.
        /// </summary>
        /// <value>
        /// The extra payment month.
        /// </value>
        public int ExtraPaymentMonth { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int UserId { get; set; }


    }
}