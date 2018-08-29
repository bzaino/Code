using System.Collections.Generic;

namespace Asa.Salt.Web.Services.Domain.Lessons.Lesson2
{
    public class Lesson2
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public MemberLesson User { get; set; }

        /// <summary>
        /// Gets or sets the current balance.
        /// </summary>
        /// <value>
        /// The current balance.
        /// </value>
        public CardInformation CurrentBalance{ get; set; }

        /// <summary>
        /// Gets or sets the imported expenses.
        /// </summary>
        /// <value>
        /// The imported expenses.
        /// </value>
        public IList<Expense> ImportedExpenses { get; set; }

        /// <summary>
        /// Gets or sets the recurring expenses.
        /// </summary>
        /// <value>
        /// The recurring expenses.
        /// </value>
        public IList<Expense> RecurringExpenses { get; set; }

        /// <summary>
        /// Gets or sets the one time expenses.
        /// </summary>
        /// <value>
        /// The one time expenses.
        /// </value>
        public IList<Expense> OneTimeExpenses { get; set; }

        /// <summary>
        /// Gets or sets the debt reduction options.
        /// </summary>
        /// <value>
        /// The debt reduction options.
        /// </value>
        public DebtReductionOptions DebtReductionOptions { get; set; }
    }
}
