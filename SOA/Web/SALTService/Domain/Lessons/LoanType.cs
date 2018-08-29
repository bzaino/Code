using System;

namespace Asa.Salt.Web.Services.Domain.Lessons
{
    public class LoanType
    {
        /// <summary>
        /// Gets or sets the name of the loan.
        /// </summary>
        /// <value>
        /// The name of the loan.
        /// </value>
        public string LoanName { get; set; }

        /// <summary>
        /// Gets or sets the loan amount.
        /// </summary>
        /// <value>
        /// The loan amount.
        /// </value>
        public Decimal LoanAmount { get; set; }

        /// <summary>
        /// Gets or sets the lesson user id.
        /// </summary>
        /// <value>
        /// The lesson user id.
        /// </value>
        public int LessonUserId { get; set; }

        
    }
}