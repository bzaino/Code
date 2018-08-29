using System.Collections.Generic;

namespace Asa.Salt.Web.Services.Domain.Lessons.Lesson1
{
    public class Lesson1
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public MemberLesson User { get; set; }

        /// <summary>
        /// Gets or sets the expenses.
        /// </summary>
        /// <value>
        /// The expenses.
        /// </value>
        public IList<Expense> Expenses { get; set; }

        /// <summary>
        /// Gets or sets the incomes.
        /// </summary>
        /// <value>
        /// The incomes.
        /// </value>
        public IList<Income> Incomes { get; set; }

        /// <summary>
        /// Gets or sets the goal.
        /// </summary>
        /// <value>
        /// The goal.
        /// </value>
        public Goal Goal { get; set; }
    }
}
