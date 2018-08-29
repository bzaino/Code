using System;
using System.Runtime.Serialization;

namespace Asa.Salt.Web.Services.Contracts.Data.Lessons.Lesson3
{
    public class LoanTypeContract
    {
        /// <summary>
        /// Gets or sets the loan type id.
        /// </summary>
        /// <value>
        /// The loan type id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the type of the degree.
        /// </summary>
        /// <value>
        /// The type of the degree.
        /// </value>
        [DataMember]
        public string DegreeType { get; set; }

        /// <summary>
        /// Gets or sets the loan amount.
        /// </summary>
        /// <value>
        /// The loan amount.
        /// </value>
        [DataMember]
        public Decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the yearly income.
        /// </summary>
        /// <value>
        /// The yearly income.
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

        /// <summary>
        /// Gets or sets the State.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        [DataMember]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the interest rate.
        /// </summary>
        /// <value>
        /// The interest rate.
        /// </value>
        [DataMember]
        public Decimal InterestRate { get; set; }

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