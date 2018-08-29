using System;

namespace Asa.Salt.Web.Services.Domain.Lessons.Lesson3
{
    public class LoanType
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the loan.
        /// </summary>
        /// <value>
        /// The name of the loan.
        /// </value>
        public string DegreeType { get; set; }

        /// <summary>
        /// Gets or sets the loan amount.
        /// </summary>
        /// <value>
        /// The loan amount.
        /// </value>
        public Decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the yearly income.
        /// </summary>
        /// <value>
        /// The yearly income.
        /// </value>
        public Decimal AnnualIncome { get; set; }

        /// <summary>
        /// Gets or sets the financial dependents.
        /// </summary>
        /// <value>
        /// The financial dependents.
        /// </value>
        public int FinancialDependents { get; set; }

        /// <summary>
        /// Gets or sets the State.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the interest rate.
        /// </summary>
        /// <value>
        /// The interest rate.
        /// </value>
        public Decimal InterestRate { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int LessonUserId { get; set; }

        
    }
}