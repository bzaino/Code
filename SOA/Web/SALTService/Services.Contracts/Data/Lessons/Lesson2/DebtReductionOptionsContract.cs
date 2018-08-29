using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson2
{
    public class DebtReductionOptionsContract
    {
        /// <summary>
        /// Gets or sets the increase monthly payment.
        /// </summary>
        /// <value>
        /// The increase monthly payment.
        /// </value>
        [DataMember]
        public Decimal IncreaseMonthlyPayment { get; set; }

        /// <summary>
        /// Gets or sets the pay cash for recurring expenses.
        /// </summary>
        /// <value>
        /// The pay cash for recurring expenses.
        /// </value>
        [DataMember]
        public Boolean PayCashForRecurringExpenses { get; set; }

        /// <summary>
        /// Gets or sets the pay cash for one time purchases.
        /// </summary>
        /// <value>
        /// The pay cash for one time purchases.
        /// </value>
        [DataMember]
        public Boolean PayCashForOneTimePurchases { get; set; }

        /// <summary>
        /// Gets or sets the lower your interest rate.
        /// </summary>
        /// <value>
        /// The lower your interest rate.
        /// </value>
        [DataMember]
        public Boolean LowerYourInterestRate { get; set; }

        /// <summary>
        /// Gets or sets the lowered interest rate.
        /// </summary>
        /// <value>
        /// The lowered interest rate.
        /// </value>
        [DataMember]
        public Decimal LoweredInterestRate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [plan an extra payment].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [plan an extra payment]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool PlanAnExtraPayment { get; set; }

        /// <summary>
        /// Gets or sets the extra payment amount.
        /// </summary>
        /// <value>
        /// The extra payment amount.
        /// </value>
        [DataMember]
        public Decimal ExtraPaymentAmount { get; set; }

        /// <summary>
        /// Gets or sets the extra payment month.
        /// </summary>
        /// <value>
        /// The extra payment month.
        /// </value>
        [DataMember]
        public int ExtraPaymentMonth { get; set; }



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